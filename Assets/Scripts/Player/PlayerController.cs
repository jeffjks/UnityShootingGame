using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;

    public Boundary(float xMin, float xMax, float yMin, float yMax) {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }
}

public class PlayerController : PlayerControllerManager
{
    [SerializeField] private float m_Tilt = 30f;
    [SerializeField] private Boundary m_Boundary = new Boundary(-7f, 7f, -14.8f, -1f);
    [SerializeField] private Transform m_PlayerRevivePoint = null;
    [SerializeField] private GameObject m_PlayerShield = null;
    [SerializeField] private string m_Explosion = string.Empty;
    public float m_ReviveInvincibleTime = 3f;

    private float m_MaxPlayerCamera;
    private float m_DefaultRotation;
    private float m_TiltSpeed = 0.2f;
    private bool m_Invincibility = false;
    private float m_InvincibleTimer;
    private bool m_HasCollided = false;
    private float m_Speed, m_SlowSpeed, m_OverviewSpeed;
    private int m_MoveRawHorizontal = 0, m_MoveRawVertical = 0;
    private StringBuilder m_String = new StringBuilder();
    
    private SystemManager m_SystemManager = null;

    void Start()
    {
        m_MaxPlayerCamera = Size.CAMERA_MOVE_LIMIT;
        m_DefaultRotation = transform.eulerAngles[0];

        m_SystemManager = SystemManager.instance_sm;

        switch(m_PlayerManager.m_CurrentAttributes.m_Speed) {
            case 0:
                m_Speed = 6f;
                m_SlowSpeed = 4f;
                break;
            case 1:
                m_Speed = 6.75f;
                m_SlowSpeed = 4.2f;
                break;
            case 2:
                m_Speed = 7.5f; // 8.1
                m_SlowSpeed = 4.4f;
                break;
            default:
                break;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (m_PlayerManager.PlayerControlable) {
            if (!m_SystemManager.m_ReplayState) {
                m_MoveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
                m_MoveRawVertical = (int) Input.GetAxisRaw ("Vertical");
            }
        }
        Tilt(m_MoveRawHorizontal);
    }

    private void Tilt(float tilt_state) {
        Quaternion maxTilt = Quaternion.AngleAxis(- m_Tilt*tilt_state, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, maxTilt, m_TiltSpeed*Time.deltaTime*60);
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;
        
        Vector2 movement = new Vector2(m_MoveRawHorizontal, m_MoveRawVertical);
        if (m_PlayerManager.PlayerControlable) {
            if (m_SlowMode)
                m_Vector2 = movement * m_SlowSpeed;
            else
                m_Vector2 = movement * m_Speed;
        }
        else {
            m_MoveRawHorizontal = 0;
            m_MoveRawVertical = 0;
        }
        MoveVector();
        OverviewPosition();

        if (m_PlayerManager.PlayerControlable) {
            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position[0], m_Boundary.xMin, m_Boundary.xMax), 
                Mathf.Clamp(transform.position[1], m_Boundary.yMin, m_Boundary.yMax),
                Depth.PLAYER
            );
        }

        UpdateInvincible();
        UpdateRevivePoint();

        m_String.Append(transform.position);
        if (m_String.Length > 100)
            Debug.LogWarning(m_String);
    }

    private void UpdateRevivePoint() {
        float playerReviveX = transform.position.x;
        m_PlayerRevivePoint.position = new Vector3(
            Mathf.Clamp(playerReviveX, - m_MaxPlayerCamera, m_MaxPlayerCamera),
            m_PlayerManager.m_RevivePointY,
            Depth.PLAYER
        );
    }

    void OnEnable()
    {
        m_Invincibility = true;
        m_HasCollided = false;
        m_SlowMode = false;
        if (m_PlayerManager.PlayerControlable) { // 시작 이벤트가 아닐때만 방어막 켜기
            if (!m_SystemManager.m_InvincibleMod) {
                EnableInvincible(m_ReviveInvincibleTime);
            }
        }
    }

    private void ResetPosition() {
        transform.position = m_PlayerRevivePoint.position;
    }

    private void OverviewPosition() {
        Vector3 target_pos = new Vector3(0f, m_PlayerRevivePoint.position.y, Depth.PLAYER);
        if (m_SystemManager.m_PlayState != 3) {
            m_OverviewSpeed = Mathf.Max(Mathf.Abs(transform.position.x - target_pos.x), Mathf.Abs(transform.position.y - target_pos.y));
            return;
        }
        m_Vector2 = new Vector2(0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, target_pos, m_OverviewSpeed * Time.fixedDeltaTime);
    }

    private void UpdateInvincible() {
        if (m_InvincibleTimer > 0f) {
            m_InvincibleTimer -= Time.fixedDeltaTime;
        }
        else {
            DisableInvincible();
        }
    }

    public void EnableInvincible(float duration) {
        if (m_SystemManager.m_InvincibleMod)
            return;
        if (m_InvincibleTimer < duration) {
            m_PlayerShield.SetActive(true);
            m_Invincibility = true;
            m_InvincibleTimer = duration;
        }
    }

    public void DisableInvincible() {
        if (m_SystemManager.m_InvincibleMod)
            return;
        m_InvincibleTimer = 0f;
        m_PlayerShield.gameObject.SetActive(false);
        m_Invincibility = false;
    }
    
    
    void OnTriggerStay2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) { // 대상이 총알이면 대상과 자신 파괴
            if (!m_Invincibility) {
                if (!m_HasCollided) {
                    EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                    enemyBullet.OnDeath();
                }
                OnDeath();
            }
        }

        else if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            if (!m_Invincibility) {
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

                if ((1 << other.gameObject.layer & Layer.AIR) != 0) {
                    enemyObject.TakeDamage(m_Damage);
                    OnDeath();
                }
            }
        }
    }
    
    public override void OnDeath() { // Override
        if (!m_Invincibility) {
            if (!m_HasCollided) {
                m_HasCollided = true;
                GameObject obj = m_PoolingManager.PopFromPool(m_Explosion, PoolingParent.EXPLOSION); // 폭발 이펙트

                obj.transform.position = transform.position;
                obj.SetActive(true);
                
                m_PlayerManager.PlayerDead(transform.position);
                ResetPosition();
                gameObject.SetActive(false);
            }
        }
    }

    public int MoveRawHorizontal {
        get { return m_MoveRawHorizontal; }
        set { m_MoveRawHorizontal = value; }
    }

    public int MoveRawVertical {
        get { return m_MoveRawVertical; }
        set { m_MoveRawVertical = value; }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PlayerController : PlayerDamageUnit
{
    [SerializeField] private float m_Tilt = 30f;
    [SerializeField] private Boundary m_Boundary = new Boundary(-7f, 7f, -14.8f, -1f);
    [SerializeField] private Transform m_PlayerRevivePoint = null;
    [SerializeField] private GameObject m_PlayerShield = null;
    [SerializeField] private string m_Explosion = string.Empty;
    public float m_ReviveInvincibleTime = 3f;

    [HideInInspector] public bool m_SlowMode = false;

    private float m_MaxPlayerCamera;
    private float m_DefaultRotation;
    private float m_TiltSpeed = 0.2f;
    private bool m_Invincibility = false;
    private float m_InvincibleTimer = 0;
    private bool m_HasCollided = false;
    private float m_Speed, m_SlowSpeed;
    private float m_OverviewSpeed;
    
    private SystemManager m_SystemManager = null;


    void Start() {
        m_MaxPlayerCamera = Size.CAMERA_MOVE_LIMIT;
        m_DefaultRotation = transform.eulerAngles[0];

        m_SystemManager = SystemManager.instance_sm;

        switch(m_PlayerManager.m_CurrentAttributes[1]) {
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

    void Update ()
    {
        float moveRawHorizontal = 0f;
        float moveRawVertical = 0f;

        if (m_PlayerManager.PlayerControlable) {
            moveRawHorizontal = Input.GetAxisRaw("Horizontal");
            moveRawVertical = Input.GetAxisRaw ("Vertical");

            Vector2 movement = new Vector2 (moveRawHorizontal, moveRawVertical);

            if (m_SlowMode)
                m_Vector2 = movement * m_SlowSpeed;
            else
                m_Vector2 = movement * m_Speed;
        }
        Turn(moveRawHorizontal);
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
    }

    void LateUpdate() {
        float playerReviveX = transform.position.x;
        m_PlayerRevivePoint.position = new Vector3(
            Mathf.Clamp(playerReviveX, - m_MaxPlayerCamera, m_MaxPlayerCamera),
            m_PlayerManager.m_revivePointY,
            transform.position.z
        );
        m_HasCollided = false;
    }

    void OnEnable() {
        m_Invincibility = true;
        m_HasCollided = false;
        m_SlowMode = false;
        if (m_PlayerManager.PlayerControlable) { // 시작 이벤트가 아닐때만 방어막 켜기
            if (!m_SystemManager.m_DebugMod) {
                EnableInvincible(m_ReviveInvincibleTime);
            }
        }
    }

    private void Turn(float turnState)
    {
        Quaternion maxTilt = Quaternion.AngleAxis( - m_Tilt*turnState, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, maxTilt, m_TiltSpeed*Time.deltaTime*60);
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
        transform.position = Vector3.MoveTowards(transform.position, target_pos, m_OverviewSpeed * Time.deltaTime);
    }

    private void UpdateInvincible() {
        if (m_InvincibleTimer > 0f) {
            m_InvincibleTimer -= Time.deltaTime;
        }
        else {
            DisableInvincible();
        }
    }

    public void EnableInvincible(float duration) {
        if (m_SystemManager.m_DebugMod)
            return;
        if (m_InvincibleTimer < duration) {
            m_PlayerShield.SetActive(true);
            m_Invincibility = true;
            m_InvincibleTimer = duration;
        }
    }

    public void DisableInvincible() {
        if (m_SystemManager.m_DebugMod)
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
}
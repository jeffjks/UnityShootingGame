using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Boundary
{
    public int xMin, xMax, yMin, yMax;

    public Boundary(int xMin, int xMax, int yMin, int yMax) {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }
}

public class PlayerController : PlayerControllerManager
{
    [SerializeField] private float m_Tilt = 30f;
    [SerializeField] private GameObject m_PlayerShield = null;
    [SerializeField] private string m_Explosion = string.Empty;

    public int m_ReviveInvincibleTime;

    private Boundary m_Boundary = new Boundary(-1792, 1792, -3789, -256); // -7f, 7f, -14.8f, -1f
    private int m_MaxPlayerCamera;
    private float m_DefaultRotation;
    private float m_TiltSpeed = 0.2f;
    private bool m_Invincibility = false;
    private int m_InvincibleTimer;
    private bool m_HasCollided = false;
    private int m_Speed, m_SlowSpeed, m_OverviewSpeed;
    private int m_MoveRawHorizontal = 0, m_MoveRawVertical = 0;
    
    private SystemManager m_SystemManager = null;

    void Start()
    {
        m_MaxPlayerCamera = (int) (Size.CAMERA_MOVE_LIMIT * 256);
        m_DefaultRotation = transform.eulerAngles[0];

        m_SystemManager = SystemManager.instance_sm;

        switch(m_PlayerManager.m_CurrentAttributes.m_Speed) {
            case 0:
                m_Speed = 26; // 6f * 256;
                m_SlowSpeed = 17; // 4f * 256;
                break;
            case 1:
                m_Speed = 29; // 6.75f * 256;
                m_SlowSpeed = 18; // 4.2f * 256;
                break;
            case 2:
                m_Speed = 32; // 7.5f; * 256 / 60
                m_SlowSpeed = 19; // 4.4f * 256;
                break;
            default:
                break;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Tilt(float tilt_state) {
        Quaternion maxTilt = Quaternion.AngleAxis(- m_Tilt*tilt_state, Vector3.forward);
        m_PlayerBody.localRotation = Quaternion.Lerp(m_PlayerBody.localRotation, maxTilt, m_TiltSpeed*Time.deltaTime * 60f);
    }

    void Update()
    {
        if (m_PlayerManager.m_PlayerControlable) {
            if (!m_SystemManager.m_ReplayState) {
                m_MoveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
                m_MoveRawVertical = (int) Input.GetAxisRaw ("Vertical");
            }
        }
        Tilt(m_MoveRawHorizontal);

        if (Time.timeScale == 0)
            return;
        
        Vector2Int movement = new Vector2Int(m_MoveRawHorizontal, m_MoveRawVertical);
        if (m_PlayerManager.m_PlayerControlable) {
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

        if (m_PlayerManager.m_PlayerControlable) {
            m_Position = new Vector2Int
            (
                Mathf.Clamp(m_Position.x, m_Boundary.xMin, m_Boundary.xMax), 
                Mathf.Clamp(m_Position.y, m_Boundary.yMin, m_Boundary.yMax)
            );
        }
        SetPosition();

        UpdateInvincible();

        //Debug.Log(m_Position);
    }

    void OnEnable()
    {
        SetPosition();

        m_Invincibility = true;
        m_HasCollided = false;
        m_SlowMode = false;
        if (m_PlayerManager.m_PlayerControlable) { // 시작 이벤트가 아닐때만 방어막 켜기
            if (!m_SystemManager.m_InvincibleMod) {
                EnableInvincible(m_ReviveInvincibleTime);
            }
        }
    }

    private void ResetPosition() {
        int playerReviveX = Mathf.Clamp(m_Position.x, -m_MaxPlayerCamera, m_MaxPlayerCamera);
        int playerReviveY = m_PlayerManager.m_RevivePositionY;

        m_Position = new Vector2Int(playerReviveX, playerReviveY);
        SetPosition();
    }

    private void OverviewPosition() {
        Vector2Int target_pos;
        if (m_SystemManager.GetCurrentStage() < 4) {
            target_pos = new Vector2Int(0, m_PlayerManager.m_RevivePositionY);
            if (m_SystemManager.m_PlayState != 3) {
                m_OverviewSpeed = Mathf.Max(Mathf.Abs(m_Position.x - target_pos.x), Mathf.Abs(m_Position.y - target_pos.y));
                m_OverviewSpeed = Mathf.Min(m_OverviewSpeed, 820);
                return;
            }
        }
        else {
            target_pos = new Vector2Int(m_Position.x, 2*256);
            if (m_SystemManager.m_PlayState != 3) {
                m_OverviewSpeed = 12*256;
                return;
            }
        }
        // m_PlayState가 3일때만 이하 내용 실행
        m_Vector2 = Vector2Int.zero;
        m_Position = Vector2Int.RoundToInt(Vector2.MoveTowards(m_Position, target_pos, m_OverviewSpeed / Application.targetFrameRate * Time.timeScale));
    }

    private void UpdateInvincible() {
        if (m_InvincibleTimer > 0) {
            m_InvincibleTimer--;
        }
        else {
            DisableInvincible();
        }
    }

    public void EnableInvincible(int millisecond) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (m_SystemManager.m_InvincibleMod)
            return;
        if (m_InvincibleTimer < frame) {
            m_PlayerShield.SetActive(true);
            m_Invincibility = true;
            m_InvincibleTimer = frame;
        }
    }

    public void DisableInvincible() {
        if (m_SystemManager.m_InvincibleMod)
            return;
        m_InvincibleTimer = 0;
        m_PlayerShield.gameObject.SetActive(false);
        m_Invincibility = false;
    }
    
    
    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) { // 대상이 총알이면 대상과 자신 파괴
            if (!m_Invincibility) {
                if (!m_HasCollided) {
                    try {
                        EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                        enemyBullet.OnDeath();
                    }
                    catch {
                        return;
                    }
                }
                OnDeath();
            }
        }

        else if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            if (!m_Invincibility) {
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

                if ((1 << other.gameObject.layer & Layer.AIR) != 0) {
                    DealDamage(enemyObject, m_Damage);
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

                obj.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
                obj.SetActive(true);
                
                m_PlayerManager.PlayerDead(m_Position);
                ResetPosition();
                gameObject.SetActive(false);
            }
        }
    }

    public void SetVerticalSpeed(int vspeed) {
        m_Vector2 = new Vector2Int(m_Vector2.x, vspeed);
    }

    public bool GetInvincibility() {
        return m_Invincibility;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerController : PlayerUnit
{
    [SerializeField] private GameObject m_PlayerShield = null;
    [SerializeField] private string m_Explosion = string.Empty;

    public int m_ReviveInvincibleTime;

    private int m_MaxPlayerCamera;
    private float m_DefaultRotation;
    private int m_InvincibleTimer;
    private bool m_HasCollided = false;
    private int m_SpeedIntDefault, m_SpeedIntSlow, m_OverviewSpeed;
    private int m_MoveRawHorizontal = 0, m_MoveRawVertical = 0;
    private bool m_Invincibility;

    private const int BOUNDARY_PLAYER_X_MIN = -1792; // -7f
    private const int BOUNDARY_PLAYER_X_MAX = 1792; // 7f
    private const int BOUNDARY_PLAYER_Y_MIN = -3789; // -14.8f
    private const int BOUNDARY_PLAYER_Y_MAX = -256; // -1f
    
    private SystemManager m_SystemManager = null;

    protected override void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        base.Awake();
    }

    void Start()
    {
        m_MaxPlayerCamera = (int) (Size.CAMERA_MOVE_LIMIT * 256);
        m_DefaultRotation = transform.eulerAngles[0];
        m_CurrentAngle = 180f;
        m_MoveVector.direction = 180f;
        transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward); // Vector3.forward

        switch(m_PlayerManager.m_CurrentAttributes.m_Speed) {
            case 0:
                m_SpeedIntDefault = 26; // 6f * 256;
                m_SpeedIntSlow = 17; // 4f * 256;
                break;
            case 1:
                m_SpeedIntDefault = 29; // 6.75f * 256;
                m_SpeedIntSlow = 18; // 4.2f * 256;
                break;
            case 2:
                m_SpeedIntDefault = 32; // 7.5f; * 256 / 60
                m_SpeedIntSlow = 19; // 4.4f * 256;
                break;
            default:
                break;
        }
        
        m_PositionInt2D = Vector2Int.RoundToInt(new Vector2(transform.position.x * 256, transform.position.y * 256));
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (m_PlayerManager.m_PlayerControlable) {
            if (m_SystemManager.m_GameType != GameType.GAMETYPE_REPLAY) {
                m_MoveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
                m_MoveRawVertical = (int) Input.GetAxisRaw ("Vertical");
            }
        }

        if (Time.timeScale == 0)
            return;
        
        Vector2Int movement_vector = new Vector2Int(m_MoveRawHorizontal, m_MoveRawVertical);
        if (m_PlayerManager.m_PlayerControlable) {
            m_MoveVector = new MoveVector(movement_vector);
            if (m_SlowMode) {
                m_MoveVector.speed = m_SpeedIntSlow;
                movement_vector *= m_SpeedIntSlow;
            }
            else {
                m_MoveVector.speed = m_SpeedIntDefault;
                movement_vector *= m_SpeedIntDefault;
            }

            m_PositionInt2D = m_PositionInt2D + movement_vector;
        }
        else {
            m_MoveRawHorizontal = 0;
            m_MoveRawVertical = 0;
        }

        OverviewPosition();

        if (m_PlayerManager.m_PlayerControlable) {
            m_PositionInt2D = new Vector2Int
            (
                Mathf.Clamp(m_PositionInt2D.x, BOUNDARY_PLAYER_X_MIN, BOUNDARY_PLAYER_X_MAX), 
                Mathf.Clamp(m_PositionInt2D.y, BOUNDARY_PLAYER_Y_MIN, BOUNDARY_PLAYER_Y_MAX)
            );
        }

        UpdateInvincible();
    }

    void OnEnable()
    {
        m_Invincibility = true;
        m_HasCollided = false;
        m_SlowMode = false;
        if (m_PlayerManager.m_PlayerControlable) { // 시작 이벤트가 아닐때만 방어막 켜기
            if (!m_SystemManager.GetInvincibleMod()) {
                DisableInvincibility(m_ReviveInvincibleTime);
            }
        }
    }

    private void ResetPosition() {
        int playerReviveX = Mathf.Clamp(m_PositionInt2D.x, -m_MaxPlayerCamera, m_MaxPlayerCamera);
        int playerReviveY = m_PlayerManager.m_RevivePositionY;

        m_PositionInt2D = new Vector2Int(playerReviveX, playerReviveY);
    }

    private void OverviewPosition() {
        Vector2Int target_pos;
        if (m_SystemManager.GetCurrentStage() < 4) {
            target_pos = new Vector2Int(0, m_PlayerManager.m_RevivePositionY);
            if (m_SystemManager.m_PlayState != 3) {
                m_OverviewSpeed = Mathf.Max(Mathf.Abs(m_PositionInt2D.x - target_pos.x), Mathf.Abs(m_PositionInt2D.y - target_pos.y));
                m_OverviewSpeed = Mathf.Min(m_OverviewSpeed, 820);
                return;
            }
        }
        else {
            target_pos = new Vector2Int(m_PositionInt2D.x, 2*256);
            if (m_SystemManager.m_PlayState != 3) {
                m_OverviewSpeed = 12*256;
                return;
            }
        }
        // m_PlayState가 3일때만 이하 내용 실행
        //m_Vector2 = Vector2Int.zero;
        m_PositionInt2D = Vector2Int.RoundToInt(Vector2.MoveTowards(m_PositionInt2D, target_pos, m_OverviewSpeed / Application.targetFrameRate * Time.timeScale));
    }

    private void UpdateInvincible() {
        if (m_InvincibleTimer > 0) {
            m_InvincibleTimer--;
        }
        else {
            DisableInvincible();
        }
    }

    public void DisableInvincibility(int millisecond) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (m_SystemManager.GetInvincibleMod())
            return;
        if (m_InvincibleTimer < frame) {
            m_PlayerShield.SetActive(true);
            m_Invincibility = true;
            m_InvincibleTimer = frame;
        }
    }

    public void DisableInvincible() {
        if (m_SystemManager.GetInvincibleMod())
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
                        enemyBullet.PlayEraseAnimation();
                    }
                    catch {
                        return;
                    }
                }
                OnPlayerDeath();
            }
        }

        else if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            if (!m_Invincibility) {
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

                if ((1 << other.gameObject.layer & Layer.AIR) != 0) {
                    DealDamage(enemyObject, m_Damage);
                    OnPlayerDeath();
                }
            }
        }
    }
    
    private void OnPlayerDeath() { // Override
        if (!m_Invincibility) {
            if (!m_HasCollided) {
                m_HasCollided = true;
                GameObject obj = m_PoolingManager.PopFromPool(m_Explosion, PoolingParent.EXPLOSION); // 폭발 이펙트
                ExplosionEffect explosionEffect = obj.GetComponent<ExplosionEffect>();

                obj.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
                obj.SetActive(true);
                explosionEffect.OnStart();
                
                m_PlayerManager.PlayerDead(m_PositionInt2D);
                ResetPosition();
                gameObject.SetActive(false);
            }
        }
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
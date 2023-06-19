using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerController : PlayerUnit
{
    public static bool IsControllable { get; set; }
    
    [SerializeField] private string m_Explosion = string.Empty;

    private const int MAX_PLAYER_CAMERA = (int) (Size.CAMERA_MOVE_LIMIT * 256);
    private float m_DefaultRotation;
    private bool m_HasCollided = false; // A
    private int m_SpeedIntDefault, m_SpeedIntSlow, m_OverviewSpeed;
    private int m_MoveRawHorizontal = 0, m_MoveRawVertical = 0;

    private const int BOUNDARY_PLAYER_X_MIN = -1792; // -7f
    private const int BOUNDARY_PLAYER_X_MAX = 1792; // 7f
    private const int BOUNDARY_PLAYER_Y_MIN = -3789; // -14.8f
    private const int BOUNDARY_PLAYER_Y_MAX = -256; // -1f

    protected override void Awake()
    {
        base.Awake();

        SystemManager.Action_OnBossClear += OnBossClear;
        SystemManager.Action_OnStageClear += OnStageClear;
        SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnQuitInGame += OnRemove;
    }

    void Start()
    {
        m_DefaultRotation = transform.eulerAngles[0];
        m_CurrentAngle = 180f;
        m_MoveVector.direction = 180f;
        transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward); // Vector3.forward

        switch(PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed)) {
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
        
        DontDestroyOnLoad(transform.parent);
    }

    void Update()
    {
        if (IsControllable) {
            if (SystemManager.GameMode != GameMode.Replay) {
                m_MoveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
                m_MoveRawVertical = (int) Input.GetAxisRaw ("Vertical");
            }
        }

        if (Time.timeScale == 0)
            return;
        
        Vector2Int movement_vector = new Vector2Int(m_MoveRawHorizontal, m_MoveRawVertical);
        if (IsControllable) {
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

        if (IsControllable) {
            m_PositionInt2D = new Vector2Int
            (
                Mathf.Clamp(m_PositionInt2D.x, BOUNDARY_PLAYER_X_MIN, BOUNDARY_PLAYER_X_MAX), 
                Mathf.Clamp(m_PositionInt2D.y, BOUNDARY_PLAYER_Y_MIN, BOUNDARY_PLAYER_Y_MAX)
            );
        }
    }

    void OnEnable()
    {
        m_HasCollided = false;
        m_SlowMode = false;
    }

    private void ResetPositionIntAfterDeath() {
        int playerReviveX = Mathf.Clamp(m_PositionInt2D.x, -MAX_PLAYER_CAMERA, MAX_PLAYER_CAMERA);
        int playerReviveY = (int) PlayerManager.REVIVE_POSITION_Y * 256;

        m_PositionInt2D = new Vector2Int(playerReviveX, playerReviveY);
    }

    private void OverviewPosition() {
        Vector2Int target_pos;
        if (SystemManager.Stage < 4) {
            target_pos = new Vector2Int(0, (int) PlayerManager.REVIVE_POSITION_Y * 256);
            if (SystemManager.PlayState != PlayState.OnStageResult) {
                m_OverviewSpeed = Mathf.Max(Mathf.Abs(m_PositionInt2D.x - target_pos.x), Mathf.Abs(m_PositionInt2D.y - target_pos.y));
                m_OverviewSpeed = Mathf.Min(m_OverviewSpeed, 820);
                return;
            }
        }
        else {
            target_pos = new Vector2Int(m_PositionInt2D.x, 2*256);
            if (SystemManager.PlayState != PlayState.OnStageResult) {
                m_OverviewSpeed = 12*256;
                return;
            }
        }
        // m_PlayState가 3일때만 이하 내용 실행
        //m_Vector2 = Vector2Int.zero;
        m_PositionInt2D = Vector2Int.RoundToInt(Vector2.MoveTowards(m_PositionInt2D, target_pos, m_OverviewSpeed / Application.targetFrameRate * Time.timeScale));
    }
    
    
    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) { // 대상이 총알이면 대상과 자신 파괴
            if (!PlayerInvincibility.IsInvincible) {
                if (!m_HasCollided) {
                    try {
                        EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                        enemyBullet.PlayEraseAnimation();
                    }
                    catch {
                        return;
                    }
                }
                OnDeath();
            }
        }

        else if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            if (!PlayerInvincibility.IsInvincible) {
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

                if (Utility.CheckLayer(other.gameObject, Layer.AIR)) {
                    DealDamage(enemyObject, m_Damage);
                    OnDeath();
                }
            }
        }
    }
    
    private void OnDeath() {
        if (!PlayerInvincibility.IsInvincible) {
            if (!m_HasCollided) {
                m_HasCollided = true;
                GameObject obj = PoolingManager.PopFromPool(m_Explosion, PoolingParent.Explosion); // 폭발 이펙트

                obj.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
                obj.SetActive(true);
                
                PlayerManager.Instance.PlayerDead(m_PositionInt2D);
                ResetPositionIntAfterDeath();
                transform.root.gameObject.SetActive(false);
            }
        }
    }

    private void OnRemove()
    {
        Destroy(transform.root);
    }

    private void OnBossClear()
    {
        PlayerInvincibility.SetInvincibility(5000);
    }

    private void OnStageClear()
    {
        IsControllable = false;
    }

    private void OnNextStage(bool hasNextStage)
    {
        if (!hasNextStage)
        {
            OnRemove();
            return;
        }
        m_PositionInt2D = new Vector2Int(0, (int)PlayerManager.REVIVE_POSITION_Y * 256);
        IsControllable = true;
        PlayerInvincibility.SetInvincibility(3000);
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
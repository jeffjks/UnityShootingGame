using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerMovement : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    
    private const int MAX_PLAYER_CAMERA = (int) (Size.CAMERA_MOVE_LIMIT * 256);
    private float m_DefaultRotation;
    private int m_SpeedIntDefault, m_SpeedIntSlow, m_OverviewSpeed;
    private int _moveRawHorizontal, _moveRawVertical;
    private PlayerCollisionDetector _playerCollisionDetector;

    private const int BOUNDARY_PLAYER_X_MIN = -1792; // -7f
    private const int BOUNDARY_PLAYER_X_MAX = 1792; // 7f
    private const int BOUNDARY_PLAYER_Y_MIN = -3789; // -14.8f
    private const int BOUNDARY_PLAYER_Y_MAX = -256; // -1f

    private Vector2Int _positionInt2D;
    public Vector2Int PositionInt2D
    {
        get => _positionInt2D;
        set {
            _positionInt2D = value;
            transform.position = new Vector3((float) _positionInt2D.x / 256, (float) _positionInt2D.y / 256, Depth.PLAYER);
        }
    }
    public int MoveRawHorizontal { get; set; }
    public int MoveRawVertical { get; set; }

    private void Awake()
    {
        _playerCollisionDetector = GetComponent<PlayerCollisionDetector>();

        SystemManager.Action_OnBossClear += OnBossClear;
        SystemManager.Action_OnStageClear += OnStageClear;
        SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnQuitInGame += OnRemove;
        
        _playerCollisionDetector.Action_OnCollideWithEnemy += m_PlayerUnit.DealCollisionDamage;
        _playerCollisionDetector.Action_OnDeath += KillPlayer;
        _playerCollisionDetector.Action_OnDeath += ResetPosition;
    }

    void Start()
    {
        m_DefaultRotation = transform.eulerAngles[0];
        m_PlayerUnit.CurrentAngle = 180f;
        m_PlayerUnit.m_MoveVector.direction = 180f;
        transform.rotation = Quaternion.AngleAxis(m_PlayerUnit.CurrentAngle, Vector3.forward); // Vector3.forward

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
        }
        
        PositionInt2D = Vector2Int.RoundToInt(new Vector2(transform.position.x * 256, transform.position.y * 256));
    }

    void Update()
    {
        if (PlayerUnit.IsControllable) {
            if (SystemManager.GameMode != GameMode.Replay) {
                _moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
                _moveRawVertical = (int) Input.GetAxisRaw ("Vertical");
            }
        }

        if (Time.timeScale == 0)
            return;
        
        Vector2Int movement_vector = new Vector2Int(_moveRawHorizontal, _moveRawVertical);
        if (PlayerUnit.IsControllable) {
            m_PlayerUnit.m_MoveVector = new MoveVector(movement_vector);
            if (m_PlayerUnit.SlowMode) {
                m_PlayerUnit.m_MoveVector.speed = m_SpeedIntSlow;
                movement_vector *= m_SpeedIntSlow;
            }
            else {
                m_PlayerUnit.m_MoveVector.speed = m_SpeedIntDefault;
                movement_vector *= m_SpeedIntDefault;
            }

            PositionInt2D = PositionInt2D + movement_vector;
        }
        else {
            _moveRawHorizontal = 0;
            _moveRawVertical = 0;
        }

        OverviewPosition();

        if (PlayerUnit.IsControllable) {
            PositionInt2D = new Vector2Int
            (
                Mathf.Clamp(PositionInt2D.x, BOUNDARY_PLAYER_X_MIN, BOUNDARY_PLAYER_X_MAX), 
                Mathf.Clamp(PositionInt2D.y, BOUNDARY_PLAYER_Y_MIN, BOUNDARY_PLAYER_Y_MAX)
            );
        }
    }

    void OnEnable()
    {
        m_PlayerUnit.SlowMode = false;
    }

    private void KillPlayer()
    {
        PlayerManager.Instance.PlayerDead(PositionInt2D);
    }

    private void ResetPosition()
    {
        int playerReviveX = Mathf.Clamp(PositionInt2D.x, -MAX_PLAYER_CAMERA, MAX_PLAYER_CAMERA);
        int playerReviveY = (int) PlayerManager.REVIVE_POSITION_Y * 256;

        PositionInt2D = new Vector2Int(playerReviveX, playerReviveY);
    }

    private void OverviewPosition() {
        Vector2Int target_pos;
        if (SystemManager.Stage < 4) {
            target_pos = new Vector2Int(0, (int) PlayerManager.REVIVE_POSITION_Y * 256);
            if (SystemManager.PlayState != PlayState.OnStageResult) {
                m_OverviewSpeed = Mathf.Max(Mathf.Abs(PositionInt2D.x - target_pos.x), Mathf.Abs(PositionInt2D.y - target_pos.y));
                m_OverviewSpeed = Mathf.Min(m_OverviewSpeed, 820);
                return;
            }
        }
        else {
            target_pos = new Vector2Int(PositionInt2D.x, 2*256);
            if (SystemManager.PlayState != PlayState.OnStageResult) {
                m_OverviewSpeed = 12*256;
                return;
            }
        }
        // m_PlayState가 3일때만 이하 내용 실행
        //m_Vector2 = Vector2Int.zero;
        PositionInt2D = Vector2Int.RoundToInt(Vector2.MoveTowards(PositionInt2D, target_pos, m_OverviewSpeed / Application.targetFrameRate * Time.timeScale));
    }

    private void OnRemove()
    {
        Destroy(transform.root.gameObject);
    }

    private void OnBossClear()
    {
        PlayerInvincibility.SetInvincibility(5000);
    }

    private void OnStageClear()
    {
        PlayerUnit.IsControllable = false;
    }

    private void OnNextStage(bool hasNextStage)
    {
        if (!hasNextStage)
        {
            OnRemove();
            return;
        }
        PositionInt2D = new Vector2Int(0, (int)PlayerManager.REVIVE_POSITION_Y * 256);
        PlayerUnit.IsControllable = true;
        PlayerInvincibility.SetInvincibility(3000);
    }
}
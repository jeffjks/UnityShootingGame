using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerMovement : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerSpeedDatas m_PlayerSpeedData;
    
    private const int MAX_PLAYER_CAMERA = (int) (Size.CAMERA_MOVE_LIMIT * 256);
    private int m_OverviewSpeed;
    private int _defaultSpeed;
    private int _slowSpeed;
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
    private int MoveRawHorizontal { get; set; }
    private int MoveRawVertical { get; set; }

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
    
    private void OnDestroy()
    {
        SystemManager.Action_OnBossClear -= OnBossClear;
        SystemManager.Action_OnStageClear -= OnStageClear;
        SystemManager.Action_OnNextStage -= OnNextStage;
        SystemManager.Action_OnQuitInGame -= OnRemove;
    }

    void Start()
    {
        m_PlayerUnit.m_MoveVector.direction = 180f;
        transform.rotation = Quaternion.AngleAxis(m_PlayerUnit.CurrentAngle, Vector3.forward); // Vector3.forward

        var index = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed);
        _defaultSpeed = m_PlayerSpeedData.playerSpeed[index].defaultSpeed;
        _slowSpeed = m_PlayerSpeedData.playerSpeed[index].slowSpeed;
        
        PositionInt2D = Vector2Int.RoundToInt(new Vector2(transform.position.x * 256, transform.position.y * 256));
    }

    public void MovePlayer(Vector2 inputVector)
    {
        MoveRawHorizontal = System.Math.Sign(inputVector.x);
        MoveRawVertical = System.Math.Sign(inputVector.y);

        OverviewPosition();
    }

    private void Update()
    {
        ReplayManager.Instance.WriteUserMoveInput(MoveRawHorizontal, MoveRawVertical);
        
        Vector2 moveVector = new Vector2(MoveRawHorizontal, MoveRawVertical);
        moveVector.Normalize();
        
        if (m_PlayerUnit.SlowMode) {
            moveVector *= _slowSpeed;
        }
        else {
            moveVector *= _defaultSpeed;
        }
        
        if (PlayerUnit.IsControllable) {
            var moveVectorInt = Vector2Int.RoundToInt(moveVector);
            PositionInt2D += moveVectorInt;
            PositionInt2D = new Vector2Int
            (
                Mathf.Clamp(PositionInt2D.x, BOUNDARY_PLAYER_X_MIN, BOUNDARY_PLAYER_X_MAX), 
                Mathf.Clamp(PositionInt2D.y, BOUNDARY_PLAYER_Y_MIN, BOUNDARY_PLAYER_Y_MAX)
            );
        }
    }

    private void OnEnable()
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

    private void OverviewPosition()
    {
        if (Time.timeScale == 0)
            return;
        
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
        var maxDistanceDelta = m_OverviewSpeed / (Application.targetFrameRate * Time.timeScale);
        PositionInt2D = Vector2Int.RoundToInt(Vector2.MoveTowards(PositionInt2D, target_pos, maxDistanceDelta));
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
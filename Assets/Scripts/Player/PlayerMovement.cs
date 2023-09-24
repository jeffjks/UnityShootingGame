using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerMovement : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerSpeedDatas m_PlayerSpeedData;
    public AircraftRollingService m_AircraftRollingService;
    
    private float _overviewSpeed;
    private float _defaultSpeed;
    private float _slowSpeed;

    private const float BOUNDARY_PLAYER_X_MIN = -7f; // -7f
    private const float BOUNDARY_PLAYER_X_MAX = 7f; // 7f
    private const float BOUNDARY_PLAYER_Y_MIN = -14.8f; // -14.8f
    private const float BOUNDARY_PLAYER_Y_MAX = -1f; // -1f
    
    private int MoveRawHorizontal { get; set; }
    private int MoveRawVertical { get; set; }

    private void Awake()
    {
        SystemManager.Action_OnBossClear += OnBossClear;
        SystemManager.Action_OnStageClear += OnStageClear;
        SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnQuitInGame += OnRemove;
        PlayerManager.Action_OnPlayerRevive += Init;
    }

    private void Start()
    {
        Init();
        transform.rotation = Quaternion.AngleAxis(m_PlayerUnit.CurrentAngle, Vector3.forward); // Vector3.forward

        var index = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed);
        _defaultSpeed = m_PlayerSpeedData.playerSpeed[index].defaultSpeed / 256f;
        _slowSpeed = m_PlayerSpeedData.playerSpeed[index].slowSpeed / 256f;
    }
    
    private void OnDestroy()
    {
        SystemManager.Action_OnBossClear -= OnBossClear;
        SystemManager.Action_OnStageClear -= OnStageClear;
        SystemManager.Action_OnNextStage -= OnNextStage;
        SystemManager.Action_OnQuitInGame -= OnRemove;
        PlayerManager.Action_OnPlayerRevive -= Init;
    }

    public void HandlePlayerMovement(Vector2Int inputVector)
    {
        MoveRawHorizontal = inputVector.x;
        MoveRawVertical = inputVector.y;
    }

    public void ExecuteMovement()
    {
        var moveVector = new Vector2(MoveRawHorizontal, MoveRawVertical);
        moveVector.Normalize();
        
        if (m_PlayerUnit.SlowMode) {
            moveVector *= _slowSpeed;
        }
        else {
            moveVector *= _defaultSpeed;
        }
            
        transform.position += (Vector3) moveVector;
        m_PlayerUnit.m_MoveVector.direction = 180f + 90f*MoveRawHorizontal;
            
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, BOUNDARY_PLAYER_X_MIN, BOUNDARY_PLAYER_X_MAX), 
            Mathf.Clamp(transform.position.y, BOUNDARY_PLAYER_Y_MIN, BOUNDARY_PLAYER_Y_MAX),
            Depth.PLAYER
        );
    }

    private void Init()
    {
        m_PlayerUnit.SlowMode = false;
        m_PlayerUnit.m_MoveVector.direction = 180f;
        m_AircraftRollingService.CurrentRollDegree = 0f;
    }

    private void OverviewPosition()
    {
        if (Time.timeScale == 0)
            return;
        
        Vector3 targetPos;
        if (SystemManager.Stage < 4)
        {
            targetPos = new Vector3(0f, PlayerManager.REVIVE_POSITION_Y, Depth.PLAYER);
            if (SystemManager.PlayState != PlayState.OnStageResult)
            {
                _overviewSpeed = Mathf.Max(Mathf.Abs(transform.position.x - targetPos.x), Mathf.Abs(transform.position.y - targetPos.y));
                _overviewSpeed = Mathf.Min(_overviewSpeed, 820f);
                _overviewSpeed /= 256f;
                return;
            }
        }
        else {
            targetPos = new Vector3(transform.position.x, 2f, Depth.PLAYER);
            if (SystemManager.PlayState != PlayState.OnStageResult)
            {
                _overviewSpeed = 12f;
                return;
            }
        }
        // m_PlayState가 3일때만 이하 내용 실행
        //m_Vector2 = Vector2Int.zero;
        var maxDistanceDelta = _overviewSpeed / (Application.targetFrameRate * Time.timeScale);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, maxDistanceDelta);
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
        transform.position = new Vector3(0f, PlayerManager.REVIVE_POSITION_Y, Depth.PLAYER);
        PlayerUnit.IsControllable = true;
        PlayerInvincibility.SetInvincibility(3000);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-101)]
public class PlayerController : MonoBehaviour
{
    public PlayerBombHandler m_PlayerBomb;
    
    private PlayerBombHandler _playerBombHandler;
    private PlayerLaserHandler _playerLaserHandler;
    private PlayerShotHandler _playerShotHandler;
    private PlayerMovement _playerMovement;
    private PlayerUnit _playerUnit;

    private bool IsFirePressed { get; set; }
    public bool IsBombPressed { get; private set; }
    private int _firePressFrame;
    private bool IsInputInvokable => ReplayManager.IsReplayAvailable && PlayerUnit.IsControllable;

    private Vector2Int _rawInputVector;
    
    private void Awake()
    {
        _playerBombHandler = Instantiate(m_PlayerBomb);
        _playerLaserHandler = GetComponentInChildren<PlayerLaserHandler>();
        _playerShotHandler = GetComponent<PlayerShotHandler>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerUnit = GetComponent<PlayerUnit>();
        
        if (!_playerUnit.m_IsPreviewObject)
        {
            InGameInputController.Action_OnFireInput += OnFireInvoked;
            InGameInputController.Action_OnBombInput += OnBombInvoked;
            InGameInputController.Action_OnMove += OnMoveInvoked;
            _playerUnit.Action_OnControllableChanged += OnControllableChanged;
            
            SystemManager.Action_OnBossClear += StopAttack;
        }
    }

    private void OnDestroy()
    {
        if (!_playerUnit.m_IsPreviewObject)
        {
            InGameInputController.Action_OnFireInput -= OnFireInvoked;
            InGameInputController.Action_OnBombInput -= OnBombInvoked;
            InGameInputController.Action_OnMove -= OnMoveInvoked;
            _playerUnit.Action_OnControllableChanged -= OnControllableChanged;
            
            SystemManager.Action_OnBossClear -= StopAttack;
        }
    }

    private void OnControllableChanged(bool controllable)
    {
        OnMoveInvoked(controllable ? _rawInputVector : Vector2Int.zero);
    }
    
    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (!IsInputInvokable)
            return;
        
        if (SystemManager.GameMode == GameMode.Replay)
            ReplayManager.ReadUserInput();
        else
            ReplayManager.WriteReplayMovementData();

        if (IsFirePressed)
        {
            _firePressFrame++;
        }
        
        _playerMovement.ExecuteMovement();
        //ExecuteShot();
        ExecuteLaser();
    }

    public void OnMoveInvoked(Vector2Int inputVector)
    {
        _rawInputVector = inputVector;
        
        if (!IsInputInvokable)
            return;
        
        _playerMovement.HandlePlayerMovement(inputVector);
        _playerShotHandler.ReceiveHorizontalMovement(inputVector.x);
        
        ReplayManager.WriteUserMovementInput(inputVector);
    }

    public void OnFireInvoked(bool isPressed)
    {
        if (!IsInputInvokable)
            return;
        
        HandleFireInput(isPressed);
        
        ReplayManager.WriteUserActionInput(ReplayManager.KeyType.Fire, isPressed);
    }

    private void HandleFireInput(bool isPressed)
    {
        if (SystemManager.PlayState is not (PlayState.None or PlayState.OnBoss or PlayState.OnMiddleBoss))
        {
            IsFirePressed = false;
            return;
        }
        
        IsFirePressed = isPressed;
        
        if (!PlayerUnit.IsControllable)
            return;

        if (IsFirePressed) // 누르는 순간
        {
            if (!_playerUnit.SlowMode) { // 샷 모드일 경우 AutoShot 증가
                if (_playerShotHandler.AutoShot < 2) {
                    _playerShotHandler.AutoShot++;
                }
            }
        }
        else // 떼는 순간
        {
            _firePressFrame = 0;
            _playerUnit.SlowMode = false;
            _playerLaserHandler.StopLaser();
            _playerUnit.IsAttacking = false;
        }
    }

    private void ExecuteLaser()
    {
        if (!_playerUnit.SlowMode) {
            if (_firePressFrame > Application.targetFrameRate / 2) { // 0.5초간 누르면 레이저 모드
                _playerUnit.SlowMode = true;
                _playerLaserHandler.StartLaser();
                _playerUnit.IsAttacking = true;
                _playerShotHandler.AutoShot = 0;
            }
        }
    }

    public void OnBombInvoked(bool isPressed)
    {
        if (!IsInputInvokable)
            return;
        
        ExecuteBomb(isPressed);
        
        ReplayManager.WriteUserActionInput(ReplayManager.KeyType.Bomb, isPressed);
    }
    
    private void ExecuteBomb(bool isPressed)
    {
        if (InGameDataManager.Instance.BombNumber <= 0)
            return;
        if (PlayerBombHandler.IsBombInUse)
            return;
        if (SystemManager.PlayState is not (PlayState.None or PlayState.OnBoss or PlayState.OnMiddleBoss))
            return;
        if (!PlayerUnit.IsControllable)
            return;
        if (!isPressed)
            return;
        
        PlayerInvincibility.SetInvincibility(4000);
        _playerBombHandler.UseBomb(transform.position);
        InGameDataManager.Instance.BombNumber--;
    }

    public void StopAttack()
    {
        IsFirePressed = false;
        _playerLaserHandler.StopLaser();
        _firePressFrame = 0;
        _playerUnit.IsAttacking = false;
        _playerUnit.SlowMode = false;
        _playerShotHandler.AutoShot = 0;
    }
}

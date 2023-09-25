using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerBombHandler m_PlayerBomb;
    
    private PlayerBombHandler _playerBombHandler;
    private PlayerLaserHandler _playerLaserHandler;
    private PlayerShotHandler _playerShotHandler;
    private PlayerMovement _playerMovement;
    private PlayerUnit _playerUnit;

    public bool IsFirePressed { get; private set; }
    public bool IsBombPressed { get; private set; }
    private int _firePressFrame;
    
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
        if (controllable)
            return;
        _playerMovement.HandlePlayerMovement(Vector2Int.zero);
    }
    
    private void Update()
    {
        if (!ReplayManager.IsUsingReplay)
            return;
        
        if (SystemManager.GameMode == GameMode.Replay)
            ReplayManager.Instance.ReadUserInput();
        else
            ReplayManager.Instance.WriteReplayData();

        if (IsFirePressed)
        {
            _firePressFrame++;
        }
        
        if (!PlayerUnit.IsControllable)
            return;
        ExecuteShot();
        ExecuteLaser();
        _playerMovement.ExecuteMovement();
    }

    public void OnFireInvoked(bool isPressed)
    {
        if (!ReplayManager.IsUsingReplay)
            return;
        
        HandleFireInput(isPressed);
        
        ReplayManager.Instance.WriteUserPressInput(isPressed, ReplayManager.KeyType.Fire);
        if (SystemManager.IsInGame)
            Debug.Log($"{ReplayManager.CurrentFrame}: FireInvoked {isPressed}");
    }

    public void HandleFireInput(bool isPressed)
    {
        IsFirePressed = false;
        
        if (SystemManager.PlayState is not (PlayState.None or PlayState.OnBoss or PlayState.OnMiddleBoss))
            return;
        
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

    private void ExecuteShot()
    {
        if (_playerShotHandler.AutoShot > 0) {
            if (!_playerUnit.IsShooting) {
                _playerUnit.IsShooting = true;
                _playerShotHandler.StartShotCoroutine();
            }
            _playerUnit.IsAttacking = true;
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
        if (!ReplayManager.IsUsingReplay)
            return;
        
        ExecuteBomb(isPressed);
        
        ReplayManager.Instance.WriteUserPressInput(isPressed, ReplayManager.KeyType.Bomb);
    }
    
    private void ExecuteBomb(bool isPressed)
    {
        if (InGameDataManager.Instance.BombNumber <= 0)
            return;
        if (_playerBombHandler.IsBombInUse)
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

    public void OnMoveInvoked(Vector2Int rawInputVector)
    {
        if (!ReplayManager.IsUsingReplay)
            return;
        
        _playerMovement.HandlePlayerMovement(rawInputVector);
        _playerShotHandler.ReceiveHorizontalMovement(rawInputVector.x);
        
        ReplayManager.Instance.WriteUserMovementInput(rawInputVector);
        if (SystemManager.IsInGame)
            Debug.Log($"{ReplayManager.CurrentFrame}: FireInvoked {rawInputVector}");
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

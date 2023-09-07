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
        _playerMovement.MovePlayer(Vector2.zero);
    }

    private void Update()
    {
        if (PauseManager.IsGamePaused)
            return;

        if (IsFirePressed)
        {
            _firePressFrame++;
        }

        FireShot();
        FireLaser();
    }

    private void OnFireInvoked(InputValue inputValue)
    {
        IsFirePressed = false;
        
        if (SystemManager.PlayState is not (PlayState.None or PlayState.OnBoss or PlayState.OnMiddleBoss))
            return;
        
        ExecuteFire(inputValue.isPressed);
    }

    public void ExecuteFire(bool isPressed)
    {
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
        
        ReplayManager.Instance.WriteUserPressInput(isPressed, 4);
    }

    private void FireShot()
    {
        if (_playerShotHandler.AutoShot > 0) {
            if (!_playerUnit.IsShooting) {
                _playerUnit.IsShooting = true;
                _playerShotHandler.StartShotCoroutine();
            }
            _playerUnit.IsAttacking = true;
        }
    }

    private void FireLaser()
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

    private void OnBombInvoked()
    {
        if (!PlayerUnit.IsControllable)
            return;
        if (InGameDataManager.Instance.BombNumber <= 0)
            return;
        if (_playerBombHandler.IsBombInUse)
            return;
        if (SystemManager.PlayState is not (PlayState.None or PlayState.OnBoss or PlayState.OnMiddleBoss))
            return;

        ExecuteBomb();
    }
    
    public void ExecuteBomb()
    {
        PlayerInvincibility.SetInvincibility(4000);
        _playerBombHandler.UseBomb(transform.position);
        InGameDataManager.Instance.BombNumber--;
        
        ReplayManager.Instance.WriteUserPressInput(true, 5);
    }

    private void OnMoveInvoked(InputValue inputValue)
    {
        var moveInput = inputValue.Get<Vector2>();

        _playerMovement.MovePlayer(moveInput);
        _playerShotHandler.ReceiveHorizontalMovement(moveInput.x);
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

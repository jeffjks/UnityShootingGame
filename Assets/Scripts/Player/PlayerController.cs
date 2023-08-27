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
    private InGameInputController _inGameInputController;

    public bool IsFirePressed { get; private set; }
    public bool IsBombPressed { get; private set; }
    private int _firePressFrame;
    
    void Awake()
    {
        _playerBombHandler = Instantiate(m_PlayerBomb);
        _playerLaserHandler = GetComponentInChildren<PlayerLaserHandler>();
        _playerShotHandler = GetComponent<PlayerShotHandler>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerUnit = GetComponent<PlayerUnit>();
        
        if (!_playerUnit.m_IsPreviewObject)
        {
            _inGameInputController = InGameInputController.Instance;

            _inGameInputController.Action_OnFireInput += OnFireInvoked;
            _inGameInputController.Action_OnBombInput += OnBombInvoked;
            _inGameInputController.Action_OnMove += OnMoveInvoked;
            _playerUnit.Action_OnControllableChanged += OnControllableChanged;
        }
    }

    private void OnControllableChanged(bool controllable)
    {
        if (controllable)
            return;
        _playerMovement.MovePlayer(Vector2.zero);
    }

    void Update()
    {
        if (Time.timeScale == 0)
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
        if (!PlayerUnit.IsControllable)
        {
            IsFirePressed = false;
            return;
        }
        ExecuteFire(inputValue.isPressed);
    }

    public void ExecuteFire(bool isPressed)
    {
        IsFirePressed = isPressed;

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
        if (!SystemManager.IsOnGamePlayState())
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
        if (!PlayerUnit.IsControllable)
            return;
        
        Vector2 moveInput = inputValue.Get<Vector2>();

        _playerMovement.MovePlayer(moveInput);
    }
    
    private void OnEnable()
    {
        //_playerShotHandler.AutoShot = 0;
        
        if (!_playerUnit.m_IsPreviewObject)
            InGameDataManager.Instance.InitBombNumber();
    }

    private void OnDisable()
    {
        IsFirePressed = false;
        _firePressFrame = 0;
        
        // _playerUnit.SlowMode = false;
        // _playerUnit.IsAttacking = false;
        // _playerUnit.IsShooting = false;
        
        _playerLaserHandler.StopLaser();
    }
}

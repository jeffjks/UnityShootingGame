using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject m_PlayerBomb;
    
    private PlayerBombHandler _playerBombHandler;
    private PlayerLaserHandler _playerLaserHandler;
    private PlayerShootHandler _playerShootHandler;
    private PlayerUnit _playerUnit;
    private IngameInputController _inGameInputController;

    private bool _isFirePress;
    private int _firePressFrame;
    
    void Awake()
    {
        var bombObject = Instantiate(m_PlayerBomb);
        _playerBombHandler = bombObject.GetComponent<PlayerBombHandler>();
        _playerLaserHandler = GetComponentInChildren<PlayerLaserHandler>();
        _playerShootHandler = GetComponent<PlayerShootHandler>();
        _playerUnit = GetComponent<PlayerUnit>();
        
        if (!_playerUnit.m_IsPreviewObject)
        {
            _inGameInputController = IngameInputController.Instance;

            _inGameInputController.Action_OnFireInput += OnFireInvoked;
            _inGameInputController.Action_OnBombInput += OnBombInvoked;
        }
    }

    
    void Start()
    {
        /*
        if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Bomb) == 0) // 폭탄 개수
            InGameDataManager.Instance.MaxBombNumber = 2;
        else
            InGameDataManager.Instance.MaxBombNumber = 3;
        InGameDataManager.Instance.InitBombNumber();

        if (m_Module != 0) {
            SetModule();
            UpdateShotNumber();
            StartCoroutine(ModuleShot());
        }
        else
            UpdateShotNumber();*/
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (_isFirePress)
        {
            _firePressFrame++;
        }

        FireShot();
        FireLaser();
    }

    private void OnFireInvoked(InputValue inputValue)
    {
        _isFirePress = inputValue.isPressed;

        if (inputValue.isPressed) // 누르는 순간
        {
            if (!_playerUnit.SlowMode) { // 샷 모드일 경우 AutoShot 증가
                if (_playerShootHandler.AutoShot < 2) {
                    _playerShootHandler.AutoShot++;
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

    private void FireShot()
    {
        if (_playerShootHandler.AutoShot > 0) {
            if (!_playerUnit.IsShooting) {
                _playerUnit.IsShooting = true;
                _playerShootHandler.StartShotCoroutine();
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
                _playerShootHandler.AutoShot = 0;
            }
        }
    }

    public void PlayerControllerBehaviour() {
        /*
        if (m_ShotKeyPress == 1) {
            _firePressFrame++;
            if (!m_ShotKeyPrevious) {
                m_ShotKeyPrevious = true;
                if (!_playerUnit.SlowMode) { // 샷 모드일 경우 AutoShot 증가
                    if (_playerShootHandler.AutoShot <= 1) {
                        _playerShootHandler.AutoShot++;
                    }
                }
            }
        }
        else {
            m_ShotKeyPrevious = false;
            _firePressFrame = 0;
            _playerUnit.SlowMode = false;
            _playerLaserHandler.StopLaser();
            _playerUnit.IsAttacking = false;
        }
        
        if (!_playerUnit.SlowMode) {
            if (_firePressFrame > Application.targetFrameRate / 2) { // 0.5초간 누르면 레이저 모드
                _playerUnit.SlowMode = true;
                _playerLaserHandler.StartLaser();
                _playerUnit.IsAttacking = true;
                _playerShootHandler.AutoShot = 0;
            }
        }

        if (_playerShootHandler.AutoShot > 0) {
            if (!_playerUnit.IsShooting) {
                _playerUnit.IsShooting = true;
                StartCoroutine(Shot());
            }
            _playerUnit.IsAttacking = true;
        }

        if (m_BombKeyPress == 1) {
            BombKeyPressed();
        }*/
    }

    private void OnBombInvoked() {
        if (InGameDataManager.Instance.BombNumber <= 0) {
            return;
        }
        if (_playerBombHandler.IsBombInUse) {
            return;
        }
        if (!SystemManager.IsOnGamePlayState()) {
            return;
        }
        PlayerInvincibility.SetInvincibility(4000);
        _playerBombHandler.UseBomb(transform.position);
        InGameDataManager.Instance.BombNumber--;
    }
    
    void OnEnable()
    {
        _isFirePress = false;
        _firePressFrame = 0;
        
        _playerUnit.SlowMode = false;
        _playerUnit.IsShooting = false;
        _playerUnit.IsAttacking = false;
        _playerShootHandler.AutoShot = 0;
        _playerLaserHandler.StopLaser();
        
        if (!_playerUnit.m_IsPreviewObject)
            InGameDataManager.Instance.InitBombNumber();
    }
}

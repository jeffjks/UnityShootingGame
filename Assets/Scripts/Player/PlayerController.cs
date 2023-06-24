using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerBomb m_PlayerBomb;
    public PlayerLaserShooterManager m_PlayerLaserShooterManager;

    private PlayerShootHandler _playerShootHandler;
    private PlayerUnit _playerUnit;
    private IngameInputController _inGameInputController;

    private bool _isFirePress;
    private int _firePressFrame;
    
    void Awake()
    {
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
    }

    private void OnFireInvoked(bool isPressed)
    {
        _isFirePress = isPressed;

        if (isPressed)
        {
            if (!_playerUnit.SlowMode) { // 샷 모드일 경우 AutoShot 증가
                if (_playerShootHandler.AutoShot < 2) {
                    _playerShootHandler.AutoShot++;
                }
            }
        }
        else
        {
            _firePressFrame = 0;
            _playerUnit.SlowMode = false;
            m_PlayerLaserShooterManager.StopLaser();
            _playerUnit.IsAttacking = false;
        }
        
        if (!_playerUnit.SlowMode) {
            if (_firePressFrame > Application.targetFrameRate / 2) { // 0.5초간 누르면 레이저 모드
                _playerUnit.SlowMode = true;
                m_PlayerLaserShooterManager.StartLaser();
                _playerUnit.IsAttacking = true;
                _playerShootHandler.AutoShot = 0;
            }
        }

        if (_playerShootHandler.AutoShot > 0) {
            if (!_playerUnit.IsShooting) {
                _playerUnit.IsShooting = true;
                _playerShootHandler.FireShot();
            }
            _playerUnit.IsAttacking = true;
        }
    }

    public void PlayerControllerBehaviour() {
        if (!PlayerMovement.IsControllable) {
            _playerUnit.SlowMode = false;
            _playerUnit.IsAttacking = false;
            //m_ShotKeyPress = 0;
            _playerShootHandler.AutoShot = 0;
            m_PlayerLaserShooterManager.StopLaser();
            return;
        }
        
        
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
            m_PlayerLaserShooterManager.StopLaser();
            _playerUnit.IsAttacking = false;
        }
        
        if (!_playerUnit.SlowMode) {
            if (_firePressFrame > Application.targetFrameRate / 2) { // 0.5초간 누르면 레이저 모드
                _playerUnit.SlowMode = true;
                m_PlayerLaserShooterManager.StartLaser();
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
        if (!m_PlayerBomb.GetEnableState()) {
            return;
        }
        if (!SystemManager.IsOnGamePlayState()) {
            return;
        }
        Vector3 bomb_pos = new Vector3(transform.position.x, transform.position.y, Depth.PLAYER_MISSILE);
        PlayerInvincibility.SetInvincibility(4000);
        m_PlayerBomb.UseBomb();
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
        m_PlayerLaserShooterManager.StopLaser();
        
        if (!_playerUnit.m_IsPreviewObject)
            InGameDataManager.Instance.InitBombNumber();
    }

    public void PowerSet(int power) {/*
        PlayerAttackLevel = Mathf.Clamp(power, 0, 4);
        ResetLaser();
        UpdateShotNumber();*/
    }

    public bool PowerUp() {/*
        if (PlayerAttackLevel < 4) {
            PlayerAttackLevel++;
            ResetLaser();
            UpdateShotNumber();
            return true;
        }

        UpdateShotNumber();*/
        return false;
    }
    
    public void PowerDown() {/*
        if (PlayerAttackLevel > 0) {
            PlayerAttackLevel--;
            ResetLaser();
        }
        UpdateShotNumber();*/
    }
}

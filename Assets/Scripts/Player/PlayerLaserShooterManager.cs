using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserShooterManager : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public GameObject[] m_LaserObjects = new GameObject[3];
    public PlayerLaserFireLight m_LaserFireLight;

    public const float HIT_OFFSET = 0.01f;
    public const float ENDPOINT_ALPHA = 0.2f;
    
    private const float LASER_SPEED = 30f;

    public float MaxLaserLength { get; private set; }
    
    private int _laserIndex;
    public int LaserIndex {
        get => _laserIndex;
        set
        {
            _laserIndex = value;
            SetLaserIndex();
        }
    }
    
    private PlayerLaserRenderer _playerLaserRenderer;
    private GameObject _laserInstance; // Laser Object

    public event Action Action_OnStartLaser;

    private void Start()
    {
        LaserIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        _laserInstance = m_LaserObjects[LaserIndex];
        m_LaserFireLight.SetLightColor(LaserIndex);
        
        _playerLaserRenderer = _laserInstance.GetComponent<PlayerLaserRenderer>();
    }
    
    private void SetLaserIndex() {
        StopLaser();
        
        _laserInstance = m_LaserObjects[LaserIndex];
        m_LaserFireLight.SetLightColor(LaserIndex);

        _laserInstance.SetActive(true);
        _playerLaserRenderer = _laserInstance.GetComponent<PlayerLaserRenderer>();
        _laserInstance.SetActive(false);

        if (m_PlayerUnit.SlowMode)
            StartLaser();
    }

    public void StartLaser() {
        Action_OnStartLaser?.Invoke();
        UpdateLaser();
        //if (transform.root.gameObject.activeInHierarchy)
        if (!m_PlayerUnit.m_IsPreviewObject)
            AudioService.PlaySound("PlayerLaser");
    }

    public void StopLaser() {
        if (_playerLaserRenderer != null) {
            _laserInstance.SetActive(false);
            m_LaserFireLight.gameObject.SetActive(false);
            _playerLaserRenderer.DisablePrepare();
        }
        MaxLaserLength = 0f;
        if (!m_PlayerUnit.m_IsPreviewObject)
            AudioService.StopSound("PlayerLaser");
    }

    private void UpdateLaser() {
        _laserInstance.SetActive(true);
        m_LaserFireLight.gameObject.SetActive(true);
        //_playerLaserRenderer.MaxLaserLength = MaxLaserLength;
        // _playerLaserRenderer.InitLaser();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_PlayerUnit.SlowMode) {
            MaxLaserLength += LASER_SPEED / Application.targetFrameRate * Time.timeScale;
        }
        else {
            MaxLaserLength = 0f;
        }

        var maxClampLength = m_PlayerUnit.m_IsPreviewObject ? 4f : -transform.position.y;
        MaxLaserLength = Mathf.Clamp(MaxLaserLength, 0f, maxClampLength);
        //if (_playerLaserRenderer != null)
        //    _playerLaserRenderer.MaxLaserLength = MaxLaserLength;
    }

    public void RestartLaser() {
        if (m_PlayerUnit.SlowMode) {
            StopLaser();
            StartLaser();
        }
    }
}

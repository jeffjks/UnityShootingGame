using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserHandler : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerLaser m_PlayerLaser;
    public PlayerLaserRenderer[] m_LaserRenderers;
    public PlayerDamageDatas[] m_PlayerDamageData;
    public PlayerLaserFireLight m_PlayerLaserFireLight;
    
    private int _laserIndex;
    //private int _laserAttackLevel;
    
    public int LaserIndex {
        get => _laserIndex;
        set
        {
            _laserIndex = value;
            SetLaserIndex();
        }
    }

    public event Action Action_OnStartLaser;
    public event Action Action_OnStopLaser;
    
    private GameObject _currentLaserInstance; // Laser Object

    private void Start()
    {
        LaserIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += () => m_PlayerLaser.DamageLevel = m_PlayerUnit.PlayerAttackLevel;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += RestartLaser;
        m_PlayerUnit.Action_OnControllableChanged += StopLaser;
    }
    
    private void SetLaserIndex() {
        StopLaser();
        
        _currentLaserInstance = m_LaserRenderers[LaserIndex].gameObject;
        m_PlayerLaser.SetPlayerDamageData(m_PlayerDamageData[LaserIndex]);
        m_PlayerLaserFireLight.SetLightColor(LaserIndex);
        m_PlayerLaser.DamageLevel = m_PlayerUnit.PlayerAttackLevel;

        _currentLaserInstance.SetActive(true);
        _currentLaserInstance.SetActive(false);

        if (m_PlayerUnit.SlowMode)
            StartLaser();
    }

    public void StartLaser() {
        _currentLaserInstance.SetActive(true);
        m_PlayerLaserFireLight.gameObject.SetActive(true);
        Action_OnStartLaser?.Invoke();
        
        if (!m_PlayerUnit.m_IsPreviewObject)
            AudioService.PlaySound("PlayerLaser", true);
    }

    public void StopLaser() {
        if (_currentLaserInstance != null)
            _currentLaserInstance.SetActive(false);
        m_PlayerLaserFireLight.gameObject.SetActive(false);
        Action_OnStopLaser?.Invoke();

        if (!m_PlayerUnit.m_IsPreviewObject)
            AudioService.StopSound("PlayerLaser");
    }

    private void RestartLaser() {
        if (m_PlayerUnit.SlowMode) {
            StopLaser();
            StartLaser();
        }
    }
}

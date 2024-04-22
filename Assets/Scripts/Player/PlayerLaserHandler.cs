using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserHandler : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerLaserRenderer[] m_LaserRenderers;
    public PlayerLaserFireLight m_PlayerLaserFireLight;
    
    private int _laserIndex;
    //private int _laserAttackLevel;
    
    public int LaserIndex {
        get => _laserIndex;
        set
        {
            _laserIndex = value;
            Action_OnLaserIndexChanged?.Invoke();
        }
    }

    public event Action Action_OnStartLaser;
    public event Action Action_OnStopLaser;
    public event Action Action_OnLaserIndexChanged;
    
    private GameObject _currentLaserInstance; // Laser Object

    private void Start()
    {
        Action_OnLaserIndexChanged += ResetLaserIndex;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += RestartLaser;
        m_PlayerUnit.Action_OnControllableChanged += (controllable) =>
        {
            if (controllable)
                return;
            StopLaser();
        };
        if (!m_PlayerUnit.m_IsPreviewObject)
            SystemManager.Action_OnBossClear += StopLaser;
        
        LaserIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
    }

    private void OnDestroy()
    {
        if (!m_PlayerUnit.m_IsPreviewObject)
            SystemManager.Action_OnBossClear -= StopLaser;
    }
    
    private void ResetLaserIndex() {
        StopLaser();
        
        _currentLaserInstance = m_LaserRenderers[LaserIndex].gameObject;
        m_PlayerLaserFireLight.SetLightColor(LaserIndex);

        _currentLaserInstance.SetActive(true);
        _currentLaserInstance.SetActive(false);

        if (m_PlayerUnit.SlowMode)
            StartLaser();
    }

    public void StartLaser() {
        _currentLaserInstance.SetActive(true);
        m_PlayerLaserFireLight.gameObject.SetActive(true);
        Action_OnStartLaser?.Invoke();
        
        if (ReplayManager.PlayerLaserStartLog)
            ReplayManager.WriteReplayLogFile($"Start Laser {PlayerManager.GetPlayerPosition().ToString("N6")}");
        
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

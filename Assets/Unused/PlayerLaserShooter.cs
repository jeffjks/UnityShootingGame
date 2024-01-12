using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

// UNUSED SCRIPT

public class PlayerLaserShooter : PlayerLaserHandler
{
/*
    public PlayerLaser m_PlayerLaser;

    void Start()
    {
        m_LaserIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        _laserInstance = m_LaserObjects[m_LaserIndex];
        m_LaserFireLight.SetLightColor(m_LaserIndex);
        
        _laserInstance.SetActive(true);
        _playerLaserRenderer = _laserInstance.GetComponent<PlayerLaserRenderer>();
        _laserInstance.SetActive(false);
    }

    public override void StartLaser() {
        m_PlayerLaser.UpdateLaserDamage();
        UpdateLaser();
        if (transform.root.gameObject.activeInHierarchy)
            AudioService.PlaySound("PlayerLaser");
    }

    public override void StopLaser() {
        if (_playerLaserRenderer != null) {
            _laserInstance.SetActive(false);
            m_LaserFireLight.gameObject.SetActive(false);
            _playerLaserRenderer.DisablePrepare();
        }
        m_MaxLaserLength = 0f;
        AudioService.StopSound("PlayerLaser");
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_PlayerUnit.SlowMode) {
            m_MaxLaserLength += m_LaserSpeed / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_MaxLaserLength = 0f;
        }
        m_MaxLaserLength = Mathf.Clamp(m_MaxLaserLength, 0f, -transform.position.y);
        if (_playerLaserRenderer != null)
            _playerLaserRenderer.m_MaxLaserLength = m_MaxLaserLength;
    }
*/
}
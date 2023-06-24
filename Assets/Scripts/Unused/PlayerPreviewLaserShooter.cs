using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerPreviewLaserShooter : PlayerLaserShooterManager
{
/*
    public void SetLaserType(int laserIndex) {
        StopLaser();
        m_LaserIndex = laserIndex;
        _laserInstance = m_LaserObjects[m_LaserIndex];
        m_LaserFireLight.SetLightColor(m_LaserIndex);

        _laserInstance.SetActive(true);
        _playerLaserRenderer = _laserInstance.GetComponent<PlayerLaserRenderer>();
        _laserInstance.SetActive(false);

        if (m_PlayerUnit.SlowMode)
            StartLaser();
    }

    public override void StartLaser() {
        UpdateLaser();
    }

    public override void StopLaser() {
        if (_playerLaserRenderer != null) {
            _laserInstance.SetActive(false);
            m_LaserFireLight.gameObject.SetActive(false);
            _playerLaserRenderer.DisablePrepare();
        }
        m_MaxLaserLength = 0f;
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
        m_MaxLaserLength = Mathf.Clamp(m_MaxLaserLength, 0f, 4f);
        if (_playerLaserRenderer != null)
            _playerLaserRenderer.m_MaxLaserLength = m_MaxLaserLength;
    }
*/
}
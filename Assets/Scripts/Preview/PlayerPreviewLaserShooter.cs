using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerPreviewLaserShooter : PlayerLaserShooterManager
{
    private PlayerManager m_PlayerManager = null;

    void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        SetLaserType();
    }

    public void SetLaserType() {
        StopLaser();
        m_LaserIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserLevel);
        m_LaserInstance = m_LaserObjects[m_LaserIndex];
        m_LaserFireLight.SetLightColor(m_LaserIndex);

        m_LaserInstance.SetActive(true);
        m_PlayerLaserCreater = m_LaserInstance.GetComponent<PlayerLaserCreater>();
        m_LaserInstance.SetActive(false);

        if (m_PlayerController.m_SlowMode)
            StartLaser();
    }

    public override void StartLaser() {
        UpdateLaser();
    }

    public override void StopLaser() {
        if (m_PlayerLaserCreater != null) {
            m_LaserInstance.SetActive(false);
            m_LaserFireLight.gameObject.SetActive(false);
            m_PlayerLaserCreater.DisablePrepare();
        }
        m_MaxLaserLength = 0f;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_PlayerController.m_SlowMode) {
            m_MaxLaserLength += m_LaserSpeed / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_MaxLaserLength = 0f;
        }
        m_MaxLaserLength = Mathf.Clamp(m_MaxLaserLength, 0f, 4f);
        if (m_PlayerLaserCreater != null)
            m_PlayerLaserCreater.m_MaxLaserLength = m_MaxLaserLength;
    }
}

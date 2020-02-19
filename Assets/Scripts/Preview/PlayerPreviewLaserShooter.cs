using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerPreviewLaserShooter : PlayerLaserShooterManager
{
    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        SetLaserType();
    }

    public void SetLaserType() {
        StopLaser();
        m_LaserIndex = m_GameManager.m_CurrentAttributes.m_LaserDamage;
        m_LaserInstance = m_LaserObjects[m_LaserIndex];

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
            m_PlayerLaserCreater.DisablePrepare();
        }
        m_MaxLength = 0f;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_PlayerController.m_SlowMode) {
            m_MaxLength += 30f * Time.deltaTime;
        }
        else {
            m_MaxLength = 0f;
        }
        m_MaxLength = Mathf.Clamp(m_MaxLength, 0f, 4f);
        if (m_PlayerLaserCreater != null)
            m_PlayerLaserCreater.m_MaxLength = m_MaxLength;
    }
}

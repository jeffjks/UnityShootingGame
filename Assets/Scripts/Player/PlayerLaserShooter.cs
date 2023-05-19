using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserShooter : PlayerLaserShooterManager
{
    public PlayerLaser m_PlayerLaser;

    private PlayerManager m_PlayerManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;

        m_LaserIndex = m_PlayerManager.m_CurrentAttributes.m_LaserDamage;
        m_LaserInstance = m_LaserObjects[m_LaserIndex];
        m_LaserFireLight.SetLightColor(m_LaserIndex);
        
        m_LaserInstance.SetActive(true);
        m_PlayerLaserCreater = m_LaserInstance.GetComponent<PlayerLaserCreater>();
        m_LaserInstance.SetActive(false);
    }

    public override void StartLaser() {
        m_PlayerLaser.UpdateLaserDamage();
        UpdateLaser();
        if (transform.root.gameObject.activeInHierarchy)
            AudioService.PlaySound("PlayerLaser");
    }

    public override void StopLaser() {
        if (m_PlayerLaserCreater != null) {
            m_LaserInstance.SetActive(false);
            m_LaserFireLight.gameObject.SetActive(false);
            m_PlayerLaserCreater.DisablePrepare();
        }
        m_MaxLaserLength = 0f;
        AudioService.StopSound("PlayerLaser");
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
        m_MaxLaserLength = Mathf.Clamp(m_MaxLaserLength, 0f, -transform.position.y);
        if (m_PlayerLaserCreater != null)
            m_PlayerLaserCreater.m_MaxLaserLength = m_MaxLaserLength;
    }
}

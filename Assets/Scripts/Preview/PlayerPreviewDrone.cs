using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewDrone : PlayerDrone
{
    private PlayerShooterManager m_PlayerShooter;
    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_PlayerShooter = GetComponentInParent<PlayerShooterManager>();
        m_PlayerController = GetComponentInParent<PlayerControllerManager>();
        ((PlayerPreviewShooter) m_PlayerShooter).InitShotLevel();
    }

    void Start()
    {
        m_DefaultDepth = transform.localPosition.y;
        SetPreviewDrones();
    }

    public void SetPreviewDrones() {
        int laser_damage;
        m_ShotForm = m_GameManager.m_CurrentAttributes.m_ShotForm;

        laser_damage = m_GameManager.m_CurrentAttributes.m_LaserDamage;
        
        SetShotLevel(m_PlayerShooter.m_ShotLevel);

        if (laser_damage == 0)
            m_ShockWaveNumber = 0;
        else
            m_ShockWaveNumber = 1;

        m_ParticleSystem[m_ShockWaveNumber].gameObject.SetActive(true);
        m_ParticleSystem[1 - m_ShockWaveNumber].gameObject.SetActive(false);
    }
}

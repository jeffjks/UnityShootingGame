using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewDrone : PlayerDrone
{
    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
    }
    
    void Start()
    {
        m_DefaultDepth = transform.localPosition.y;
        
        m_PlayerController = GetComponentInParent<PlayerControllerManager>();

        m_ParticleSystem[m_ShockWaveNumber].gameObject.SetActive(true);
        m_ParticleLocalScale = m_ParticleSystem[m_ShockWaveNumber].transform.localScale[0];

        SetPreviewDrones();
    }

    public void SetPreviewDrones() {
        int laser_damage;
        m_ShotForm = m_GameManager.m_CurrentAttributes.m_ShotForm;

        laser_damage = m_GameManager.m_CurrentAttributes.m_LaserDamage;

        if (laser_damage == 0)
            m_ShockWaveNumber = 0;
        else
            m_ShockWaveNumber = 1;
    }
}

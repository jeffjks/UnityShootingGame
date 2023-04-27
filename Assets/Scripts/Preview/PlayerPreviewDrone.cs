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
        m_PlayerController = GetComponentInParent<PlayerUnit>();
        ((PlayerPreviewShooter) m_PlayerShooter).InitShotLevel();
    }

    void Start()
    {
        m_DefaultDepth = transform.localPosition.y;
        SetPreviewDrones();
    }
}

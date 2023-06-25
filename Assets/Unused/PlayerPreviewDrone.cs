using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewDrone : PlayerDrone
{
    private PlayerShootHandler m_PlayerController;
    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_PlayerController = GetComponentInParent<PlayerShootHandler>();
        m_PlayerMovement = GetComponentInParent<PlayerUnit>();
        //((PlayerPreviewShooter) m_PlayerController).InitShotLevel();
    }

    void Start()
    {
        m_DefaultDepth = transform.localPosition.y;
        SetPreviewDrones();
    }
}

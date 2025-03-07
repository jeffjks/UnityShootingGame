﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerPreviewSimulator : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerLaserHandler m_PlayerLaserHandler;
    public bool m_AutoChangeMode;
    
    [SerializeField] private PlayerShotHandler m_PlayerShotHandler;
    private bool _shotMode;

    private void OnEnable()
    {
        _shotMode = true;
        SetShotMode();
        m_PlayerUnit.IsAttacking = true;
        
        if (m_AutoChangeMode)
            StartCoroutine(PreviewSlowMode());
        
        m_PlayerShotHandler.StartShotCoroutine();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        SimulateShotMode();
    }

    private IEnumerator PreviewSlowMode() {
        yield return new WaitForMillisecondFrames(0);
        while(true)
        {
            yield return new WaitForMillisecondFrames(Random.Range(1000, 3000));
            ToggleShotMode();
        }
    }

    public void ToggleShotMode()
    {
        _shotMode = !_shotMode;
        if (_shotMode)
        {
            SetShotMode();
        }
        else
        {
            SetLaserMode();
        }
    }

    private void SetShotMode()
    {
        m_PlayerUnit.SlowMode = false;
        m_PlayerLaserHandler.StopLaser();
    }

    private void SetLaserMode()
    {
        m_PlayerUnit.SlowMode = true;
        m_PlayerLaserHandler.StartLaser();
    }

    private void SimulateShotMode()
    {
        if (_shotMode && !m_PlayerUnit.IsShooting)
        {
            m_PlayerShotHandler.AutoShot = 1;
        }
    }
}

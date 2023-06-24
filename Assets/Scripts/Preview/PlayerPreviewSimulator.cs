using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerPreviewSimulator : MonoBehaviour
{
    public PlayerUnit m_PlayerUnit;
    public PlayerLaserShooterManager m_PlayerLaserShooterManager;
    
    private PlayerShootHandler _playerShootHandler;
    private bool _shotMode;
    
    void Awake()
    {
        _playerShootHandler = GetComponent<PlayerShootHandler>();
        m_PlayerUnit.IsAttacking = true;
    }

    void OnEnable() {
        StartCoroutine(PreviewSlowMode());
    }

    private void Update()
    {
        SimulateShotMode();
    }

    private IEnumerator PreviewSlowMode() {
        yield return new WaitForMillisecondFrames(0);
        while(true)
        {
            _shotMode = true;
            SetShotMode();
            yield return new WaitForMillisecondFrames(Random.Range(1000, 3000));

            _shotMode = false;
            SetLaserMode();
            yield return new WaitForMillisecondFrames(Random.Range(1000, 3000));
        }
    }

    private void SetShotMode()
    {
        m_PlayerUnit.SlowMode = false;
        m_PlayerLaserShooterManager.StopLaser();
    }

    private void SetLaserMode()
    {
        m_PlayerUnit.SlowMode = true;
        m_PlayerLaserShooterManager.StartLaser();
    }

    private void SimulateShotMode()
    {
        if (_shotMode && !m_PlayerUnit.IsShooting)
        {
            _playerShootHandler.FireShot();
            m_PlayerUnit.IsShooting = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class PlayerLaserShooter : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public PlayerLaser m_PlayerLaser;
    public GameObject[] m_LaserObjects = new GameObject[3];
    public AudioSource m_AudioLaser = null;

    public float m_HitOffset;
    public float m_EndPointAlpha;

    [HideInInspector] public float m_MaxLength = 0f;
    [HideInInspector] public int m_LaserIndex;
    
    private PlayerLaserCreater m_PlayerLaserCreater;
    private GameObject m_LaserInstance;
    private PlayerManager m_PlayerManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_LaserIndex = m_PlayerManager.m_CurrentAttributes[4];
        
        m_LaserInstance = m_LaserObjects[m_LaserIndex];
        m_LaserInstance.SetActive(true);
        m_PlayerLaserCreater = m_LaserInstance.GetComponent<PlayerLaserCreater>();
        m_LaserInstance.SetActive(false);
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
        m_MaxLength = Mathf.Clamp(m_MaxLength, 0f, -transform.position.y);
        if (m_PlayerLaserCreater != null)
            m_PlayerLaserCreater.m_MaxLength = m_MaxLength;
    }

    public void StartLaser() {
        m_PlayerLaser.UpdateLaserDamage();
        UpdateLaser();
        m_AudioLaser.Play();
    }

    public void StopLaser() {
        if (m_PlayerLaserCreater != null) {
            m_LaserInstance.SetActive(false);
            m_PlayerLaserCreater.DisablePrepare();
        }
        m_MaxLength = 0f;
        m_AudioLaser.Stop();
    }

    private void UpdateLaser() {
        m_LaserInstance.SetActive(true);
        m_PlayerLaserCreater.m_MaxLength = m_MaxLength;
        // m_PlayerLaserCreater.InitLaser();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public abstract class PlayerLaserShooterManager : MonoBehaviour
{
    public PlayerControllerManager m_PlayerController;
    public GameObject[] m_LaserObjects = new GameObject[3];

    public float m_HitOffset;
    public float m_EndPointAlpha;

    [HideInInspector] public float m_MaxLength;
    [HideInInspector] public int m_LaserIndex; // Laser Damage Type
    
    protected PlayerLaserCreater m_PlayerLaserCreater;
    protected GameObject m_LaserInstance; // Laser Object
    public abstract void StartLaser();
    public abstract void StopLaser();

    protected void UpdateLaser() {
        m_LaserInstance.SetActive(true);
        m_PlayerLaserCreater.m_MaxLength = m_MaxLength;
        // m_PlayerLaserCreater.InitLaser();
    }
}

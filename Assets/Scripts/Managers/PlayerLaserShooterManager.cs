using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public abstract class PlayerLaserShooterManager : MonoBehaviour
{
    public PlayerControllerManager m_PlayerController;
    public GameObject[] m_LaserObjects = new GameObject[3];
    public PlayerLaserFireLight m_LaserFireLight;
    
    protected float m_LaserSpeed = 30f;

    public float m_HitOffset;
    public float m_EndPointAlpha;

    [HideInInspector] public float m_MaxLaserLength;
    [HideInInspector] public int m_LaserIndex; // Laser Damage Type
    
    protected PlayerLaserCreater m_PlayerLaserCreater;
    protected GameObject m_LaserInstance; // Laser Object
    public abstract void StartLaser();
    public abstract void StopLaser();

    protected void UpdateLaser() {
        m_LaserInstance.SetActive(true);
        m_LaserFireLight.gameObject.SetActive(true);
        m_PlayerLaserCreater.m_MaxLaserLength = m_MaxLaserLength;
        // m_PlayerLaserCreater.InitLaser();
    }
}

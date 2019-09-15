using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewLaser : PlayerLaserManager {

    void Start()
    {
        m_LaserIndex = m_PlayerLaserShooter.m_LaserIndex;
    }
}
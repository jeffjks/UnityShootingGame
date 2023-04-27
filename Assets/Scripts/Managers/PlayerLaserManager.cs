using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserManager : PlayerObject {

    public PlayerLaserShooterManager m_PlayerLaserShooter;
    [HideInInspector] public int m_LaserDamage;
    
    protected int m_LaserIndex;
}
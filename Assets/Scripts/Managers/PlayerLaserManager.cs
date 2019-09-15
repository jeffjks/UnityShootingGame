using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserManager : PlayerDamageUnit {

    public PlayerLaserShooterManager m_PlayerLaserShooter;
    
    protected int m_LaserIndex;
    
    public override void OnDeath() {
        Destroy(gameObject);
    }
}
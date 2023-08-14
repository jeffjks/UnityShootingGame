using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShipLarge : EnemyUnit
{
    public EnemyShipLarge_FrontTurret m_FrontTurret;
    public EnemyShipLarge_BackTurret m_BackTurret;
    private int _phase = 1;
    
    void Start()
    {
        CurrentAngle = m_MoveVector.direction;

        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
    }
    
    protected override void Update()
    {
        base.Update();
        
        CurrentAngle = m_MoveVector.direction;
    }

    private void DestroyChildEnemy() {
        if (_phase >= 2) {
            return;
        }
        if (m_EnemyHealth.HealthPercent <= 0.33f) {
            _phase = 2;
            StartPattern("2A", new EnemyShipLarge_BulletPattern_2A(this));
            if (m_FrontTurret != null)
                m_FrontTurret.m_EnemyDeath.KillEnemy();
            if (m_BackTurret != null)
                m_BackTurret.m_EnemyDeath.KillEnemy();
        }
    }
}

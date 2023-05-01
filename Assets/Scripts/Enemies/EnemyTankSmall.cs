using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall : EnemyUnit
{
    public EnemyUnit m_Turret;
    public bool m_HasDestroyableTurret;

    void Start()
    {
        if (m_HasDestroyableTurret) {
            m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
    }

    private void DestroyChildEnemy() {
        if (m_EnemyHealth.m_HealthPercent <= 0.50f) { // 체력 50% 이하
            m_Turret?.m_EnemyDeath.OnDying();
        }
    }
}

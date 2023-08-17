using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret2 : EnemyUnit
{
    public EnemyUnit m_Turret;
    
    void Start()
    {
        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private void DestroyChildEnemy() {
        if (m_EnemyHealth.HealthPercent <= 0.50f) { // 체력 50% 이하
            if (m_Turret != null)
                m_EnemyDeath.KillEnemy();
        }
    }
}

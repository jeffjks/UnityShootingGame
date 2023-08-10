using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBody : EnemyUnit
{
    public EnemyUnit m_Turret;
    public float m_HealthPercentTurretDestroying;

    void Start()
    {
        if (m_HealthPercentTurretDestroying > 0f)
            m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private void DestroyChildEnemy() {
        if (m_EnemyHealth.HealthPercent <= m_HealthPercentTurretDestroying) {
            if (m_Turret != null)
                m_Turret.m_EnemyDeath.OnDying();
        }
    }
}

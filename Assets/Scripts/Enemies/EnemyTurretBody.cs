using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBody : EnemyUnit
{
    public EnemyUnit m_Turret;
    public int m_HealthPercentTurretDestroying;

    protected override void Start()
    {
        base.Start();

        if (m_HealthPercentTurretDestroying > 0f)
            m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private void DestroyChildEnemy() {
        if (m_EnemyHealth.HealthPercent <= (float) m_HealthPercentTurretDestroying / 100) {
            if (m_Turret != null)
                m_Turret.m_EnemyDeath.KillEnemy();
        }
    }
}

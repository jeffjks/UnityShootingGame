using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretBody : EnemyUnit
{
    public EnemyUnit m_Turret;
    public int m_HealthRatioScaledTurretDestroying;

    private void Start()
    {
        if (m_HealthRatioScaledTurretDestroying > 0f)
            m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private void DestroyChildEnemy()
    {
        if (m_EnemyHealth.HealthRatioScaled > m_HealthRatioScaledTurretDestroying)
            return;
        if (m_Turret != null)
            m_Turret.m_EnemyDeath.KillEnemy();
    }
}

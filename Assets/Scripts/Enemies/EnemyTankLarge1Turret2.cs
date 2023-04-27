using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1Turret2 : EnemyUnit
{
    protected override void Update()
    {
        base.Update();

        if (3 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            m_EnemyHealth.OnDeath();
        }

        RotateImmediately(m_ParentEnemy.m_CurrentAngle);
    }
}

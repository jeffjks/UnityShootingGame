using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1Turret2 : EnemyUnit
{
    protected override void Update()
    {
        if (3 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }

        RotateImmediately(m_ParentEnemy.m_CurrentAngle);
        
        base.Update();
    }
}

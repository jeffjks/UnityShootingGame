using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1Turret2 : EnemyUnit
{
    protected override void FixedUpdate()
    {
        if (3 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        base.FixedUpdate();
    }
}

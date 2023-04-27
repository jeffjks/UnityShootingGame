using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBoss3Part : EnemyUnit
{
    protected override void Update()
    {
        base.Update();
        
        m_CurrentAngle = m_ParentEnemy.m_CurrentAngle;
    }
}

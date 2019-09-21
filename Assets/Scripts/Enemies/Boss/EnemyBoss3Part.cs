using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyBoss3Part : EnemyUnit
{
    protected override void FixedUpdate()
    {
        m_CurrentAngle = m_ParentEnemy.m_CurrentAngle;
        base.FixedUpdate();
    }
}

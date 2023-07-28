using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2_Turret2 : EnemyUnit
{
    private int _killScore;

    void Start()
    {
        RotateUnit(AngleToPlayer);
        _killScore = m_Score;
        m_Score = 0;
        
        SetRotatePattern(new RotatePattern_TargetPlayer());
        StartPattern("0", new BulletPattern_EnemyMiddleBoss2_Turret2_0(this));
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = _killScore;
        }
    }
}

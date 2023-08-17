using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2_SubTurret : EnemyUnit
{
    private int _killScore;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        _killScore = m_Score;
        m_Score = 0;
        
        SetRotatePattern(new RotatePattern_TargetPlayer());
        StartPattern("0", new BulletPattern_EnemyMiddleBoss2_SubTurret_0(this));
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = _killScore;
        }
    }
}

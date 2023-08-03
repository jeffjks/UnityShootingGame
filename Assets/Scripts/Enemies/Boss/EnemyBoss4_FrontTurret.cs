using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4_FrontTurret : EnemyUnit
{
    private int _killScore;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
        _killScore = m_Score;
        m_Score = 0;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = _killScore;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4_SmallTurret : EnemyUnit
{
    private int _killScore;

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        _killScore = m_Score;
        m_Score = 0;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = _killScore;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMiddleBoss4_Part : EnemyUnit
{
    public EnemyItemCreater m_EnemyItemCreater;

    private void Start()
    {
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
        m_CustomDirection = new CustomDirection();
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        m_CustomDirection[0] -= 20f / Application.targetFrameRate * Time.timeScale;
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_EnemyItemCreater.enabled = true;
        }
    }
}

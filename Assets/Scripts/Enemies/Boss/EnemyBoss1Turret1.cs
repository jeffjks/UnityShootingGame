using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret1 : EnemyUnit
{
    [HideInInspector] public int m_Phase;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        _bulletPatterns.Add("1A", new BulletPattern_EnemyBoss1_Turret1_1A(this));
        _bulletPatterns.Add("2A", new BulletPattern_EnemyBoss1_Turret1_2A(this));
        _bulletPatterns.Add("2B", new BulletPattern_EnemyBoss1_Turret1_2B(this));
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (PlayerManager.IsPlayerAlive)
                RotateImmediately(PlayerManager.GetPlayerPosition());
            else
                RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        }
        else {
            RotateSlightly(0f, 100f);
        }
    }

    public void StartPattern(string key, int patternIndex = 0)
    {
        m_CurrentPattern = _bulletPatterns[key].ExecutePattern(patternIndex);
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
}
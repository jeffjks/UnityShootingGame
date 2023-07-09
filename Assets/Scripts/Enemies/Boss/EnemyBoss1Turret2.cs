using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret2 : EnemyUnit
{
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        _bulletPatterns.Add("1A", new BulletPattern_EnemyBoss1_Turret2_1A(this));
        _bulletPatterns.Add("2A", new BulletPattern_EnemyBoss1_Turret2_2A(this));
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(PlayerManager.GetPlayerPosition());
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    public void StartPattern(string key)
    {
        m_CurrentPattern = _bulletPatterns[key].ExecutePattern();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
}
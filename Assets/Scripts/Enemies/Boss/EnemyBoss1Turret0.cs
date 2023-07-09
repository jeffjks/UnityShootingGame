using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret0 : EnemyUnit, IEnemySubAttacker
{
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        _bulletPatterns.Add("1A", new BulletPattern_EnemyBoss1_Turret0_1A(this));
        _bulletPatterns.Add("2A", new BulletPattern_EnemyBoss1_Turret0_2A(this));
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

    private IEnumerator Pattern1()
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos = m_FirePosition[0].position;
        CreateBulletsSector(0, pos, 7f, CurrentAngle + Random.Range(-2f, 2f), accel, 7, 14f);
        yield break;
    }

    private IEnumerator Pattern2()
    {
        yield break;
    }
}
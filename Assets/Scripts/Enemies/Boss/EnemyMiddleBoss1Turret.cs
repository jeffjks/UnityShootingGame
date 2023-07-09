using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMiddleBoss1Turret : EnemyUnit, IEnemySubAttacker
{
    private int m_Phase;
    private IEnumerator m_CurrentPattern;
    private int m_RotateState;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());

        _bulletPatterns.Add("1A", new BulletPattern_EnemyMiddleBoss1_Turret_1A(this, param => m_RotateState = param));
        _bulletPatterns.Add("2A", new BulletPattern_EnemyMiddleBoss1_Turret_2A(this, param => m_RotateState = param));
    }

    protected override void Update()
    {
        base.Update();

        if (m_RotateState == 0) {
            if (IsRotatable) {
                RotateSlightly(PlayerManager.GetPlayerPosition(), 90f);
            }
        }
        else if (m_RotateState == 1) {
            RotateImmediately(CurrentAngle + 300f*transform.localScale.x / Application.targetFrameRate * Time.timeScale);
        }
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

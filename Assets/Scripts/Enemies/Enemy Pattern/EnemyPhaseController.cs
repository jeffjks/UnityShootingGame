using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPhaseController : MonoBehaviour
{
    [SerializeField] private UnityEvent[] _onNextPhase;
    private int _phase;
    private Coroutine _currentPhaseCoroutine;
    private EnemyUnit _enemyUnit;
    
    private readonly List<IEnumerator> _coroutineList = new();

    private void Start()
    {
        _enemyUnit = GetComponent<EnemyUnit>();
        
        _coroutineList.Add(Phase1());

        _enemyUnit.m_EnemyHealth.Action_OnHealthChanged += CheckNextPhase;
    }

    private void StartNextPhase()
    {
        _onNextPhase[_phase]?.Invoke();
        _phase++;
        _enemyUnit.StopAllPatterns();
        
        if (_currentPhaseCoroutine != null)
            StopCoroutine(_currentPhaseCoroutine);
        StartCoroutine(_coroutineList[_phase]);
    }

    private void CheckNextPhase()
    {/*
        if (_phase == 1)
        {
            if (_enemyUnit.m_EnemyHealth.HealthPercent <= 0.65f) { // 체력 65% 이하
                for (int i = 0; i < m_FrontTurrets.Length; i++) {
                    if (m_FrontTurrets[i] != null)
                        m_FrontTurrets[i].m_EnemyDeath.OnDying();
                }
                BulletManager.SetBulletFreeState(2000);
                NextPhaseExplosion();
                ToNextPhase();
            }
        }*/
    }

    private IEnumerator Phase1()
    {
        yield break;
    }
}

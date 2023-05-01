using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public event Action Action_OnDying;
    public event Action Action_OnDeath;
    public event Action Action_OnRemoved;
    [HideInInspector] public bool m_IsDead = false;

    void Start()
    {
        Action_OnDying += SetDeadState;
    }

    private void SetDeadState() {
        m_IsDead = true;
    }

    public void OnDying() {
        if (m_IsDead) {
            return;
        }
        Action_OnDying?.Invoke();
        
        EnemyDeath[] enemyDeath = GetComponentsInChildren<EnemyDeath>();
        for (int i = 0; i < enemyDeath.Length; ++i) {
            enemyDeath[i].OnDying();
        }
    }

    public void OnDeath() {
        Action_OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void OnRemoved() {
        Action_OnRemoved?.Invoke();
        Destroy(gameObject);
    }
}
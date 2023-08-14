using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public event Action Action_OnKilled;
    public event Action Action_OnEndDeathAnimation;
    public event Action Action_OnRemoved;
    public bool IsDead { get; set; }

    void Start()
    {
        Action_OnKilled += SetDeadState;
    }

    private void SetDeadState() {
        IsDead = true;
    }

    public void KillEnemy() {
        if (IsDead) {
            return;
        }
        Action_OnKilled?.Invoke();
        
        EnemyDeath[] enemyDeath = GetComponentsInChildren<EnemyDeath>();
        for (int i = 0; i < enemyDeath.Length; ++i) {
            enemyDeath[i].KillEnemy();
        }
    }

    public void OnEndDeathAnimation() {
        Action_OnEndDeathAnimation?.Invoke();
        Destroy(gameObject);
    }

    public void RemoveEnemy() {
        Action_OnRemoved?.Invoke();
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public event Action Action_OnKilled;
    public event Action Action_OnEndDeathAnimation;
    public event Action Action_OnRemoved;
    public bool IsDead { get; private set; }

    public void KillEnemy()
    {
        if (IsDead) {
            return;
        }
        IsDead = true;
        
        Action_OnKilled?.Invoke();
        
        EnemyDeath[] enemyDeaths = GetComponentsInChildren<EnemyDeath>();
        foreach (var enemyDeath in enemyDeaths)
        {
            if (enemyDeath == this)
                continue;
            enemyDeath.KillEnemy();
        }
        //Debug.Log($"{ReplayManager.CurrentFrame}: killed {name} at {transform.position}");
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
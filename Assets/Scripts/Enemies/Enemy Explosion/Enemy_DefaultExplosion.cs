using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DefaultExplosion : EnemyExplosionCreater
{
    [SerializeField] private ExplosionEffect m_ExplosionEffect;
    [SerializeField] private ExplosionAudio m_ExplosionAudio;

    protected override IEnumerator DyingExplosion()
    {
        MoveVector moveVector = new MoveVector();
        if ((1 << gameObject.layer & Layer.AIR) != 0) {
            EnemyUnit enemyUnit = gameObject.GetComponent<EnemyUnit>();
            moveVector = enemyUnit.m_MoveVector;
        }

        CreateExplosionEffect(m_ExplosionEffect, m_ExplosionAudio, Vector3.zero, moveVector);
        
        m_EnemyDeath.OnDeath();
        yield break;
    }
}

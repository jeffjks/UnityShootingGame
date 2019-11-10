using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyMiddleBoss4Part : EnemyUnit
{
    protected override void KilledByPlayer() {
        m_GemNumber = 12;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, 1.6f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, 0.6f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, -0.4f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, -1.4f));
        Destroy(gameObject);
        yield break;
    }
}

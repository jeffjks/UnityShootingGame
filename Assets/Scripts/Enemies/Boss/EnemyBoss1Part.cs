using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Part : EnemyUnit
{
    protected override void KilledByPlayer() {
        m_SystemManager.BulletsToGems(0f);
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.EraseBullets(1.5f);
        ((EnemyBoss1) m_ParentEnemy).m_Phase = 1;

        ExplosionEffect(0, -1, new Vector3(-0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(-0.62f, 0f, 0.33f));
        ExplosionEffect(0, -1, new Vector3(0.62f, 0f, 0.33f));
        ExplosionEffect(1, -1, new Vector3(-0.69f, 0f, -0.4f));
        ExplosionEffect(1, -1, new Vector3(0.69f, 0f, -0.4f));
        Destroy(gameObject);
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Part : EnemyUnit
{
    public EnemyUnit[] m_Turret;
    public int m_NextPhaseDelay;

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.EraseBullets(2000);
        for (int i = 0; i < m_Turret.Length; i++)
            m_Turret[i].OnDeath(); // 이펙트용
        
        ((EnemyBoss2) m_ParentEnemy).ToNextPhase(m_NextPhaseDelay);
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}

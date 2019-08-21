using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret1 : EnemyUnit
{
    [SerializeField] private GameObject m_Collider = null;
    [SerializeField] private GameObject m_Turret = null;

    protected override IEnumerator AdditionalOnDeath() { // 추가 폭발 이펙트 (기본값은 없음)
        Destroy(m_Collider);
        Destroy(m_Turret);
        ImageBlend(m_DefaultAlbedo);
        yield break;
    }
}

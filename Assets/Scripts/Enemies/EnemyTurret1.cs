using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret1 : EnemyUnit
{
    public GameObject m_Collider;
    public GameObject m_Turret;
    
    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);
        base.Update();
    }

    protected override IEnumerator AdditionalOnDeath() { // 추가 폭발 이펙트 (기본값은 없음)
        Destroy(m_Collider);
        Destroy(m_Turret);
        m_Collider2D = new Collider2D[0];
        ImageBlend(m_DefaultAlbedo);
        
        CreateItems();
        yield break;
    }
}

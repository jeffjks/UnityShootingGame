using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2 : EnemyUnit
{
    public EnemyUnit[] m_Turret = new EnemyUnit[4];
    private int _phase;

    protected override void Start()
    {
        base.Start();

        m_MoveVector = new MoveVector(0.8f, 0f);
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (_phase == 0) {
            if (m_Position2D.y < - 1f)
            {
                StartPattern("1A", new EnemyPlaneLarge2_BulletPattern_1A(this));
                _phase = 1;
            }
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        for (int i = 0; i < m_Turret.Length; i++) {
            if (m_Turret[i] != null)
                m_Turret[i].m_EnemyDeath.KillEnemy();
        }
        BulletManager.SetBulletFreeState(1000);
        
        yield break;
    }
}

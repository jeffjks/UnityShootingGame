using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShipLarge : EnemyUnit, IHasPhase
{
    public EnemyShipLarge_FrontTurret m_FrontTurret;
    public EnemyShipLarge_BackTurret m_BackTurret;
    private int _phase = 1;
    
    void Start()
    {
        CurrentAngle = m_MoveVector.direction;
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (_phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        CurrentAngle = m_MoveVector.direction;
    }

    public void ToNextPhase()
    {
        if (_phase == 2)
            return;
            
        if (m_FrontTurret != null)
            m_FrontTurret.m_EnemyDeath.KillEnemy();
        if (m_BackTurret != null)
            m_BackTurret.m_EnemyDeath.KillEnemy();

        StartPattern("2A", new EnemyShipLarge_BulletPattern_2A(this));
        _phase++;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        yield break;
    }
}

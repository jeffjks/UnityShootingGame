using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyShipLarge : EnemyUnit, IHasPhase
{
    public EnemyShipLarge_FrontTurret m_FrontTurret;
    public EnemyShipLarge_BackTurret m_BackTurret;
    private int _phase = 1;
    
    private void Start()
    {
        CurrentAngle = m_MoveVector.direction;

        if (SystemManager.GameMode != GameMode.Replay)
            m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
    }
    
    protected override void Update()
    {
        base.Update();
        
        CurrentAngle = m_MoveVector.direction * Time.timeScale;
    }

    public void ToNextPhase()
    {
        if (SystemManager.GameMode != GameMode.Replay)
        {
            switch (_phase)
            {
                case 1:
                    if (m_EnemyHealth.HealthRatioScaled > 330) // 체력 33% 이하
                        return;
                    break;
                default:
                    return;
            }
        }

        m_EnemyHealth.WriteReplayHealthData();
        
        _phase++;
        if (m_FrontTurret != null)
            m_FrontTurret.m_EnemyDeath.KillEnemy();
        if (m_BackTurret != null)
            m_BackTurret.m_EnemyDeath.KillEnemy();

        StartPattern("2A", new EnemyShipLarge_BulletPattern_2A(this));
        
        if (SystemManager.GameMode != GameMode.Replay)
            m_EnemyHealth.Action_OnHealthChanged -= ToNextPhase;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        yield break;
    }
}

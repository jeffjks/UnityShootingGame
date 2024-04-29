using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyTankLarge1 : EnemyUnit, IHasPhase
{
    public EnemyTankLarge1_SubTurret[] m_SubTurrets = new EnemyTankLarge1_SubTurret[2];
    public EnemyTankLarge1_BackTurret m_BackTurret;
    public Animator m_RotateAnimator;

    private int _phase;
    private bool _isSubTurretStart;
    private readonly int _rotateAnimationTrigger = Animator.StringToHash("Rotate");

    private void Start()
    {
        SetRotatePattern(new RotatePattern_MoveDirection());

        m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
    }

    public void ToNextPhase()
    {
        switch (_phase)
        {
            case 0:
            case 1:
                if (m_EnemyHealth.HealthRatioScaled > 330) // 체력 33% 이하
                    return;
                break;
            default:
                return;
        }
            
        _phase = 2;
        if (m_SubTurrets[0] != null)
            m_SubTurrets[0].m_EnemyDeath.KillEnemy();
        if (m_SubTurrets[1] != null)
            m_SubTurrets[1].m_EnemyDeath.KillEnemy();
        if (m_BackTurret != null)
            m_BackTurret.m_EnemyDeath.KillEnemy();
                
        m_RotateAnimator.SetTrigger(_rotateAnimationTrigger);
        _isSubTurretStart = true;
        StartPattern("B", new EnemyTankLarge1_BulletPattern_B(this));
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;

        if (_phase == 0) {
            if (Position2D.y < -1f) {
                _phase = 1;
            }
        }

        ActivateSubTurrets();
    }

    private void ActivateSubTurrets()
    {
        if (!_isSubTurretStart) {
            if (Mathf.Abs(m_SubTurrets[0].Position2D.x) <= 7f && Mathf.Abs(m_SubTurrets[1].Position2D.x) <= 7f)
            {
                m_SubTurrets[0].StartPattern("A", new EnemyTankLarge1_BulletPattern_SubTurret_A(m_SubTurrets[0], m_BackTurret));
                m_SubTurrets[1].StartPattern("A", new EnemyTankLarge1_BulletPattern_SubTurret_A(m_SubTurrets[1], null));
                _isSubTurretStart = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyTankLarge1 : EnemyUnit
{
    public EnemyTankLarge1_SubTurret[] m_SubTurrets = new EnemyTankLarge1_SubTurret[2];
    public EnemyTankLarge1_BackTurret m_BackTurret;
    public Transform m_LauncherRotation;
    public Animator m_RotateAnimator;

    private int _phase;
    private bool _isSubTurretStart;
    private readonly int _rotateAnimationTrigger = Animator.StringToHash("Rotate");

    void Start()
    {
        m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
        
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private void ToNextPhase()
    {
        if (m_EnemyHealth.HealthPercent <= 0.33f)
        { // 체력 33% 이하
            m_SubTurrets[0]?.m_EnemyDeath.KillEnemy();
            m_SubTurrets[2]?.m_EnemyDeath.KillEnemy();
            m_BackTurret?.m_EnemyDeath.KillEnemy();

            if (_phase <= 1)
            {
                m_RotateAnimator.SetTrigger(_rotateAnimationTrigger);
                _isSubTurretStart = true;
                StartPattern("B", new EnemyTankLarge1_BulletPattern_B(this));
                _phase = 2;
            }
        }
    }
    
    protected override void Update()
    {
        base.Update();

        if (_phase == 0) {
            if (m_Position2D.y < - 1f) {
                _phase = 1;
            }
        }

        ActivateSubTurrets();
    }

    private void ActivateSubTurrets()
    {
        if (!_isSubTurretStart) {
            if (Mathf.Abs(m_SubTurrets[0].m_Position2D.x) <= 7f && Mathf.Abs(m_SubTurrets[1].m_Position2D.x) <= 7f)
            {
                m_SubTurrets[0].StartPattern("A", new EnemyTankLarge1_BulletPattern_SubTurret_A(m_SubTurrets[0]));
                m_SubTurrets[1].StartPattern("A", new EnemyTankLarge1_BulletPattern_SubTurret_A(m_SubTurrets[1]));
                _isSubTurretStart = true;
            }
        }
    }
}

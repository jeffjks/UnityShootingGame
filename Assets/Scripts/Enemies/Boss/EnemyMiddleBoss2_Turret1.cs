using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2_Turret1 : EnemyUnit
{
    public Animator m_BarrelAnimator;

    void Start()
    {
        RotateUnit(AngleToPlayer);
        
        SetRotatePattern(new RotatePattern_TargetPlayer(45f, 100f));
        StartPattern("0", new BulletPattern_EnemyMiddleBoss2_Turret1_0(this));
    }
}

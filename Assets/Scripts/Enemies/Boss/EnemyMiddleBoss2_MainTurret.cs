using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2_MainTurret : EnemyUnit
{
    public Animator m_BarrelAnimator;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        
        SetRotatePattern(new RotatePattern_TargetPlayer(45f, 100f));
        StartPattern("0", new BulletPattern_EnemyMiddleBoss2_MainTurret_0(this));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4_MainTurret : EnemyUnit
{
    public Animator m_BarrelAnimator;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
    }
}
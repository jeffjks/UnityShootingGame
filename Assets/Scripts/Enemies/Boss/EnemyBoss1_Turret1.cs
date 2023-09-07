using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1_Turret1 : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}
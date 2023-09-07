using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3_Turret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        Action_OnPatternStopped += () => SetRotatePattern(new RotatePattern_TargetPlayer(100f));
    }
}
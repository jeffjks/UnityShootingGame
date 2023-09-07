using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium3_Turret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

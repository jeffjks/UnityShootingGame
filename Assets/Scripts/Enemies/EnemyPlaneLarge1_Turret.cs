using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1_Turret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(24f));
    }
}

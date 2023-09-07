using System.Collections;
using System;
using UnityEngine;

public class EnemyTankLarge1_SubTurret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-48f));
    }
}

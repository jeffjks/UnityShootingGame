using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge2_Turret : EnemyUnit
{
    private readonly IRotatePattern _defaultRotatePattern = new RotatePattern_TargetPlayer(64f, 100f);

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankLarge2_BulletPattern_Turret_A(this, _defaultRotatePattern));
        SetRotatePattern(_defaultRotatePattern);
    }
}

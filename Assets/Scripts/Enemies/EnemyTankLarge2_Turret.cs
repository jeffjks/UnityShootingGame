using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge2_Turret : EnemyUnit
{
    private readonly IRotatePattern _defaultRotatePattern = new RotatePattern_TargetPlayer(64f, 100f);
    private readonly IRotatePattern _stopRotatePattern = new RotatePattern_Stop();

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankLarge2_BulletPattern_Turret_A(this, _defaultRotatePattern, _stopRotatePattern));
        SetRotatePattern(_defaultRotatePattern);
    }
}

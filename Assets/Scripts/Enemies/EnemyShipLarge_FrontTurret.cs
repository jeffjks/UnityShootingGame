using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLarge_FrontTurret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyShipLarge_BulletPattern_FrontTurret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer(50f, 100f));
    }
}

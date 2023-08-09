using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLarge_BackTurret : EnemyUnit
{
    void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyShipLarge_BulletPattern_BackTurret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

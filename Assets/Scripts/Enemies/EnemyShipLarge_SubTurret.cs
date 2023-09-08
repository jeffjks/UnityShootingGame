using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLarge_SubTurret : EnemyUnit
{
    private int _side;

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());

        _side = (transform.localPosition.x > 0f) ? -1 : 1;

        StartPattern("A", new EnemyShipLarge_BulletPattern_SubTurret_A(this, _side));
    }
}

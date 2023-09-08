using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall2_Turret : EnemyUnit
{
    private void Start()
    {
        StartPattern("A", new BulletPattern_EnemyShipSmall2_Turret_A(this));
        RotateUnit(AngleToPlayer);
        SetRotatePattern(new RotatePattern_TargetPlayer(60f, 100f));
    }
}

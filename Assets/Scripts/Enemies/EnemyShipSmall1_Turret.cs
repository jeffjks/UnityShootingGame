using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall1_Turret : EnemyUnit
{
    void Start()
    {
        StartPattern("A", new BulletPattern_EnemyShipSmall1_Turret_A(this));
        RotateUnit(AngleToPlayer);
        SetRotatePattern(new RotatePattern_TargetPlayer(60f, 100f));
    }
}

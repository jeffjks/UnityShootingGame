﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankLarge3_BulletPattern_Turret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1_Turret2 : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3_Turret : EnemyUnit
{
    private void Start()
    {
        Action_OnPatternStopped += () => SetRotatePattern(new RotatePattern_TargetPlayer(100f));
    }
}
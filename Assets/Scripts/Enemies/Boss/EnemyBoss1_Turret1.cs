using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1_Turret1 : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}
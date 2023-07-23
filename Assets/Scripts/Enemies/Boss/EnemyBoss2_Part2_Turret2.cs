using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2_Part2_Turret2 : EnemyUnit
{
    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5b_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

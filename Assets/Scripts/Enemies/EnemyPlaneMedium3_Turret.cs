using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium3_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

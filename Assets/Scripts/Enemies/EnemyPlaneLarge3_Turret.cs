using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3_Turret : EnemyUnit
{
    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

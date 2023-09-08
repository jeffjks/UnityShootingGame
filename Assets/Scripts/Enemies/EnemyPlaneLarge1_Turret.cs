using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(24f));
    }
}

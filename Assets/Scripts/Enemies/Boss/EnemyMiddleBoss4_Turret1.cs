using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4_Turret1 : EnemyUnit
{
    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

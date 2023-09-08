using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4_FrontTurret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

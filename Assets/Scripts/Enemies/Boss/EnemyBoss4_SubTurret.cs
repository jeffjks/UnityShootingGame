using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4_SubTurret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
    }
}
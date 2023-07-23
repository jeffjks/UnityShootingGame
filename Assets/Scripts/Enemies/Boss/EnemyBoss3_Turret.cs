using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3_Turret : EnemyUnit
{
    void Start()
    {
        CurrentAngle = AngleToPlayer;
        _onPatternStopped += () => SetRotatePattern(new RotatePattern_TargetPlayer(100f));
    }
}
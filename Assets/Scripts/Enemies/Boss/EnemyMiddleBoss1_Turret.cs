using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMiddleBoss1_Turret : EnemyUnit
{
    void Start()
    {
        CurrentAngle = 0f;
        SetRotatePattern(new RotatePattern_TargetPlayer(90f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5a_SubTurret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
    }
}

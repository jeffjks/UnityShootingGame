using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret3 : EnemyUnit
{
    protected override void Start()
    {
        base.Start();
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
}

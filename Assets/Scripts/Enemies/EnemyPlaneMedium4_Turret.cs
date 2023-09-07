using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium4_Turret : EnemyUnit
{
    private int _side;

    protected override void Start()
    {
        base.Start();

        _side = transform.localScale.x < 0f ? -1 : 1;
        SetRotatePattern(new RotatePattern_RotateAround(180f * _side));
    }
}

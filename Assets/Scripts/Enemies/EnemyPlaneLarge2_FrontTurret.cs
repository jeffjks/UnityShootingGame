using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2_FrontTurret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        StartPattern("A", new EnemyPlaneLarge2_BulletPattern_FrontTurret_A(this));
    }
}

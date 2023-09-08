using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2_FrontTurret : EnemyUnit
{
    private void Start()
    {
        StartPattern("A", new EnemyPlaneLarge2_BulletPattern_FrontTurret_A(this));
    }
}

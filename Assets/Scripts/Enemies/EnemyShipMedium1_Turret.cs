using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipMedium1_Turret : EnemyUnit
{
    void Start()
    {
        CurrentAngle = -transform.localRotation.eulerAngles.y;
        StartPattern("A", new BulletPattern_EnemyPlaneMedium1_Turret_A(this));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2_Part3_Turret2 : EnemyUnit
{
    protected override void Start()
    {
        base.Start();
        
        CurrentAngle = 0f;
    }
}
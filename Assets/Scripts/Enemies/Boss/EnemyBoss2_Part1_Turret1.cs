using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2_Part1_Turret1 : EnemyUnit
{
    private void Start()
    {
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}
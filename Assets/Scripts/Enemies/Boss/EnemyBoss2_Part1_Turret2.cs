using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2_Part1_Turret2 : EnemyUnit
{
    void Start()
    {
        SetRotatePattern(new RotatePattern_TargetPlayer(180f));
    }
}
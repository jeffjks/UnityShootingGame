using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2_Part3_Turret1 : EnemyUnit
{
    public int Side { private get; set; }

    protected override void Update()
    {
        base.Update();

        CustomDirection += 80f / Application.targetFrameRate * Time.timeScale * Side;
    }
}
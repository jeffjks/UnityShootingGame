using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2_BackTurret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        
        var targetAngle = GetAngleToTarget(transform.root.position, PlayerManager.GetPlayerPosition()); // Special (Parent 기준)
        SetRotatePattern(new RotatePattern_Target_Conditional(targetAngle, 50f,
            () => PlayerManager.IsPlayerAlive, targetAngle, 100f));
        StartPattern("A", new EnemyPlaneLarge2_BulletPattern_BackTurret_A(this));
    }
}

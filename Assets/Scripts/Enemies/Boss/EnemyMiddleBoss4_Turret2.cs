using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4_Turret2 : EnemyUnit
{
    void Start()
    {
        RotateImmediately((Vector2) PlayerManager.GetPlayerPosition() + new Vector2(0f, 1.5f));
        SetRotatePattern(new RotatePattern_CustomTarget(GetCustomTarget));
    }

    private (float, float) GetCustomTarget()
    {
        var targetPos = (Vector2)PlayerManager.GetPlayerPosition() + new Vector2(0f, 1.5f);
        var targetAngle = GetAngleToTarget(m_FirePosition[0].position, targetPos);
        var speed = PlayerManager.IsPlayerAlive ? 0f : 100f;
        return (targetAngle, speed);
    }
}

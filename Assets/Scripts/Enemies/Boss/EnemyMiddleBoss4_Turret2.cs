using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4_Turret2 : EnemyUnit
{
    void Start()
    {
        RotateImmediately((Vector2) PlayerManager.GetPlayerPosition() + new Vector2(0f, 1.5f));
        SetRotatePattern(new RotatePattern_TargetPlayer(0f, 100f).SetOffsetPosition(new Vector2(0f, 1.5f)));
    }
}

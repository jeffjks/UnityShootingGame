using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5a_MainTurret : EnemyUnit
{
    public int PlayerSweepRotatePattern { private get; set; }

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
    }

    protected override void Update()
    {
        base.Update();

        if (PlayerSweepRotatePattern < 0)
        {
            SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-70f));
        }
        else if (PlayerSweepRotatePattern > 0)
        {
            SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(70f));
        }
    }
}

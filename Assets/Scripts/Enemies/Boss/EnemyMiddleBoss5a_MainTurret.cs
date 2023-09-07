using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5a_MainTurret : EnemyUnit
{
    public int PlayerSweepRotatePattern { private get; set; }

    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(150f, 100f));
    }

    protected override void Update()
    {
        base.Update();

        if (PlayerSweepRotatePattern < 0)
        {
            SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-70f));
            var deltaAngle = Mathf.Abs(Mathf.DeltaAngle(CurrentAngle, AngleToPlayer - 70f));
            if (deltaAngle < 1f)
                PlayerSweepRotatePattern = 1;
        }
        else if (PlayerSweepRotatePattern > 0)
        {
            SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(70f));
            var deltaAngle = Mathf.Abs(Mathf.DeltaAngle(CurrentAngle, AngleToPlayer + 70f));
            if (deltaAngle < 1f)
                PlayerSweepRotatePattern = -1;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShipMedium1 : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = m_MoveVector.direction;
        StartPattern("A", new BulletPattern_EnemyPlaneMedium1_A(this));
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
}

public class BulletPattern_EnemyPlaneMedium1_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyPlaneMedium1_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2200, 2000, 1500 };
        var accel = new BulletAccel(6f, 800);

        while(true)
        {
            for (var i = 0; i < 2; i++)
            {
                var pos = GetFirePos(i);
                var dir = Random.Range(0f, 360f);
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 2f, BulletPivot.Fixed, dir, accel, 12, 30f));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 2f, BulletPivot.Fixed, dir, accel, 24, 15f));
                }
                else
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 2f, BulletPivot.Fixed, dir, accel, 30, 12f));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyPlaneMedium1_Turret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyPlaneMedium1_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while(true) {
            for (var i = 0; i < 3; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 4.3f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(100);
            }
            yield return new WaitForMillisecondFrames(1600);
        }
        //onCompleted?.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTurret2_Turret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTurret2_BulletPattern_Turret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer(90f, 100f));
    }
}

public class EnemyTurret2_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyTurret2_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1250, 500, 250 };
        float[] speedArray = { 5.7f, 6.8f, 6.8f };
        
        yield return new WaitForMillisecondFrames(Random.Range(0, fireDelay[(int) SystemManager.Difficulty]));
        while(true)
        {
            var pos = GetFirePos(0);
            var speed = speedArray[(int)SystemManager.Difficulty];
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Current, 0f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

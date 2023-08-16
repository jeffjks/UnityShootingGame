using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTurret1_Turret : EnemyUnit
{
    private readonly IRotatePattern _defaultRotatePattern = new RotatePattern_TargetPlayer(90f, 100f);

    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(_defaultRotatePattern);
        StartPattern("A", new EnemyTurret1_BulletPattern_Turret_A(this, _defaultRotatePattern));
    }
}

public class EnemyTurret1_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    private readonly IRotatePattern _defaultRotatePattern;

    public EnemyTurret1_BulletPattern_Turret_A(EnemyObject enemyObject, IRotatePattern defaultRotatePattern) :
        base(enemyObject)
    {
        _defaultRotatePattern = defaultRotatePattern;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1600, 800, 400 };
        int[] subFireDelay = { 5, 5, 5 };
        float[] speedArray = { 5.5f, 6f, 6.5f };
        yield return new WaitForMillisecondFrames(Random.Range(0, 1000));
        
        while(true)
        {
            var speed = speedArray[(int)SystemManager.Difficulty];
            for (int i = 0; i < 4; i++)
            {
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                var enemyBullets = CreateBullet(new BulletProperty(pos0, BulletImage.BlueSmall, speed, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, speed, BulletPivot.Current, 0f));

                if (enemyBullets.Count == 0)
                {
                    break;
                }
                _enemyObject.SetRotatePattern(new RotatePattern_Stop());
                
                yield return new WaitForFrames(subFireDelay[(int) SystemManager.Difficulty]);
            }
            _enemyObject.SetRotatePattern(_defaultRotatePattern);
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
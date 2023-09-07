using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankMedium1_Turret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankMedium1_BulletPattern_Turret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer(32f, 100f));
    }
}

public class EnemyTankMedium1_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyTankMedium1_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const float gap = 0.07f;
        yield return new WaitForMillisecondFrames(Random.Range(0, 1500));
        
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7f, BulletPivot.Current, 0f, 4, 20f));
                yield return new WaitForMillisecondFrames(2000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var pos1 = GetFirePos(0, -gap);
                var pos2 = GetFirePos(0, gap);
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 6.0f, BulletPivot.Current, -1f, 3 ,20f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 7.1f, BulletPivot.Current, -1f, 4, 20f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 6.0f, BulletPivot.Current, 1f, 3 ,20f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 7.1f, BulletPivot.Current, 1f, 4, 20f));
                yield return new WaitForMillisecondFrames(800);
            }
            else
            {
                var pos1 = GetFirePos(0, -gap);
                var pos2 = GetFirePos(0);
                var pos3 = GetFirePos(0, gap);
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, -3f, 3, 20f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 7.3f, BulletPivot.Current, -3f, 4, 20f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, 0f, 3, 20f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 7.3f, BulletPivot.Current, 0f, 4, 20f));
                CreateBullet(new BulletProperty(pos3, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, 3f, 3, 20f));
                CreateBullet(new BulletProperty(pos3, BulletImage.BlueSmall, 7.3f, BulletPivot.Current, 3f, 4, 20f));
                yield return new WaitForMillisecondFrames(800);
            }

            if (SystemManager.Difficulty != GameDifficulty.Normal)
            {
                for (int i = 0; i < 6; i++) {
                    var pos1 = GetFirePos(0, -gap);
                    var pos2 = GetFirePos(0, gap);
                    var dir1 = Random.Range(-1f, 0f);
                    var dir2 = Random.Range(0f, 1f);
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f + i, BulletPivot.Current, dir1));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f + i, BulletPivot.Current, dir2));
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(1000);
            }
        }
        //onCompleted?.Invoke();
    }
}
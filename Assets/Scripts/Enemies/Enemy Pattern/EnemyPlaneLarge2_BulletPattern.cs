using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneLarge2_BulletPattern_1A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge2_BulletPattern_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(1000);

        while(true)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                break;
            }
            if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 16; i++) {
                    var pos = GetFirePos(0);
                    var dir = 115f - 4.8f * i;
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 10f, BulletPivot.Player, 0f, 2, dir));
                    yield return new WaitForFrames(3);
                }
            }
            else {
                for (int i = 0; i < 20; i++) {
                    var pos = GetFirePos(0);
                    var dir1 = 120f - 5.3f * i;
                    var dir2 = 120f - 4.8f * i;
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 9.5f, BulletPivot.Player, 0f, 2, dir1));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 13f, BulletPivot.Player, 0f, 2, dir2));
                    yield return new WaitForFrames(3);
                }
            }
            yield return new WaitForMillisecondFrames(3000);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge2_BulletPattern_FrontTurret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge2_BulletPattern_FrontTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 1500, 1000 };
        
        while(true)
        {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.4f, BulletPivot.Current, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.5f, BulletPivot.Current, 0f, 3, 16f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.6f, BulletPivot.Current, 0f, 3, 16f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge2_BulletPattern_BackTurret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge2_BulletPattern_BackTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2500, 1900, 1400 };

        while(true)
        {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.9f, BulletPivot.Current, 0f, 2, 13f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f, BulletPivot.Current, 0f, 2, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.3f, BulletPivot.Current, 0f, 2, 12f));
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f, BulletPivot.Current, -12f, 2, 8f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f, BulletPivot.Current, 12f, 2, 8f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.3f, BulletPivot.Current, -12f, 2, 8f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.3f, BulletPivot.Current, 12f, 2, 8f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
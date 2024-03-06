using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankLarge3 : EnemyUnit
{
    private void Start()
    {
        StartPattern("A", new EnemyTankLarge3_BulletPattern_A(this));
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
}

public class EnemyTankLarge3_BulletPattern_A : BulletFactory, IBulletPattern
{
    public EnemyTankLarge3_BulletPattern_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1000, 550, 250 };

        while(true)
        {
            for (var i = 0; i < 2; i++)
            {
                var pos = GetFirePos(i);
                var targetAngle = _enemyObject.AngleToPlayer;
                
                for (var j = 0; j < 5; j++)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8f, BulletPivot.Fixed, targetAngle));
                    yield return new WaitForFrames(3);
                }
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyTankLarge3_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyTankLarge3_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 2000 - 300, 1500 - 200, 1200 - 200 };
        
        while(true)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                int[] nums = { 3, 4, 5, 4 };
                foreach (var num in nums)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Current, 0f, num, 19.5f));
                    yield return new WaitForMillisecondFrames(300);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                int[] nums = { 3, 4, 5, 6, 5 };
                foreach (var num in nums)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Current, 0f, num, 16f));
                    yield return new WaitForMillisecondFrames(200);
                }
            }
            else
            {
                int[] nums = { 4, 5, 6, 7, 6, 7 };
                foreach (var num in nums)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Current, 0f, num, 16f));
                    yield return new WaitForMillisecondFrames(200);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
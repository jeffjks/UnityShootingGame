using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss4_Turret1_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_Turret1_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1000, 600, 500 };
        while (true) {
            var pos = GetFirePos(0);
            var dir = Random.Range(-1.5f, 1.5f);
            var interval = Random.Range(0.5f, 14f);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Player, dir, 5, interval));
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Player, dir, 7, interval));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_Turret1_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_Turret1_1B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2300, 1800, 1300 };
        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 6; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 3.5f + i*1.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(70);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 8; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 3.5f + i*1.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(70);
                }
            }
            else {
                for (int i = 0; i < 10; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 3.5f + i*1.2f, BulletPivot.Current, 0f, 3, 3f));
                    yield return new WaitForMillisecondFrames(70);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_Turret2_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_Turret2_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var random_value = Random.Range(0, 2);

            if (random_value == 0) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    for (int i = 0; i < 5; i++)
                    {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.8f + 0.82f*i, BulletPivot.Current, 0f, 7, 14f));
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(1200);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    for (int i = 0; i < 5; i++)
                    {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.8f + 0.82f*i, BulletPivot.Current, 0f, 9, 11f));
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(1000);
                }
                else {
                    for (int i = 0; i < 5; i++) {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.8f + 0.82f*i, BulletPivot.Current, 0f, 11, 8f));
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(800);
                }
            }
            else {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    for (int i = 0; i < 3; i++) {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, -2f, 5, 17f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 0, 5, 17f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 2f, 5, 17f));
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(1300);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    for (int i = 0; i < 3; i++) {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, -1.5f, 7, 13f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 0, 7, 13f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 1.5f, 7, 13f));
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(1100);
                }
                else {
                    for (int i = 0; i < 3; i++) {
                        var pos = GetFirePos(0);
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, -1.5f, 7, 13f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 0, 7, 13f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.4f, BulletPivot.Current, 1.5f, 7, 13f));
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(800);
                }
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_Turret2_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_Turret2_1B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1000, 400, 320 };
        
        while (true)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                for (var i = 0; i < 5; ++i)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.3f - 0.7f*i, BulletPivot.Current, 0f, 2, 80f - 18f*i));
                    yield return new WaitForFrames(2);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (var i = 0; i < 5; ++i)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.3f - 0.7f*i, BulletPivot.Current, 0f, 2, 80f - 18f*i));
                    yield return new WaitForFrames(2);
                }
            }
            else {
                for (var i = 0; i < 8; ++i)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8f - 0.5f*i, BulletPivot.Current, 0f, 2, 90f - 12f*i));
                    yield return new WaitForFrames(2);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
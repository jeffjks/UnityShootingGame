using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss3_1A1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_1A1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const int timer = 1250;
        int[] fireDelay = { 1150, 500, 300 };
        var accel = new BulletAccel(0.1f, timer);

        while (true)
        {
            var dir1 = Random.Range(-70f, 70f);
            var dir2 = Random.Range(-3f, 3f);

            var pos = GetFirePos(2);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var property = new BulletProperty(pos, BulletImage.BlueLarge, 10f, BulletPivot.Player, dir1, accel);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, timer);
                var newProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 6f, BulletPivot.Player, dir2);
                CreateBullet(property, spawnTiming, newProperty);
            }
            else if (SystemManager.Difficulty >= GameDifficulty.Expert) {
                var property = new BulletProperty(pos, BulletImage.BlueLarge, 10f, BulletPivot.Player, dir1, accel);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, timer);
                for (int i = 0; i < 4; i++) {
                    var newProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 5.6f + 0.4f*i, BulletPivot.Player, dir2);
                    CreateBullet(property, spawnTiming, newProperty);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss3_1A2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_1A2(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 430, 270, 250 };

        while (true)
        {
            var pos = GetFirePos(1);
            var dir = Random.Range(0f, 360f);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (var i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Player, 32f * i, 3, 3f));
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (var i = 0; i < 5; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Player, 25f * i, 4, 3f));
                }
            }
            else {
                for (var i = 0; i < 5; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.9f, BulletPivot.Player, 25f * i, 4, 3f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Player, 25f * i, 4, 3f));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss3_1B1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_1B1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] repeatNum = { 1, 3, 3 };
        var accel = new BulletAccel(8.7f, 1200);

        for (int i = 0; i < repeatNum[(int)SystemManager.Difficulty]; i++)
        {
            var pos = GetFirePos(2);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.1f, BulletPivot.Player, 0f, 7, 18f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.9f, BulletPivot.Player, 0f, 7, 18f));
                yield return new WaitForMillisecondFrames(1200);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.1f, BulletPivot.Player, 0f, 11, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.9f, BulletPivot.Player, 0f, 11, 12f));
                yield return new WaitForMillisecondFrames(800);
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.1f, BulletPivot.Player, 0f, 13, 10f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.2f, BulletPivot.Player, 0f, accel, 14, 10f));
                yield return new WaitForMillisecondFrames(800);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss3_1B2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_1B2(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = GetFirePos(1);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var dir = Random.Range(-45f, 45f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.3f, BulletPivot.Player, dir));
                yield return new WaitForMillisecondFrames(80);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var dir = Random.Range(-40f, 40f);
                var speed = Random.Range(5f, 5.8f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Player, dir));
                yield return new WaitForMillisecondFrames(40);
            }
            else {
                var dir = Random.Range(-40f, 40f);
                var speed = Random.Range(5f, 6f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Player, dir));
                yield return new WaitForMillisecondFrames(20);
            }
        }
        //onCompleted?.Invoke();
    }
}
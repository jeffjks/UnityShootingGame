using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss3_1A1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_1A1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
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
                var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 6f, BulletPivot.Player, dir2);
                CreateBullet(property, spawnTiming, subProperty);
            }
            else if (SystemManager.Difficulty >= GameDifficulty.Expert) {
                var property = new BulletProperty(pos, BulletImage.BlueLarge, 10f, BulletPivot.Player, dir1, accel);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, timer);
                for (int i = 0; i < 4; i++) {
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 5.6f + 0.4f*i, BulletPivot.Player, dir2);
                    CreateBullet(property, spawnTiming, subProperty);
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
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 430, 270, 250 };

        while (true)
        {
            var pos = GetFirePos(1);
            var dir = Random.Range(0f, 360f);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (var i = 0; i < 4; i++) {
                    var property = new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Fixed, dir + 32f * i, 3, 3f);
                    CreateBullet(property);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (var i = 0; i < 5; i++) {
                    var property1 = new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Fixed, dir + 25f * i, 4, 3f);
                    var property2 = new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Fixed, dir + 25f * i + 180f, 4, 3f);
                    CreateBullet(property1);
                    CreateBullet(property2);
                }
            }
            else {
                for (var i = 0; i < 5; i++) {
                    var property1 = new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Fixed, dir + 25f * i, 4, 3f);
                    var property2 = new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Fixed, dir + 25f * i, 4, 3f);
                    var property3 = new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Fixed, dir + 25f * i + 180f, 4, 3f);
                    var property4= new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Fixed, dir + 25f * i + 180f, 4, 3f);
                    CreateBullet(property1);
                    CreateBullet(property2);
                    CreateBullet(property3);
                    CreateBullet(property4);
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
        yield return new WaitForEndOfFrame();
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
        yield return new WaitForEndOfFrame();
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

public class BulletPattern_EnemyMiddleBoss3_2A1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_2A1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 120, 80, 60 };
        yield return new WaitForMillisecondFrames(1500);

        while (true)
        {
            var pos = GetFirePos(0);
            var dir = _enemyObject.m_CustomDirection[0];
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Fixed, dir, 4, 90f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Fixed, dir, 5, 72f));
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Fixed, dir, 6, 60f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss3_2A2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss3_2A2(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 330, 250, 220 };
        float[] speedScale = { 1f, 1.1f, 1.2f };
        yield return new WaitForMillisecondFrames(1500);

        while (true) {
            var pos = GetFirePos(0);
            var dir = _enemyObject.m_CustomDirection[1];
            var curSpeedScale = speedScale[(int)SystemManager.Difficulty];

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.4f * curSpeedScale, BulletPivot.Fixed, -dir, 6, 60f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f * curSpeedScale, BulletPivot.Fixed, -dir, 6, 60f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.9f * curSpeedScale, BulletPivot.Fixed, -dir + 1.5f, 6, 60f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.4f * curSpeedScale, BulletPivot.Fixed, -dir + 3.5f, 6, 60f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.9f * curSpeedScale, BulletPivot.Fixed, -dir + 6f, 6, 60f));
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f * curSpeedScale, BulletPivot.Fixed, -dir, 8, 45f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.9f * curSpeedScale, BulletPivot.Fixed, -dir + 1.5f, 8, 45f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.4f * curSpeedScale, BulletPivot.Fixed, -dir + 3.5f, 8, 45f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.9f * curSpeedScale, BulletPivot.Fixed, -dir + 6f, 8, 45f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss4_MainTurret_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_MainTurret_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
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

public class BulletPattern_EnemyMiddleBoss4_MainTurret_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_MainTurret_1B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
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

public class BulletPattern_EnemyMiddleBoss4_MainTurret_2A1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_MainTurret_2A1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 600, 350, 250 };
        while (true)
        {
            var pos = GetFirePos(0);
            var dir = Random.Range(-12f, 12f);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 3.6f, BulletPivot.Current, dir, 5, 24f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir - 1f, 4, 24f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir, 4, 24f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir + 1f, 4, 24f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 3.6f, BulletPivot.Current, dir, 6, 19.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir - 1.5f, 5, 19.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir - 0.5f, 5, 19.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir + 0.5f, 5, 19.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir + 1.5f, 5, 19.2f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 3.6f, BulletPivot.Current, dir, 7, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir - 1.5f, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir - 0.5f, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir + 0.5f, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Current, dir + 1.5f, 6, 16f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_MainTurret_2A2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_MainTurret_2A2(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1500, 1000, 800 };
        while (true)
        {
            var pos = GetFirePos(0);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f + 0.4f*i, BulletPivot.Current, 0f, 6, 60f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f + 0.4f*i, BulletPivot.Current, 30f, 6, 60f));
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 3; i++)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f + 0.4f*i, BulletPivot.Current, 0f, 10, 36f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f + 0.4f*i, BulletPivot.Current, 18f, 10, 36f));
                }
            }
            else {
                for (int i = 0; i < 3; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f + 0.4f*i, BulletPivot.Current, 0f, 10, 36f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f + 0.4f*i, BulletPivot.Current, 18f, 10, 36f));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_BackTurret_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_BackTurret_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
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

public class BulletPattern_EnemyMiddleBoss4_BackTurret_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_BackTurret_1B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
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

public class BulletPattern_EnemyMiddleBoss4_PartA : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_PartA(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            var pos = GetFirePos(0);
            var dir = _enemyObject.m_CustomDirection[0];
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.8f, BulletPivot.Fixed, dir, 1, 90f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.2f, BulletPivot.Fixed, dir, 1, 90f));
        }
        else {
            for (int i = 0; i < 4; i++) {
                var pos = GetFirePos(0);
                var dir = _enemyObject.m_CustomDirection[0];
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.8f, BulletPivot.Fixed, dir, 4, 90f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.2f, BulletPivot.Fixed, dir, 4, 90f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss4_PartB : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss4_PartB(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        const int timer = 300;
        BulletAccel accel = new BulletAccel(0f, timer);
        int[] fireDelay = { 300, 200, 100 };

        while (true) {
            var pos = GetFirePos(0);
            var dir = Random.Range(0f, 360f);
            var property = new BulletProperty(pos, BulletImage.BlueSmall, 3.2f, BulletPivot.Fixed, dir, accel);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, timer);
            var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 8f, BulletPivot.Fixed, 0f);
            CreateBullet(property, spawnTiming, subProperty);
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
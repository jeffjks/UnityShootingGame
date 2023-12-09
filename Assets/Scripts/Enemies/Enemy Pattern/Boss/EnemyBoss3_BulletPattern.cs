using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyBoss3_1A1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1A1(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var frame = 0;
        int[] fireDelay = { 12, 8, 6 };
        var frameTotal = 192;

        while (frame < frameTotal) {
            var pos = GetFirePos(0);
            frame += fireDelay[(int) SystemManager.Difficulty];
            var dir = _enemyObject.m_CustomDirection[0];
            switch(frame / (frameTotal / 3))
            {
                case 0:
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 7.4f, BulletPivot.Fixed, dir));
                    break;
                case 1:
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 7.4f, BulletPivot.Fixed, dir, 2, 24f));
                    break;
                case 2:
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 7.4f, BulletPivot.Fixed, dir, 3, 24f));
                    break;
            }
            yield return new WaitForFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_1A2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1A2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var frame = 0;
        int[] fireDelay = { 18, 12, 9 };
        var frameTotal = 192;

        while (frame < frameTotal) {
            var pos = GetFirePos(0);
            frame += fireDelay[(int) SystemManager.Difficulty];
            var dir = _enemyObject.m_CustomDirection[0];
            switch(frame / (frameTotal / 3)) {
                case 0:
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir, 3, 2.25f));
                    break;
                case 1:
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir - 14f, 3, 2.25f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir + 14f, 3, 2.25f));
                    break;
                case 2:
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir - 28f, 3, 2.25f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir, 3, 2.25f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Fixed, dir + 28f, 3, 2.25f));
                    break;
            }
            yield return new WaitForFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1000, 500, 330 };
        
        yield return new WaitForMillisecondFrames(1500);

        for (int j = 0; j < 4; j++) {
            var duration = 0;
            while (duration < 1600) {
                duration += fireDelay[(int) SystemManager.Difficulty];
                Vector3 pos = GetFirePos(0);
                for (int i = 0; i <= j; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f + i*0.6f, BulletPivot.Player, 0f));
                    if (SystemManager.Difficulty == GameDifficulty.Normal) {
                        break;
                    }
                }
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_Turret_1B : BulletFactory, IBulletPattern
{
    private readonly int _side;

    public BulletPattern_EnemyBoss3_Turret_1B(EnemyObject enemyObject, int side) : base(enemyObject)
    {
        _side = side;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(1500);
        
        _enemyObject.SetRotatePattern(new RotatePattern_RotateAround_PingPong(_enemyObject.CurrentAngle, 0f, 160f, 100f*_side));

        while(true)
        {
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, Random.Range(-1f, 1f), 8, 20.6f));
                yield return new WaitForMillisecondFrames(640);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, Random.Range(-3f, 3f), 11, 15f));
                yield return new WaitForMillisecondFrames(320);
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, Random.Range(-5f, 5f), 13, 13f));
                yield return new WaitForMillisecondFrames(270);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_1C1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1C1(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 210, 120, 80 };

        while (true) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 7.4f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], 8, 45f));
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueSmall, 7.4f, BulletPivot.Fixed, -_enemyObject.m_CustomDirection[0], 8, 45f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_1C2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1C2(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        Vector3 pos;
        yield return new WaitForMillisecondFrames(1000);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while (true) {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while (true) {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 30, 12f));
                yield return new WaitForMillisecondFrames(220);
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 36, 10f));
                yield return new WaitForMillisecondFrames(2000);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Hell) {
            while (true) {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 30, 12f));
                yield return new WaitForMillisecondFrames(220);
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 36, 10f));
                yield return new WaitForMillisecondFrames(220);
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), 30, 12f));
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_1D : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_1D(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(7.3f, 1500);
        Vector3 pos1 = GetFirePos(1);
        Vector3 pos2 = GetFirePos(2);
        var property1 = new BulletProperty(pos1, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, 0f);
        var property2 = new BulletProperty(pos2, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, 0f);
        var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 0.1f, BulletPivot.Current, 0f, accel, 2, 180f);

        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, Random.Range(0, 100), new Vector2Int(200, 300));
            CreateBullet(property1, spawnTiming, subProperty);
            CreateBullet(property2, spawnTiming, subProperty);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, Random.Range(0, 100), new Vector2Int(100, 150));
            CreateBullet(property1, spawnTiming, subProperty);
            CreateBullet(property2, spawnTiming, subProperty);
        }
        else {
            var property3 = new BulletProperty(pos1, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, 40f);
            var property4 = new BulletProperty(pos2, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, -40f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, Random.Range(0, 100), new Vector2Int(100, 140));
            CreateBullet(property1, spawnTiming, subProperty);
            CreateBullet(property2, spawnTiming, subProperty);
            CreateBullet(property3, spawnTiming, subProperty);
            CreateBullet(property4, spawnTiming, subProperty);
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss3_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_2A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(0.5f, 1200);
        int[] fireDelay = { 6, 2, 1 };
        var duration = 0;

        while (duration < 300) {
            var pos = GetFirePos(0);
            var property1 = new BulletProperty(pos, BulletImage.BlueNeedle, Random.Range(8f, 9f), BulletPivot.Fixed, Random.Range(0f, 360f), accel);
            var property2 = new BulletProperty(pos, BulletImage.BlueNeedle, Random.Range(9f, 10f), BulletPivot.Fixed, Random.Range(0f, 360f), accel);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 1200);
            var newProperty1 = new BulletProperty(pos, BulletImage.PinkLarge, 3.2f, BulletPivot.Fixed, Random.Range(0f, 360f), 2, Random.Range(0f, 360f));
            var newProperty2 = new BulletProperty(pos, BulletImage.PinkSmall, 2.1f, BulletPivot.Fixed, Random.Range(0f, 360f), 3, Random.Range(0f, 360f));
            CreateBullet(property1, spawnTiming, newProperty1);
            CreateBullet(property1, spawnTiming, newProperty1);
            CreateBullet(property2, spawnTiming, newProperty2);
            CreateBullet(property2, spawnTiming, newProperty2);
            
            duration += fireDelay[(int)SystemManager.Difficulty];
            yield return new WaitForFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_2B1 : BulletFactory, IBulletPattern // Pink Wall
{
    private readonly UnityAction _action;

    public BulletPattern_EnemyBoss3_2B1(EnemyObject enemyObject, UnityAction action) : base(enemyObject)
    {
        _action = action;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        Vector3 pos1, pos2;
        int total_duration = 1400, duration, fireDelay = 80;

        while (true) {
            for (int i = 0; i < 3; i++) {
                duration = 0;
                while (duration < total_duration) {
                    pos1 = GetFirePos(1);
                    pos2 = GetFirePos(2);

                    if (SystemManager.Difficulty == GameDifficulty.Normal) {
                        CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 8f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0] - 20f, 4, 50f));
                        CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 8f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0] + 20f, 4, 50f));
                    }
                    else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                        CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 8.3f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], 10, 24f));
                        CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 8.3f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], 10, 24f));
                    }
                    else {
                        CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 8.3f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], 12, 20f));
                        CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 8.3f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], 12, 20f));
                    }
                    duration += fireDelay;
                    yield return new WaitForMillisecondFrames(fireDelay);
                }
                _action?.Invoke();
            }
            yield return new WaitForMillisecondFrames(600);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_2B2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss3_2B2(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(4.2f, 900);
        int[] fireDelay = { 100, 64, 50 };
        var duration = 0;
        
        while (duration < 2000) {
            Vector3 pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8f, BulletPivot.Player, Random.Range(-40f, 40f), accel, 2, 8f));
            
            duration += fireDelay[(int)SystemManager.Difficulty];
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss3_Turret_2B : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;

    public BulletPattern_EnemyBoss3_Turret_2B(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }
    
    public BulletPattern_EnemyBoss3_Turret_2B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1800, 1800 };
        yield return new WaitForMillisecondFrames(1000);

        if (_patternIndex == 0)
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty] / 2);

        while(true) {
            var pos = GetFirePos(0);
            var random_value = Random.Range(-3f, 3f);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 3; i++)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f + i*0.7f, BulletPivot.Current, random_value));
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f + i*0.7f, BulletPivot.Current, random_value));
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            }
            else {
                for (int i = 0; i < 4; i++)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f + i*0.7f, BulletPivot.Current, random_value, 3, 18f));
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            }
        }
        //onCompleted?.Invoke();
    }
}
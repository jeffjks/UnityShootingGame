using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyBoss2_Part1_Turret1_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part1_Turret1_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        Vector3 pos;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 3; i++)
            {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.4f, BulletPivot.Current, 0f, 5, 22.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.2f, BulletPivot.Current, 0f, 6, 22.5f));
                if (i > 1)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6f, BulletPivot.Current, 0f, 5, 22.5f));
                yield return new WaitForMillisecondFrames(1200);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 4; i++) {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5f, BulletPivot.Current, 0f, 8, 15f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Current, 0f, 7, 15f));
                if (i > 0)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, 0f, 8, 15f));
                if (i > 1)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.8f, BulletPivot.Current, 0f, 7, 15f));
                if (i > 2)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.4f, BulletPivot.Current, 0f, 8, 15f));
                yield return new WaitForMillisecondFrames(900 + 100*i);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5f, BulletPivot.Current, 0f, 13, 10f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Current, 0f, 12, 10f));
                if (i > 0)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, 0f, 13, 10f));
                if (i > 1)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.8f, BulletPivot.Current, 0f, 12, 10f));
                if (i > 2)
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.4f, BulletPivot.Current, 0f, 13, 10f));
                yield return new WaitForMillisecondFrames(900 + 100*i);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part1_Turret1_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part1_Turret1_1B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1600, 900, 500 };
        const float gap = 0.32f;
        while(true) {
            for (int i = 0; i < 3; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.PinkLarge, 5.3f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 5.3f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkLarge, 5.3f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(90);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part1_Turret2_0 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part1_Turret2_0(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            if (PlayerManager.GetPlayerPosition().y >= -5.8f) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 6.6f, BulletPivot.Current, 0f, 8, 2.5f));
            }
            yield return new WaitForMillisecondFrames(500);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part1_Turret2_1A : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;

    public BulletPattern_EnemyBoss2_Part1_Turret2_1A(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1500, 1000 };
        if (_patternIndex == 1)
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty] / 2);
        
        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 2; i++)
                {
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6f + i*0.5f, BulletPivot.Current, 0f));
                }
            }
            else {
                for (int i = 0; i < 4; i++)
                {
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6.5f + i*0.5f, BulletPivot.Current, 0f));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part1_Turret2_1B : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;
    
    public BulletPattern_EnemyBoss2_Part1_Turret2_1B(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _enemyObject.SetRotatePattern(new RotatePattern_TargetAngle(90f + 90f * _patternIndex, 180f));
        yield return new WaitForMillisecondFrames(1200);

        var sign = Mathf.Sign(_enemyObject.transform.localScale.x);
        _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(360f*sign));

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while (true) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6f, BulletPivot.Current, 0f, 2, 18f));
                yield return new WaitForMillisecondFrames(160);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while (true) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6f, BulletPivot.Current, 0f, 5, 14f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Hell) {
            while (true) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 5.8f, BulletPivot.Current, 0f, 6, 12f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6.6f, BulletPivot.Current, 0f, 6, 12f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part1_Turret2_1C : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;
    
    public BulletPattern_EnemyBoss2_Part1_Turret2_1C(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const float gap = 0.14f;
        float speed1 = 1.5f, speed2 = 7f;
        
        BulletAccel accel1 = new BulletAccel(speed1, 400);
        BulletAccel accel2 = new BulletAccel(speed2, 700);
        
        _enemyObject.SetRotatePattern(new RotatePattern_TargetAngle(90f + 90f * _patternIndex, 180f));
        yield return new WaitForMillisecondFrames(1500);

        var sign = Mathf.Sign(_enemyObject.transform.localScale.x);
        _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(850f*sign));

        if (_patternIndex == 1) {
            yield return new WaitForMillisecondFrames(400);
        }

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 18; i++)
            {
                var property1 = new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 6.2f, BulletPivot.Current, 0f, accel1);
                var property2 = new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 6.2f, BulletPivot.Current, 0f, accel1);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 400);
                var subProperty = new BulletProperty(CurrentPos, BulletImage.PinkNeedle, 8f, BulletPivot.Current, 15f, accel2);
                CreateBullet(property1, spawnTiming, subProperty);
                CreateBullet(property2, spawnTiming, subProperty);
                yield return new WaitForFrames(2);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 36; i++) {
                var property1 = new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 8f, BulletPivot.Current, 0f, accel1, 2, 2f);
                var property2 = new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 8f, BulletPivot.Current, 0f, accel1, 2, 2f);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 400);
                var subProperty = new BulletProperty(CurrentPos, BulletImage.PinkNeedle, 8f, BulletPivot.Current, 15f, accel2);
                CreateBullet(property1, spawnTiming, subProperty);
                CreateBullet(property2, spawnTiming, subProperty);
                yield return new WaitForFrames(1);
            }
        }
        else {
            for (int i = 0; i < 36; i++) {
                var property1 = new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 8.5f, BulletPivot.Current, 0f, accel1, 2, 6f);
                var property2 = new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 10f, BulletPivot.Current, 0f, accel1, 2, 6f);
                var spawnTiming1 = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 360);
                var spawnTiming2 = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 560);
                var newProperty1 = new BulletProperty(CurrentPos, BulletImage.PinkNeedle, 8f, BulletPivot.Current, 15f, accel2);
                var newProperty2 = new BulletProperty(CurrentPos, BulletImage.PinkNeedle, 8f, BulletPivot.Current, -9f, accel2);
                CreateBullet(property1, spawnTiming1, newProperty1);
                CreateBullet(property1, spawnTiming2, newProperty2);
                CreateBullet(property2, spawnTiming1, newProperty1);
                CreateBullet(property2, spawnTiming2, newProperty2);
                yield return new WaitForFrames(1);
            }
        }

        _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(180f));
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part2_Turret1_2B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part2_Turret1_2B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int duration = 800;
        BulletAccel accel1 = new BulletAccel(3f, duration);
        BulletAccel accel2 = new BulletAccel(7.6f, 600);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            int[] num = { 1, 2, 3, 3 };
           foreach (var repeatNum in num) {
                for (int i = 0; i < repeatNum; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13.7f, BulletPivot.Current, 0f, accel1, 7, 18f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 800);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 3f, BulletPivot.Current, Random.Range(-6f, 6f), accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(300);
                }
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            int[] num = { 3, 5, 12 };
            foreach (var repeatNum in num) {
                for (int i = 0; i < repeatNum; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13.7f, BulletPivot.Current, 0f, accel1, 13, 12f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 800);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 3f, BulletPivot.Current, Random.Range(-7f, 7f), accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(1200);
            }
        }
        else {
            int[] num = { 3, 5, 12 };
            foreach (var repeatNum in num) {
                for (int i = 0; i < repeatNum; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13.7f, BulletPivot.Current, 0f, accel1, 15, 10f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 800);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 3f, BulletPivot.Current, Random.Range(-8f, 8f), accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(900);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part2_Turret2_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part2_Turret2_2A(EnemyObject enemyObject) : base(enemyObject) {}

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 2; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 3.5f, BulletPivot.Current, 0f, 11, 17f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 4.5f, BulletPivot.Current, 0f, 6, 17f));
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 3; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 3.3f, BulletPivot.Current, 0f, 17, 11f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 4f, BulletPivot.Current, 0f, 14, 11f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 4.7f, BulletPivot.Current, 0f, 11, 11f));
                yield return new WaitForMillisecondFrames(1600);
            }
        }
        else {
            for (int i = 0; i < 3; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 3.3f, BulletPivot.Current, 0f, 24, 8f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 4f, BulletPivot.Current, 0f, 19, 8f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 4.7f, BulletPivot.Current, 0f, 16, 8f));
                yield return new WaitForMillisecondFrames(1600);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part2_Turret3_2A : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;
    
    public BulletPattern_EnemyBoss2_Part2_Turret3_2A(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(5f, 500);
        
        if (_patternIndex == 1)
            yield return new WaitForMillisecondFrames(1500);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, 0f, accel, 3, 32f));
            yield return new WaitForMillisecondFrames(3000);
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, 0f, accel, 3, 32f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (var i = 0; i < 5; ++i) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, -40f+20f*i, accel, 8, 0.8f));
            }
            yield return new WaitForMillisecondFrames(3000);
            for (var i = 0; i < 6; ++i) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, -50f+20f*i, accel, 8, 0.8f));
            }
        }
        else {
            for (var i = 0; i < 5; ++i) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, -40f+20f*i, accel, 10, 0.8f));
            }
            yield return new WaitForMillisecondFrames(3000);
            for (var i = 0; i < 6; ++i) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 2f, BulletPivot.Current, -50f+20f*i, accel, 10, 0.8f));
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part3_Turret1_3A : BulletFactory, IBulletPattern
{
    private readonly UnityAction<int> _action;

    public BulletPattern_EnemyBoss2_Part3_Turret1_3A(EnemyObject enemyObject, UnityAction<int> action) : base(enemyObject)
    {
        _action = action;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const int duration = 600;
        BulletAccel accelInit = new BulletAccel(3f, duration);
        BulletAccel accel2 = new BulletAccel(6.1f, 500); // Normal
        BulletAccel accel3 = new BulletAccel(7f, 500); // Expert, Hell

        var side = Random.Range(0, 2) * 2 - 1;
        _action?.Invoke(side);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 7; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 10, 36f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 3f, BulletPivot.Current, 30f*side, accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(600);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 9; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 20, 18f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 3f, BulletPivot.Current, 30f*side, accel3);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(300);
                }
            }
            else {
                for (int i = 0; i < 11; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 24, 15f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 3f, BulletPivot.Current, 30f*side, accel3);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(220);
                }
            }
            side *= -1;
            _action?.Invoke(side);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part3_Turret1_3B : BulletFactory, IBulletPattern
{
    private readonly UnityAction<int> _action;

    public BulletPattern_EnemyBoss2_Part3_Turret1_3B(EnemyObject enemyObject, UnityAction<int> action) : base(enemyObject)
    {
        _action = action;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const int duration = 600;
        BulletAccel accelInit = new BulletAccel(3f, duration);
        BulletAccel accel2 = new BulletAccel(6.7f, 500);

        var side = Random.Range(0, 2) * 2 - 1;
        _action?.Invoke(side);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 7; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 12, 30f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 3f, BulletPivot.Current, 30f*side, accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(480);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 9; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 24, 15f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 3f, BulletPivot.Current, 30f*side, accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(240);
                }
            }
            else {
                for (int i = 0; i < 11; i++) {
                    var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 13f, BulletPivot.Fixed, _enemyObject.m_CustomDirection[0], accelInit, 30, 12f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, duration);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 3f, BulletPivot.Current, 30f*side, accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                    yield return new WaitForMillisecondFrames(180);
                }
            }
            side *= -1;
            _action?.Invoke(side);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part3_Turret2_3A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part3_Turret2_3A(EnemyObject enemyObject) : base(enemyObject) {}

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2100, 1500, 900 };
        const float gap = 0.07f;

        while(true) {
            _enemyObject.SetRotatePattern(new RotatePattern_Stop());
            for (int i = 0; i < 3; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueSmall, 6.1f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueSmall, 6.1f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(70);
            }
            _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(60f, 100f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss2_Part3_Turret3_3A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss2_Part3_Turret3_3A(EnemyObject enemyObject) : base(enemyObject) {}

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1800, 900, 600 };
        const float gap = 0.32f;

        while(true) {
            CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueSmall, 7.3f, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueSmall, 7.6f, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueSmall, 7.3f, BulletPivot.Current, 0f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
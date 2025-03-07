using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyBoss4_1A1 : BulletFactory, IBulletPattern
{
    private readonly UnityAction<int> _action;

    public BulletPattern_EnemyBoss4_1A1(EnemyObject enemyObject, UnityAction<int> action) : base(enemyObject)
    {
        _action = action;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 3000, 3000, 2000 };
        var index = Random.Range(0, 2);
        yield return new WaitForMillisecondFrames(1000);

        while (true)
        {
            _action?.Invoke(index);
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
            index = 1 - index;
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_1A2 : BulletFactory, IBulletPattern
{
    private readonly UnityAction<int> _action;

    public BulletPattern_EnemyBoss4_1A2(EnemyObject enemyObject, UnityAction<int> action) : base(enemyObject)
    {
        _action = action;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        var index = Random.Range(0, 2);
        int[] repeatNum = { 6, 12, 12 };
        int[] fireDelay = { 1000, 500, 500 };
        
        for (int i = 0; i < repeatNum[(int) SystemManager.Difficulty]; i++)
        {
            _action?.Invoke(index);
            
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            index = 1 - index;
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_1C1 : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_Launcher[] _launchers;

    public BulletPattern_EnemyBoss4_1C1(EnemyObject enemyObject, EnemyBoss4_Launcher[] launchers) : base(enemyObject)
    {
        _launchers = launchers;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1800, 1200, 800 };
        while (true)
        {
            var rand = Random.Range(0, 2);
            _launchers[rand].StartPattern("1C", new BulletPattern_EnemyBoss4_Launcher_1C(_launchers[rand]));
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_1C2 : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_FrontTurret[] _frontTurrets;

    public BulletPattern_EnemyBoss4_1C2(EnemyObject enemyObject, EnemyBoss4_FrontTurret[] frontTurrets) : base(enemyObject)
    {
        _frontTurrets = frontTurrets;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        var rand = Random.Range(0, 2);
        int[] fireDelay = { 3000, 2000, 2000 };
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            if (_frontTurrets[rand] != null) {
                _frontTurrets[rand]?.StartPattern("1C", new BulletPattern_EnemyBoss4_FrontTurret_1C(_frontTurrets[rand]));
            }
            else if (_frontTurrets[1-rand] != null) {
                _frontTurrets[1-rand].StartPattern("1C", new BulletPattern_EnemyBoss4_FrontTurret_1C(_frontTurrets[1-rand]));
            }
            else {
                break;
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
            rand = 1 - rand;
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_1D2 : BulletFactory, IBulletPattern
{
    private readonly UnityAction<int> _action;

    public BulletPattern_EnemyBoss4_1D2(EnemyObject enemyObject, UnityAction<int> action) : base(enemyObject)
    {
        _action = action;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        var index = Random.Range(0, 2);
        int[] repeatNum = { 6, 12, 12 };
        int[] fireDelay = { 800, 400, 320 };
        
        for (int i = 0; i < repeatNum[(int) SystemManager.Difficulty]; i++)
        {
            _action?.Invoke(index);
            
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            index = 1 - index;
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_2A : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_MainTurret _mainTurret;

    public BulletPattern_EnemyBoss4_2A(EnemyObject enemyObject, EnemyBoss4_MainTurret mainTurret) : base(enemyObject)
    {
        _mainTurret = mainTurret;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            int[] repeatNum = { 1, 1, 2, 2, 2 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    _mainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(_mainTurret));
                    yield return new WaitForMillisecondFrames(500);
                }
                yield return new WaitForMillisecondFrames(1000);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            int[] repeatNum = { 1, 2, 2, 3, 3 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    _mainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(_mainTurret));
                    yield return new WaitForMillisecondFrames(400);
                }
                yield return new WaitForMillisecondFrames(900);
            }
        }
        else {
            int[] repeatNum = { 1, 2, 3, 3, 3 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    _mainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(_mainTurret));
                    yield return new WaitForMillisecondFrames(400);
                }
                yield return new WaitForMillisecondFrames(800);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_Launcher_1B : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_Launcher _typedEnemyObject;
    private readonly int _patternIndex;

    public BulletPattern_EnemyBoss4_Launcher_1B(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBoss4_Launcher;
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireFrame = { 9, 7, 5 };
        float[] directionDelta = { 20f, 25f, 30f };
        var durationFrame = 0;
        const int totalDurationFrame = 64;
        _enemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        _typedEnemyObject.CustomDirectionDelta = directionDelta[(int)SystemManager.Difficulty];
        
        while (true)
        {
            var directionChanged = false;
            _typedEnemyObject.CustomDirectionSide = Random.Range(0, 2) * 2 - 1;

            while ((durationFrame / totalDurationFrame) % 2 == _patternIndex)
            {
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    var dir = _enemyObject.m_CustomDirection[0];
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6.1f, BulletPivot.Fixed, dir, 4, 90f));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    var dir = _enemyObject.m_CustomDirection[0];
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6.4f, BulletPivot.Fixed, dir, 5, 72f));
                }
                else
                {
                    var dir = _enemyObject.m_CustomDirection[0];
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueNeedle, 6.8f, BulletPivot.Fixed, dir, 6, 60f));
                }

                if (!directionChanged && (durationFrame % totalDurationFrame) >= totalDurationFrame / 2)
                {
                    _typedEnemyObject.CustomDirectionSide *= -1;
                    directionChanged = true;
                }

                durationFrame += fireFrame[(int)SystemManager.Difficulty];
                yield return new WaitForFrames(fireFrame[(int)SystemManager.Difficulty]);
            }

            while ((durationFrame / totalDurationFrame) % 2 != _patternIndex)
            {
                durationFrame += 1;
                yield return null;
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_Launcher_1C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_Launcher_1C(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        BulletAccel accel = new BulletAccel(4.8f, 500);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 7f, BulletPivot.Fixed, Random.Range(0f, 360f), accel, 24, 15f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 7.6f, BulletPivot.Fixed, Random.Range(0f, 360f), accel, 36, 10f));
        }
        else {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 8f, BulletPivot.Fixed, Random.Range(0f, 360f), accel, 45, 8f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss4_Launcher_2A : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_Launcher _typedEnemyObject;
    private readonly int _patternIndex;

    public BulletPattern_EnemyBoss4_Launcher_2A(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBoss4_Launcher;
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        _typedEnemyObject.CustomDirectionDelta = 0f;
        _typedEnemyObject.CustomDirectionSide = _patternIndex;
        _enemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        
        while (true) {
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var dir1 = (_enemyObject.m_CustomDirection[0] - 1.4f) * _patternIndex;
                var dir2 = _enemyObject.m_CustomDirection[0] * _patternIndex;
                var dir3 = (_enemyObject.m_CustomDirection[0] + 1.4f) * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.3f, BulletPivot.Fixed, dir1, 8, 45f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.5f, BulletPivot.Fixed, dir2, 8, 45f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.7f, BulletPivot.Fixed, dir3, 8, 45f));
                _enemyObject.m_CustomDirection[0] += 12f;
                yield return new WaitForMillisecondFrames(1000 + Random.Range(0, 300));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                var dir1 = (_enemyObject.m_CustomDirection[0] - 1.5f) * _patternIndex;
                var dir2 = _enemyObject.m_CustomDirection[0] * _patternIndex;
                var dir3 = (_enemyObject.m_CustomDirection[0] + 1.5f) * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.25f, BulletPivot.Fixed, dir1, 12, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.5f, BulletPivot.Fixed, dir2, 12, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.75f, BulletPivot.Fixed, dir3, 12, 30f));
                _enemyObject.m_CustomDirection[0] += 10f;
                yield return new WaitForMillisecondFrames(600 + Random.Range(0, 200));
            }
            else {
                var dir1 = (_enemyObject.m_CustomDirection[0] - 2.25f) * _patternIndex;
                var dir2 = (_enemyObject.m_CustomDirection[0] - 0.75f) * _patternIndex;
                var dir3 = (_enemyObject.m_CustomDirection[0] + 0.75f) * _patternIndex;
                var dir4 = (_enemyObject.m_CustomDirection[0] + 2.25f) * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.25f, BulletPivot.Fixed, dir1, 12, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.5f, BulletPivot.Fixed, dir2, 12, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.75f, BulletPivot.Fixed, dir3, 12, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5f, BulletPivot.Fixed, dir4, 12, 30f));
                _enemyObject.m_CustomDirection[0] += 10f;
                yield return new WaitForMillisecondFrames(400 + Random.Range(0, 200));
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_MainTurret_1B1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_MainTurret_1B1(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1000, 500, 450 };
        const float gap = 0.64f;
        
        while (true) {
            var rand = Random.Range(-5f, 5f);
            Vector3 pos0 = GetFirePos(0);
            Vector3 pos1 = GetFirePos(0, gap);
            Vector3 pos2 = GetFirePos(0, -gap);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand - 30f, 2, 18f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand + 30f, 2, 18f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand, 3, 12f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand - 26f, 2, 12f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand + 26f, 2, 12f));
            }
            else {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand, 3, 10f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand - 25f, 3, 10f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand + 25f, 3, 10f));
            }

            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_MainTurret_1B2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_MainTurret_1B2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1500, 850, 600 };
        
        while (true) {
            var rand = Random.Range(-5f, 5f);
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.1f, BulletPivot.Current, rand, 4, 14f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.1f, BulletPivot.Current, rand, 5, 10f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.1f, BulletPivot.Current, rand, 5, 8f));
            }

            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_MainTurret_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_MainTurret_2A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        float rand;

        Vector3 pos = GetFirePos(0);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 6; i++) {
                rand = Random.Range(-5f, 5f);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.3f, BulletPivot.Current, rand - 60f + 20f*i));
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 11; i++) {
                rand = Random.Range(-3f, 3f);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.8f, BulletPivot.Current, rand - 60f + 12f*i));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Current, rand - 60f + 12f*i));
            }
        }
        else {
            for (int i = 0; i < 11; i++) {
                rand = Random.Range(-3f, 3f);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6f, BulletPivot.Current, rand - 60f + 12f*i));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Current, rand - 60f + 12f*i));
            }
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss4_MainTurret_2C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_MainTurret_2C(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        while (true) {
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.79f * 1.2f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.79f * 1.2f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.16f * 1.2f, BulletPivot.Current, 0f, 2, 17.63f*2f*1.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.48f * 1.2f, BulletPivot.Current, 0f, 2, 28.53f*2f*1.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.77f * 1.2f, BulletPivot.Current, 0f, 2, 40.95f*2f*1.5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.16f * 1.2f, BulletPivot.Current, 0f, 2, 56.77f*2f*1.5f));

                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.79f * 1.4f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.5f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.16f * 1.4f, BulletPivot.Current, 0f, 2, 17.63f*2f*1.5f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.48f * 1.4f, BulletPivot.Current, 0f, 2, 28.53f*2f*1.5f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.77f * 1.4f, BulletPivot.Current, 0f, 2, 40.95f*2f*1.5f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.16f * 1.4f, BulletPivot.Current, 0f, 2, 56.77f*2f*1.5f + 5f));
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.79f * 1f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.16f * 1f, BulletPivot.Current, 0f, 2, 17.63f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.48f * 1f, BulletPivot.Current, 0f, 2, 28.53f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.77f * 1f, BulletPivot.Current, 0f, 2, 40.95f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.16f * 1f, BulletPivot.Current, 0f, 2, 56.77f*2f*1.2f + 5f));

                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.79f * 1.2f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.16f * 1.2f, BulletPivot.Current, 0f, 2, 17.63f*2f*1.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.48f * 1.2f, BulletPivot.Current, 0f, 2, 28.53f*2f*1.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.77f * 1.2f, BulletPivot.Current, 0f, 2, 40.95f*2f*1.2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.16f * 1.2f, BulletPivot.Current, 0f, 2, 56.77f*2f*1.2f));

                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.79f * 1.4f, BulletPivot.Current, 0f, 2, 8.13f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.16f * 1.4f, BulletPivot.Current, 0f, 2, 17.63f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.48f * 1.4f, BulletPivot.Current, 0f, 2, 28.53f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.77f * 1.4f, BulletPivot.Current, 0f, 2, 40.95f*2f*1.2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.16f * 1.4f, BulletPivot.Current, 0f, 2, 56.77f*2f*1.2f + 5f));
                yield return new WaitForMillisecondFrames(1200);
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.79f * 1f, BulletPivot.Current, 0f, 2, 8.13f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.16f * 1f, BulletPivot.Current, 0f, 2, 17.63f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.48f * 1f, BulletPivot.Current, 0f, 2, 28.53f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.77f * 1f, BulletPivot.Current, 0f, 2, 40.95f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.16f * 1f, BulletPivot.Current, 0f, 2, 56.77f*2f + 5f));

                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.79f * 1.2f, BulletPivot.Current, 0f, 2, 8.13f*2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.16f * 1.2f, BulletPivot.Current, 0f, 2, 17.63f*2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.48f * 1.2f, BulletPivot.Current, 0f, 2, 28.53f*2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.77f * 1.2f, BulletPivot.Current, 0f, 2, 40.95f*2f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.16f * 1.2f, BulletPivot.Current, 0f, 2, 56.77f*2f));

                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.79f * 1.4f, BulletPivot.Current, 0f, 2, 8.13f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.16f * 1.4f, BulletPivot.Current, 0f, 2, 17.63f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.48f * 1.4f, BulletPivot.Current, 0f, 2, 28.53f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.77f * 1.4f, BulletPivot.Current, 0f, 2, 40.95f*2f + 5f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 4.16f * 1.4f, BulletPivot.Current, 0f, 2, 56.77f*2f + 5f));
                yield return new WaitForMillisecondFrames(900);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_MainTurret_3A : BulletFactory, IBulletPattern
{
    private readonly EnemyBoss4_MainTurret _typedEnemyObject;
    private readonly int _barrelAnimationTrigger = Animator.StringToHash("BarrelShoot");

    public BulletPattern_EnemyBoss4_MainTurret_3A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBoss4_MainTurret;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        Vector3 pos = GetFirePos(0);

        _typedEnemyObject.m_BarrelAnimator.SetTrigger(_barrelAnimationTrigger);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            var property = new BulletProperty(pos, BulletImage.PinkLarge, 5.1f, BulletPivot.Current, 0f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 500);
            var subProperty = new BulletProperty(pos, BulletImage.PinkNeedle, 4.5f, BulletPivot.Fixed, Random.Range(0f, 360f), 30, 12f);
            CreateBullet(property, spawnTiming, subProperty);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            var property = new BulletProperty(pos, BulletImage.PinkLarge, 5.1f, BulletPivot.Current, 0f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, 200, new Vector2Int(170, 170));
            var subProperty = new BulletProperty(pos, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, Random.Range(0f, 360f), 45, 8f);
            CreateBullet(property, spawnTiming, subProperty);
        }
        else
        {
            var property = new BulletProperty(pos, BulletImage.PinkLarge, 5.1f, BulletPivot.Current, 0f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, 200, new Vector2Int(125, 125));
            var subProperty = new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Fixed, Random.Range(0f, 360f), 50, 7.2f);
            CreateBullet(property, spawnTiming, subProperty);
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss4_SmallTurret_0 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SmallTurret_0(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 3000, 2000, 1000 };
        while(true) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 4f, BulletPivot.Current, 0f));
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_FrontTurret_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_FrontTurret_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 4f, BulletPivot.Current, 0f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 4.1f, BulletPivot.Current, 0f, 3, 14f));
        }
        else {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 3.5f, BulletPivot.Current, 0f, 2, 9f));
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 4.1f, BulletPivot.Current, 0f, 3, 14f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss4_FrontTurret_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_FrontTurret_1B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        const float gap = 0.6f;
        int[] repeatNum = { 2, 3, 3 };
        const int duration = 6000;
        int currentRepeatNum = repeatNum[(int)SystemManager.Difficulty];
        int period = duration / currentRepeatNum;
        
        for (var i = 0; i < currentRepeatNum; ++i)
        {
            Vector3 pos0, pos1, pos2;
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                pos0 = GetFirePos(0, -gap);
                pos1 = GetFirePos(0);
                pos2 = GetFirePos(0, gap);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (var j = 0; j < 5; j++)
                {
                    pos0 = GetFirePos(0, -gap);
                    pos1 = GetFirePos(0);
                    pos2 = GetFirePos(0, gap);
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Hell)
            {
                for (var j = 0; j < 5; j++)
                {
                    pos0 = GetFirePos(0, -gap);
                    pos1 = GetFirePos(0);
                    pos2 = GetFirePos(0, gap);
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    yield return new WaitForMillisecondFrames(20);
                }
            }

            yield return new WaitForMillisecondFrames(period);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_FrontTurret_1C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_FrontTurret_1C(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        Vector3 pos = GetFirePos(0);
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.5f, BulletPivot.Current, 0f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.3f, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.9f, BulletPivot.Current, 0f));
        }
        else {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6f, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.6f, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.2f, BulletPivot.Current, 0f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        Vector3 pos0, pos1, pos2;
        const float gap = 0.6f;
        var rand = Random.Range(-3f, 3f);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos0 = GetFirePos(0, -gap);
            pos1 = GetFirePos(0);
            pos2 = GetFirePos(0, gap);
            CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand - 27f, 2, 10f));
            CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand, 3, 12f));
            CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand + 27f, 2, 10f));
        }
        else {
            _enemyObject.SetRotatePattern(new RotatePattern_Stop());
            for (int i = 0; i < 3; i++) {
                pos0 = GetFirePos(0, -gap);
                pos1 = GetFirePos(0);
                pos2 = GetFirePos(0, gap);
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 7f, BulletPivot.Current, rand - 26f, 3, 8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 7f, BulletPivot.Current, rand, 3, 10f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 7f, BulletPivot.Current, rand + 26f, 3, 8f));
                    yield return new WaitForMillisecondFrames(50);
                }
                else {
                    CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 8f, BulletPivot.Current, rand - 20f, 3, 6f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8f, BulletPivot.Current, rand, 3, 8f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8f, BulletPivot.Current, rand + 20f, 3, 6f));
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_1B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        const float gap = 0.6f;
        int[] repeatNum = { 2, 3, 3 };
        const int duration = 6000;
        int currentRepeatNum = repeatNum[(int)SystemManager.Difficulty];
        int period = duration / currentRepeatNum;
        
        for (var i = 0; i < currentRepeatNum; ++i)
        {
            Vector3 pos0, pos1, pos2;
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                pos0 = GetFirePos(0, -gap);
                pos1 = GetFirePos(0);
                pos2 = GetFirePos(0, gap);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5.4f, BulletPivot.Current, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (var j = 0; j < 5; j++)
                {
                    pos0 = GetFirePos(0, -gap);
                    pos1 = GetFirePos(0);
                    pos2 = GetFirePos(0, gap);
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Hell)
            {
                for (var j = 0; j < 5; j++)
                {
                    pos0 = GetFirePos(0, -gap);
                    pos1 = GetFirePos(0);
                    pos2 = GetFirePos(0, gap);
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f + j * 0.8f, BulletPivot.Current,
                        0f));
                    yield return new WaitForMillisecondFrames(20);
                }
            }

            yield return new WaitForMillisecondFrames(period);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_1C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_1C(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        const float gap = 0.6f;
        int[] fireDelay = { 450, 200, 150 };
        _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(140f));
        
        while (true) {
            Vector3 pos0 = GetFirePos(0, -gap);
            Vector3 pos1 = GetFirePos(0);
            Vector3 pos2 = GetFirePos(0, gap);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.2f, BulletPivot.Current, 0f, 5, 15f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, -15f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, 15f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 6.7f, BulletPivot.Current, 0f, 5, 13f));
            }
            else {
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueLarge, 6f, BulletPivot.Current, -15f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6f, BulletPivot.Current, 15f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 7.1f, BulletPivot.Current, 0f, 5, 12f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_1D : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_1D(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        Vector3 pos0, pos1, pos2;
        const float gap = 0.6f;
        var rand = Random.Range(-3f, 3f);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos0 = GetFirePos(0, -gap);
            pos1 = GetFirePos(0);
            pos2 = GetFirePos(0, gap);
            CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand - 19f, 2, 10f));
            CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand, 3, 12f));
            CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 6f, BulletPivot.Current, rand + 19f, 2, 10f));
        }
        else {
            _enemyObject.SetRotatePattern(new RotatePattern_Stop());
            for (int i = 0; i < 3; i++) {
                pos0 = GetFirePos(0, -gap);
                pos1 = GetFirePos(0);
                pos2 = GetFirePos(0, gap);
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 7.25f, BulletPivot.Current, rand - 23f, 4, 6f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 7.25f, BulletPivot.Current, rand, 4, 6f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 7.25f, BulletPivot.Current, rand + 23f, 4, 6f));
                    yield return new WaitForFrames(3);
                }
                else {
                    CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, rand - 23f, 4, 6f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, rand, 4, 6f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, rand + 23f, 4, 6f));
                    yield return new WaitForFrames(3);
                }
            }
            _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_2B : BulletFactory, IBulletPattern
{
    private readonly int _side;

    public BulletPattern_EnemyBoss4_SubTurret_2B(EnemyObject enemyObject, int side) : base(enemyObject)
    {
        _side = side;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        const float gap = 0.6f;
        var frame = 0;
        int[] frameAdd = { 4, 3, 2 };

        _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(-280f * _side));
        
        while (frame < 30) {
            Vector3 pos0 = GetFirePos(0, gap);
            Vector3 pos1 = GetFirePos(0);
            Vector3 pos2 = GetFirePos(0, -gap);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 5.6f, BulletPivot.Current, -5f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 6.8f, BulletPivot.Current, 5f));
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueSmall, 8f, BulletPivot.Current, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 6f, BulletPivot.Current, -5f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 7.2f, BulletPivot.Current, 5f));
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueSmall, 8.4f, BulletPivot.Current, 0f));
            }
            else {
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueSmall, 5.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueSmall, 6.6f, BulletPivot.Current, -5f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueSmall, 7.8f, BulletPivot.Current, 5f));
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueSmall, 9f, BulletPivot.Current, 0f));
            }
            frame += frameAdd[(int) SystemManager.Difficulty];
            yield return new WaitForFrames(frameAdd[(int) SystemManager.Difficulty]);
        }
        _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_2C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_2C(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] repeatNum = { 2, 3, 4 };
        int[] fireDelay = { 370, 140, 80 };
        
        while (true) {
            Vector3 pos = GetFirePos(0);
            var rand = Random.Range(-28f, 28f);
            
            for (int i = 0; i < repeatNum[(int) SystemManager.Difficulty]; i++)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 4f + i*0.15f, BulletPivot.Current, rand));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss4_SubTurret_3A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss4_SubTurret_3A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            int[] bulletNum = { 3, 5, 8, 9 };
            foreach (var num in bulletNum)
            {
                Vector3 pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 4.5f, BulletPivot.Current, 0f, num, 13f));
                yield return new WaitForMillisecondFrames(330);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            int[] bulletNum = { 5, 8, 11, 12, 13 };
            foreach (var num in bulletNum)
            {
                Vector3 pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 4.5f, BulletPivot.Current, 0f, num, 9f));
                yield return new WaitForMillisecondFrames(250);
            }
        }
        else
        {
            int[] bulletNum = { 5, 8, 11, 14, 15, 16 };
            foreach (var num in bulletNum)
            {
                Vector3 pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 4.5f, BulletPivot.Current, 0f, num, 7.5f));
                yield return new WaitForMillisecondFrames(190);
            }
        }
        onCompleted?.Invoke();
    }
}
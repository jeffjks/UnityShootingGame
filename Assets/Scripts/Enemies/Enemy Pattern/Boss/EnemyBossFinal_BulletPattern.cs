using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyBossFinal_BulletPattern_1A1 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1A1(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4f, BulletPivot.Player, -20f, 6, 4f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4f, BulletPivot.Player, 20f, 6, 4f));
            
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.4f, BulletPivot.Player, -40f, 6, 6f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.8f, BulletPivot.Player, 0f, 6, 6f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.4f, BulletPivot.Player, 40f, 6, 6f));
            yield return new WaitForMillisecondFrames(1600);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1A2 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1A2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var dir = Random.Range(0f, 360f);
        int timer;
        var timerDelta = 110;
        const float randomDistribution = 2f;
        
        const int bulletNum1 = 25;
        const float interval1 = 360f / bulletNum1;
        const int bulletNum2 = 30;
        const float interval2 = 360f / bulletNum1;

        timer = 0;
        while (timer < 4000)
        {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f, BulletPivot.Fixed, dir, bulletNum1, interval1));
            dir += Random.Range(180f/bulletNum1 - randomDistribution, interval1/2 + randomDistribution);
            yield return new WaitForFrames(8);
            timer += timerDelta;
        }

        yield return new WaitForMillisecondFrames(500);
        dir = Random.Range(0f, 360f);

        timer = 0;
        while (timer < 3500)
        {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 8.1f, BulletPivot.Fixed, dir, bulletNum2, interval2));
            dir += Random.Range(180f/bulletNum2 - randomDistribution, interval2/2 + randomDistribution);
            yield return new WaitForFrames(7);
            timer += timerDelta;
        }
        onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1B1 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1B1(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var accel1 = new BulletAccel(3f, 1000);
        var accel2 = new BulletAccel(6f, 400);

        while (true) {
            for (var i = 0; i < 3; i++)
            {
                var dir = Random.Range(-45f, 45f);
                var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 6f, BulletPivot.Player, dir, accel1);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.Create, 600);
                var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 3f, BulletPivot.Current, 0f, accel2);
                CreateBullet(property, spawnTiming, subProperty);
            }
            yield return new WaitForFrames(5);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1B2 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1B2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var accel1 = new BulletAccel(0.1f, 1000);

        for (int i = 0; i < 10; i++)
        {
            var accel2 = new BulletAccel(7f+i*0.86f, 400);
            var property = new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 10f, BulletPivot.Fixed, 0f, accel1, 4, 90f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 500);
            var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkLarge, 0.1f, BulletPivot.Player, 0f, accel2);
            CreateBullet(property, spawnTiming, subProperty);
            yield return new WaitForFrames(4);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1C1 : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;

    public EnemyBossFinal_BulletPattern_1C1(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _typedEnemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirectionDelta[0] = 29f;
        
        while (true)
        {
            var dir = _typedEnemyObject.m_CustomDirection[0];
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.2f, BulletPivot.Fixed, dir, 10, 36f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.2f, BulletPivot.Fixed, dir - 11f, 10, 36f));
            yield return new WaitForFrames(9);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1C2 : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;

    public EnemyBossFinal_BulletPattern_1C2(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _typedEnemyObject.m_CustomDirection[1] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirectionDelta[1] = 67f;

        while (true) {
            var dir = _typedEnemyObject.m_CustomDirection[1];
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 9f, BulletPivot.Fixed, dir, 9, 40f));
            yield return new WaitForFrames(7);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1D1 : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;
    private readonly int _patternIndex;

    public EnemyBossFinal_BulletPattern_1D1(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _typedEnemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirection[1] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirectionDelta[0] = 23f * _patternIndex;
        _typedEnemyObject.m_CustomDirectionDelta[1] = 43f;

        while (true)
        {
            var pos = GetFirePos(0);
            var dir = _typedEnemyObject.m_CustomDirection;
            var property = new BulletProperty(pos, BulletImage.BlueLarge, 5.3f, BulletPivot.Fixed, dir[0], 4, 90f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
            var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 6.4f, BulletPivot.Fixed, dir[1], 6, 60f);
            CreateBullet(property, spawnTiming, subProperty);
            yield return new WaitForFrames(9);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1D2 : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;

    public EnemyBossFinal_BulletPattern_1D2(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(5000);

        var initDirection = _typedEnemyObject.m_CustomDirectionDelta[1];
        var frame = 2700 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_dir = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            _typedEnemyObject.m_CustomDirectionDelta[1] = Mathf.Lerp(initDirection, -43f, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }

        yield return new WaitForMillisecondFrames(1300);

        initDirection = _typedEnemyObject.m_CustomDirectionDelta[1];
        frame = 2700 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_dir = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            _typedEnemyObject.m_CustomDirectionDelta[1] = Mathf.Lerp(initDirection, 43f, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1E1 : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;
    private readonly int _patternIndex;

    public EnemyBossFinal_BulletPattern_1E1(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
        _patternIndex = patternIndex;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _typedEnemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirectionDelta[0] = -79f * _patternIndex;

        while (true)
        {
            var pos = GetFirePos(0);
            var dir = _typedEnemyObject.m_CustomDirection[0];
            for (int i = 0; i < 6; i++)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 4f + 0.9f*i, BulletPivot.Fixed, dir, 4, 90f));
            }
            yield return new WaitForFrames(7);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1E2 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1E2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true)
        {
            var pos = GetFirePos(0);
            var property = new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Player, 0f, 10, 36f);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 700);
            var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 6.4f, BulletPivot.Player, 0f);
            CreateBullet(property, spawnTiming, subProperty);
            yield return new WaitForMillisecondFrames(1000);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_2A : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;

    public EnemyBossFinal_BulletPattern_2A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        _typedEnemyObject.m_CustomDirectionDelta[0] = 31f;

        yield return ExecuteSubPattern(BulletImage.PinkLarge, 3, 30f);
        yield return new WaitForMillisecondFrames(400);
        yield return ExecuteSubPattern(BulletImage.BlueLarge, 5, -60f);
        yield return new WaitForMillisecondFrames(400);
        yield return ExecuteSubPattern(BulletImage.PinkLarge, 7, 90f);
        yield return new WaitForMillisecondFrames(1400);
        
        for (int i = 0; i < 9; i++)
        {
            var pos = GetFirePos(0);
            var dir = Random.Range(3.5f, 6.5f);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, dir, 36, 10f));
            yield return new WaitForFrames(10);
            
            //yield return ExecuteSubPattern(BulletImage.BlueLarge, 9, -140f);
            //CreateBulletsSector(3, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletSpawnType.EraseAndCreate, 1000,
            //3, 1f, BulletPivot.Current, -140f, accel2);
        }
        onCompleted?.Invoke();
    }

    private IEnumerator ExecuteSubPattern(BulletImage bulletImage, int num, float subDir)
    {
        _typedEnemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        var accel1 = new BulletAccel(1f, 1000);
        var accel2 = new BulletAccel(9.2f, 500);
        var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 1000);
        var subProperty = new BulletProperty(Vector3.zero, bulletImage, 1f, BulletPivot.Current, subDir, accel2);
        var rand = Random.Range(0, 2) * 2 - 1;
        
        for (int i = 0; i < num; i++)
        {
            var pos = GetFirePos(0);
            var dir = _typedEnemyObject.m_CustomDirection[0];
            var property = new BulletProperty(pos, bulletImage, 8f, BulletPivot.Fixed, dir * rand, accel1, 30, 12f);
            CreateBullet(property, spawnTiming, subProperty);
            yield return new WaitForFrames(7);
        }
    }
}

public class EnemyBossFinal_BulletPattern_FinalA : BulletFactory, IBulletPattern
{
    private readonly EnemyBossFinal _typedEnemyObject;
    private float _currentBulletSpeed;
    const int BULLET_DELAY = 700;
    private const float MAX_BULLET_SPEED = 10f;
    private const float MIN_BULLET_SPEED = 7f;
    private const float MAX_DIRECTION_DELTA_0 = 73f;
    private const float MAX_DIRECTION_DELTA_1 = 137f;
    /*
    분홍탄 = m_CustomDirection[0]
    청탄 = m_CustomDirection[1]
    */

    public EnemyBossFinal_BulletPattern_FinalA(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyBossFinal;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var accel = new BulletAccel(0.1f, BULLET_DELAY + 300);
        _typedEnemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirection[1] = Random.Range(0f, 360f);
        _typedEnemyObject.m_CustomDirectionDelta[0] = MAX_DIRECTION_DELTA_0;
        _typedEnemyObject.m_CustomDirectionDelta[1] = MAX_DIRECTION_DELTA_1;
        _currentBulletSpeed = MAX_BULLET_SPEED;

        //_typedEnemyObject.StartCoroutine(ExecuteSubPattern1());
        _typedEnemyObject.StartCoroutine(ExecuteSubPattern2());
        _typedEnemyObject.StartCoroutine(ExecuteSubPattern3());

        while (true)
        {
            var pos = GetFirePos(0);
            var dir = _typedEnemyObject.m_CustomDirection;
            var property = new BulletProperty(pos, BulletImage.PinkLarge, _currentBulletSpeed, BulletPivot.Fixed, dir[0], accel);
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, BULLET_DELAY);
            var subProperty1 = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 8.3f, BulletPivot.Fixed, dir[1], 3, 48f);
            var subProperty2 = new BulletProperty(Vector3.zero, BulletImage.BlueSmall, 6.8f, BulletPivot.Fixed, dir[1] + 180f, 2, 36f);
            
            for (var i = 0; i < 3; ++i)
            {
                CreateBullet(property, spawnTiming, subProperty1);
                CreateBullet(property, spawnTiming, subProperty2);
                property.direction += 120f;
                subProperty1.direction += 120f;
                subProperty2.direction += 120f;
            }
            yield return new WaitForFrames(5);
        }
        //onCompleted?.Invoke();
    }

    // Unused
    private IEnumerator ExecuteSubPattern1()
    {
        yield return new WaitForMillisecondFrames(2400);
        while (true)
        {
            yield return new WaitForMillisecondFrames(Random.Range(8000, 10000) - 2400);

            var initDirection = _typedEnemyObject.m_CustomDirectionDelta[0];
            var frame = 1200 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
                
                _typedEnemyObject.m_CustomDirectionDelta[0] = Mathf.Lerp(initDirection, - MAX_DIRECTION_DELTA_0, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(8000, 10000) - 2400);

            initDirection = _typedEnemyObject.m_CustomDirectionDelta[0];
            frame = 1200 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
                
                _typedEnemyObject.m_CustomDirectionDelta[0] = Mathf.Lerp(initDirection, MAX_DIRECTION_DELTA_0, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }
    
    private IEnumerator ExecuteSubPattern2()
    {
        yield return new WaitForMillisecondFrames(1700);
        while (true)
        {
            yield return new WaitForMillisecondFrames(Random.Range(7000, 9000) - 1700);

            var initDirection = _typedEnemyObject.m_CustomDirectionDelta[1];
            var frame = 700 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
                
                _typedEnemyObject.m_CustomDirectionDelta[1] = Mathf.Lerp(initDirection, - MAX_DIRECTION_DELTA_1, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(7000, 9000) - 1700);

            initDirection = _typedEnemyObject.m_CustomDirectionDelta[1];
            frame = 700 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
                
                _typedEnemyObject.m_CustomDirectionDelta[1] = Mathf.Lerp(initDirection, MAX_DIRECTION_DELTA_1, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }
    
    private IEnumerator ExecuteSubPattern3()
    {
        yield return new WaitForMillisecondFrames(2000);
        while (true) {
            yield return new WaitForMillisecondFrames(Random.Range(5000, 8000) - 2000);

            var initSpeed = _currentBulletSpeed;
            var frame = 2000 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_spd = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                
                _currentBulletSpeed = Mathf.Lerp(initSpeed, MIN_BULLET_SPEED, t_spd);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(5000, 8000) - 2000);

            initSpeed = _currentBulletSpeed;
            frame = 2000 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_spd = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                
                _currentBulletSpeed = Mathf.Lerp(initSpeed, MAX_BULLET_SPEED, t_spd);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }
}

public class EnemyBossFinal_BulletPattern_FinalB : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_FinalB(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true)
        {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.3f, BulletPivot.Player, 0f, 26, 13.8461f));
            yield return new WaitForMillisecondFrames(400);
        }
        //onCompleted?.Invoke();
    }
}
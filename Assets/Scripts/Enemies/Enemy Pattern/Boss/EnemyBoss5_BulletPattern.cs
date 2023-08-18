using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BulletPattern_EnemyBoss5_1A1 : BulletFactory, IBulletPattern
{
    private readonly Func<int> _func_fireDelay;
    private readonly float _targetDelay;

    public BulletPattern_EnemyBoss5_1A1(EnemyObject enemyObject, Func<int> func_fireDelay, float targetDelay) : base(enemyObject)
    {
        _func_fireDelay = func_fireDelay;
        _targetDelay = targetDelay;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        float rand = Random.Range(0f, 360f);

        while (_func_fireDelay.Invoke() < _targetDelay) {
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.7f, BulletPivot.Fixed,rand, 18, 20f));
                rand += Random.Range(8.4375f, 14.0625f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f, BulletPivot.Fixed,rand, 24, 15f));
                rand += Random.Range(6.75f, 11.25f);
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f, BulletPivot.Fixed,rand, 30, 12f));
                rand += Random.Range(4.5f, 7.5f);
            }
            yield return new WaitForMillisecondFrames(_func_fireDelay.Invoke());
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1A2 : BulletFactory, IBulletPattern
{
    private readonly Func<int> _func_fireDelay;
    private readonly float _targetDelay;

    public BulletPattern_EnemyBoss5_1A2(EnemyObject enemyObject, Func<int> func_fireDelay, float targetDelay) : base(enemyObject)
    {
        _func_fireDelay = func_fireDelay;
        _targetDelay = targetDelay;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(4f, 600);

        while (_func_fireDelay.Invoke() > _targetDelay) {
            Vector3 pos = GetFirePos(0);
            var dir = Random.Range(-6f, 6f);
            var interval = Random.Range(0f, 24f);
            
            var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
            var subProperty = new BulletProperty(pos, BulletImage.PinkNeedle, 2f, BulletPivot.Current, 0f, accel, 2, 20f + interval);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5f, BulletPivot.Player, dir, 6, 20f);
                CreateBullet(property, spawnTiming, subProperty);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5.3f, BulletPivot.Player, dir, 10, 12f);
                CreateBullet(property, spawnTiming, subProperty);
            }
            else {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f, BulletPivot.Player, dir, 12, 10f);
                CreateBullet(property, spawnTiming, subProperty);
            }
            yield return new WaitForMillisecondFrames(_func_fireDelay.Invoke());
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1B1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_1B1(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true)
        {
            var pos = GetFirePos(0);
            var dir = Random.Range(-5f, 5f);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.2f, BulletPivot.Player, dir - 2f, 7, 19f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir, 7, 19f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.2f, BulletPivot.Player, dir + 2f, 7, 19f));
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir - 2f, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f, BulletPivot.Player, dir, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir + 2f, 9, 14f));
                yield return new WaitForMillisecondFrames(650);
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.2f, BulletPivot.Player, dir - 4f, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir - 2f, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f, BulletPivot.Player, dir, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir + 2f, 9, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5.2f, BulletPivot.Player, dir + 4f, 9, 14f));
                yield return new WaitForMillisecondFrames(500);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1B2 : BulletFactory, IBulletPattern
{
    private readonly Func<Vector2, Vector2, float> _func_getAngleToTarget;

    public BulletPattern_EnemyBoss5_1B2(EnemyObject enemyObject, Func<Vector2, Vector2, float> func_getAngleToTarget) : base(enemyObject)
    {
        _func_getAngleToTarget = func_getAngleToTarget;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 400, 210, 170 };

        while (true) {
            Vector3 center_point = _enemyObject.transform.position + new Vector3(0f, 5f, 0f);
            for (int i = 1; i < 4; i++) {
                var pos = GetFirePos(i);
                var dir = _func_getAngleToTarget.Invoke(center_point, pos);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Fixed, dir));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1C1 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_1C1(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                var dir = Random.Range(-10f, 10f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4f, BulletPivot.Player, dir, 7, 17f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Player, dir, 7, 13f));
                yield return new WaitForMillisecondFrames(1200);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                var dir = Random.Range(-8f, 8f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4f, BulletPivot.Player, dir, 8, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Player, dir, 8, 10f));
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                var dir = Random.Range(-6f, 6f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4f, BulletPivot.Player, dir, 9, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Player, dir, 9, 8f));
                yield return new WaitForMillisecondFrames(400);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1C2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_1C2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty <= GameDifficulty.Expert) {
                yield return null;
            }
            if (SystemManager.Difficulty == GameDifficulty.Hell) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Player, 0f, 9, 10f));
                yield return new WaitForMillisecondFrames(500);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1D1 : BulletFactory, IBulletPattern
{
    private readonly int _firePositionIndex;
    private int _duration;
    private float _dirInit;
    private readonly float _dirDelta;

    public BulletPattern_EnemyBoss5_1D1(EnemyObject enemyObject, int firePositionIndex, int duration, float dirInit, float dirDelta) : base(enemyObject)
    {
        _firePositionIndex = firePositionIndex;
        _duration = duration;
        _dirInit = dirInit;
        _dirDelta = dirDelta;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int time = 0;
        int[] time_add = { 120, 120, 100 };
        int[] period = { 1200, 700, 500 };

        int[] durationScale = { 50, 80, 100 }; // 난이도에 따른 duration 비율 (%)
        _duration *= durationScale[(int) SystemManager.Difficulty] / 100;

        yield return new WaitForMillisecondFrames(Random.Range(0, 1200));

        while (true) {
            while (time < _duration) {
                var pos = GetFirePos(_firePositionIndex);
                
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4.4f, BulletPivot.Fixed, _dirInit, 3, 90f));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4.4f, BulletPivot.Fixed, _dirInit, 5, 72f));
                }
                else {
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4.4f, BulletPivot.Fixed, _dirInit, 6, 60f));
                }
                time += time_add[(int) SystemManager.Difficulty];
                _dirInit += _dirDelta;
                yield return new WaitForMillisecondFrames(time_add[(int) SystemManager.Difficulty]);
            }
            time = 0;
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_1D2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_1D2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Player, 0f));
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Player, 0f));
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 3.6f, BulletPivot.Player, 0f));
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(300);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2A1a : BulletFactory, IBulletPattern
{
    private readonly Transform _bottomLine;

    public BulletPattern_EnemyBoss5_2A1a(EnemyObject enemyObject, Transform bottomLine) : base(enemyObject)
    {
        _bottomLine = bottomLine;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelayFrame = { 15, 7, 5 };

        while (true)
        {
            for (var i = 0; i < 3; ++i)
            {
                var pos = GetFirePos(i + 1);
                if (pos.z > _bottomLine.position.z)
                    continue;

                var dir = _enemyObject.m_CustomDirection[1];
                var randDir = Random.Range(-2f, 2f);
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.5f, BulletPivot.Fixed, dir + randDir));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 9f, BulletPivot.Fixed, dir + randDir));
                }
                else
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 10f, BulletPivot.Fixed, dir + randDir));
                }
            }
            yield return new WaitForFrames(fireDelayFrame[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2A1b : BulletFactory, IBulletPattern
{
    private readonly Transform _bottomLine;

    public BulletPattern_EnemyBoss5_2A1b(EnemyObject enemyObject, Transform bottomLine) : base(enemyObject)
    {
        _bottomLine = bottomLine;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelayFrame = { 8, 3, 2 };

        while (true)
        {
            for (var i = 0; i < 3; ++i)
            {
                var pos = GetFirePos(i + 1);
                if (pos.z > _bottomLine.position.z)
                    continue;

                var dir = _enemyObject.m_CustomDirection[2];
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, dir));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, dir));
                }
                else
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, dir));
                }
            }
            yield return new WaitForFrames(fireDelayFrame[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2A2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_2A2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var interval = Random.Range(0f, 260f);

        while (true) {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8f, BulletPivot.Fixed, -180f, 2, interval));
            interval += 6.1f;
            interval = Mathf.Repeat(interval, 260f);
            yield return new WaitForMillisecondFrames(40);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2B1 : BulletFactory, IBulletPattern
{
    private readonly Transform _bottomLine;

    public BulletPattern_EnemyBoss5_2B1(EnemyObject enemyObject, Transform bottomLine) : base(enemyObject)
    {
        _bottomLine = bottomLine;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 3000, 2500, 2000 };
        int[] bulletNumArray = { 4, 8, 12 };

        while (true)
        {
            var bulletNum = bulletNumArray[(int)SystemManager.Difficulty];
            for (var j = 0; j < bulletNum; ++j)
            {
                var speed = 10f - (bulletNum - 1) * 0.3f + 0.6f * j;
                for (var i = 1; i < 4; i++)
                {
                    var pos = GetFirePos(i);
                    if (pos.z > _bottomLine.position.z)
                        continue;

                    if (SystemManager.Difficulty == GameDifficulty.Normal)
                    {
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Player, 0f));
                    }
                    else if (SystemManager.Difficulty == GameDifficulty.Expert)
                    {
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Player, 0f));
                    }
                    else
                    {
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Player, 0f, 9, 40f));
                    }
                }
                yield return new WaitForMillisecondFrames(600 / bulletNum);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2B2 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_2B2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var dir = 0f;
        int[] fireDelayFrame = { 60, 45, 30 };
        
        while (true) {
            var pos = GetFirePos(0);
            var spawnTiming1 = new BulletSpawnTiming(BulletSpawnType.Create, 350);
            var spawnTiming2 = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 350);
            var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkNeedle, 5.6f, BulletPivot.Current, 0f, 2, 180f);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var property = new BulletProperty(pos, BulletImage.PinkSmall, 6f, BulletPivot.Fixed, dir, 8, 45);
                CreateBullet(property, spawnTiming1, subProperty);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var property = new BulletProperty(pos, BulletImage.PinkSmall, 6f, BulletPivot.Fixed, dir, 12, 30);
                CreateBullet(property, spawnTiming2, subProperty);
            }
            else
            {
                var property = new BulletProperty(pos, BulletImage.PinkSmall, 6f, BulletPivot.Fixed, dir, 15, 24);
                CreateBullet(property, spawnTiming2, subProperty);
            }
            dir += 3f + fireDelayFrame[(int) SystemManager.Difficulty] / 2f;
            yield return new WaitForFrames(fireDelayFrame[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2B2_Old : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss5_2B2_Old(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        Vector3 pos;
        float interval = 350f, timer = 0f;
        float[] min_interval = { 45f, 35f, 30f };
        int[] number = { 52, 54, 55 };

        while (interval > min_interval[(int) SystemManager.Difficulty]) {
            pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 8f, BulletPivot.Fixed, 0f, 2, interval));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8f, BulletPivot.Fixed, 0f, 2, interval + 14f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 8f, BulletPivot.Fixed, 0f, 2, interval + 21f));
            interval -= 21.1f;
            yield return new WaitForMillisecondFrames(80);
        }
        interval = min_interval[(int) SystemManager.Difficulty];
        var rand = Random.Range(0, 2) * 2 - 1;
        var dir = 0f;
        
        while (true) {
            pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 8f, BulletPivot.Fixed, 0f, 2, interval));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8f, BulletPivot.Fixed, 0f, 2, interval + 14f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 8f, BulletPivot.Fixed, 0f, 2, interval + 21f));
            if (timer > 0.8f) {
                for (int i = 0; i < number[(int) SystemManager.Difficulty]; i++)
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8f, BulletPivot.Fixed, dir + 180f, 2, 3f + i*6f));
                timer -= 0.8f;
            }
            timer += 0.08f;
            dir += 1.5f*rand;
            if (Mathf.Abs(dir) > 40f) {
                rand *= -1;
            }
            yield return new WaitForMillisecondFrames(80);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2C1 : BulletFactory, IBulletPattern
{
    private readonly Transform _bottomLine;

    public BulletPattern_EnemyBoss5_2C1(EnemyObject enemyObject, Transform bottomLine) : base(enemyObject)
    {
        _bottomLine = bottomLine;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2500, 1600, 1000 };
        int[] subFireDelay = { 500, 400, 300 };
        int[] repeatNum = { 3, 5, 7 };

        while (true)
        {
            for (var i = 0; i < repeatNum[(int) SystemManager.Difficulty]; ++i)
            {
                for (var j = 0; j < 3; j++)
                {
                    var pos = GetFirePos(0);
                    var dir = _enemyObject.m_CustomDirection[0] / 2f + 120f * j;

                    if (SystemManager.Difficulty == GameDifficulty.Normal)
                    {
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 4.3f, BulletPivot.Fixed, dir, 3, 15f));
                    }
                    else if (SystemManager.Difficulty == GameDifficulty.Expert)
                    {
                        CreateBullet( new BulletProperty(pos, BulletImage.PinkNeedle, 4.4f, BulletPivot.Fixed, dir, 5, 10f));
                    }
                    else
                    {
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 4.5f, BulletPivot.Fixed, dir, 9, 7f));
                        CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.2f, BulletPivot.Fixed, dir + 60f));
                    }
                }
                yield return new WaitForMillisecondFrames(subFireDelay[(int) SystemManager.Difficulty]);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss5_2C2 : BulletFactory, IBulletPattern
{
    private readonly Transform _bottomLine;

    public BulletPattern_EnemyBoss5_2C2(EnemyObject enemyObject, Transform bottomLine) : base(enemyObject)
    {
        _bottomLine = bottomLine;
    }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel1 = new BulletAccel(5f, 800);
        BulletAccel accel2 = new BulletAccel(6f, 800);
        BulletAccel accel3 = new BulletAccel(7f, 800);
        int[] fireDelay = { 320, 150, 100 };

        while (true) {
            for (int i = 1; i < 4; i++) {
                var pos = GetFirePos(i);
                if (pos.z > _bottomLine.position.z)
                    continue;

                var dir = Random.Range(-18f, 18f);
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 2f, BulletPivot.Fixed, dir, accel1));
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 2.5f, BulletPivot.Fixed, dir, accel2));
                }
                else
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 3f, BulletPivot.Fixed, dir, accel3));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
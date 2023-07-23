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
            var newProperty = new BulletProperty(pos, BulletImage.PinkNeedle, 2f, BulletPivot.Current, 0f, accel, 2, 20f + interval);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5f, BulletPivot.Player, dir, 6, 20f);
                CreateBullet(property, spawnTiming, newProperty);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5.3f, BulletPivot.Player, dir, 10, 12f);
                CreateBullet(property, spawnTiming, newProperty);
            }
            else {
                var property = new BulletProperty(pos, BulletImage.PinkNeedle, 5.6f, BulletPivot.Player, dir, 12, 10f);
                CreateBullet(property, spawnTiming, newProperty);
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
        _dirInit = dirDelta;
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
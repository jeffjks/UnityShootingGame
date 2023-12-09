using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss5b_A : BulletFactory, IBulletPattern
{
    private readonly EnemyMiddleBoss5b _typedEnemyObject;

    public BulletPattern_EnemyMiddleBoss5b_A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyMiddleBoss5b;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        string[] stateKey = { "B1", "B2" };
        int[] fireDelay = { 3200, 2600, 2000 };
        var pos = new Vector3[2];
        var state = 1;
        float[,] bulletInfo1 = { {5.4f, 6.6f, 7.5f, 6.6f, 5.4f}, {-56.3f, -24.3f, 0f, 24.3f, 56.3f} };
        float[,] bulletInfo2A = { {4.1f, 4.5f, 5.1f, 5.8f, 5.8f, 5.1f, 4.5f, 4.1f}, {-63.4f, -41.4f, -23.8f, -7.4f, 7.4f, 23.8f, 41.4f, 63.4f} };
        float[,] bulletInfo2B = {
            {5.3f, 5.4f, 5.9f, 6.6f, 7.5f, 7.8f, 7.5f, 6.6f, 5.9f, 5.4f, 5.3f},
            {-75.3f, -56.3f, -38.8f, -24.3f, -10.8f, 0f, 10.8f, 24.3f, 38.8f, 56.3f, 75.3f}
        };
        float[,] bulletInfo3A = { {4.1f, 4.5f, 5.1f, 5.8f, 5.8f, 5.1f, 4.5f, 4.1f}, {-63.4f, -41.4f, -23.8f, -7.4f, 7.4f, 23.8f, 41.4f, 63.4f} };
        float[,] bulletInfo3B = {
            {5.3f, 5.4f, 5.9f, 6.6f, 7.5f, 7.8f, 7.5f, 6.6f, 5.9f, 5.4f, 5.3f},
            {-75.3f, -56.3f, -38.8f, -24.3f, -10.8f, 0f, 10.8f, 24.3f, 38.8f, 56.3f, 75.3f}
        };
        
        while (true) {
            pos[0] = GetFirePos(0);
            pos[1] = GetFirePos(1);
            if (_typedEnemyObject.GetCurrentPhase() == 0)
            {
                _typedEnemyObject.StartPattern(stateKey[(state + 1) / 2], new BulletPattern_EnemyMiddleBoss5b_B(_typedEnemyObject, state));
            }
            else if (_typedEnemyObject.GetCurrentPhase() == 1)
            {
                _typedEnemyObject.m_Turret.StartPattern("B", new BulletPattern_EnemyMiddleBoss5b_Turret_B(_typedEnemyObject.m_Turret));
            }
            state *= -1;
            
            for (int i = 0; i < 2; i++)
            {
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    for (int j = 0; j < bulletInfo1.GetLength(1); ++j)
                    {
                        var speed = bulletInfo1[0, j];
                        var dir = bulletInfo1[1, j];
                        CreateBullet(new BulletProperty(pos[i], BulletImage.PinkLarge, speed, BulletPivot.Current, dir));
                    }
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    for (int j = 0; j < bulletInfo2A.GetLength(1); ++j)
                    {
                        var speed = bulletInfo2A[0, j];
                        var dir = bulletInfo2A[1, j];
                        CreateBullet(new BulletProperty(pos[i], BulletImage.PinkSmall, speed, BulletPivot.Current, dir));
                    }
                    for (int j = 0; j < bulletInfo2B.GetLength(1); ++j)
                    {
                        var speed = bulletInfo2B[0, j];
                        var dir = bulletInfo2B[1, j];
                        CreateBullet(new BulletProperty(pos[i], BulletImage.PinkLarge, speed, BulletPivot.Current, dir));
                    }
                }
                else
                {
                    for (int j = 0; j < bulletInfo3A.GetLength(1); ++j)
                    {
                        var speed = bulletInfo3A[0, j];
                        var dir = bulletInfo3A[1, j];
                        CreateBullet(new BulletProperty(pos[i], BulletImage.PinkSmall, speed, BulletPivot.Current, dir));
                    }
                    for (int j = 0; j < bulletInfo3B.GetLength(1); ++j)
                    {
                        var speed = bulletInfo3B[0, j];
                        var dir = bulletInfo3B[1, j];
                        CreateBullet(new BulletProperty(pos[i], BulletImage.PinkLarge, speed, BulletPivot.Current, dir));
                    }
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5b_B : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;

    public BulletPattern_EnemyMiddleBoss5b_B(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(2000);
        var rand = Random.Range(-6f, 6f);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 4; i++) {
                var pos = GetFirePos(2);
                var speed = 6.7f - 0.2f * i;
                var dir = rand + 4.8f * i * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, speed, BulletPivot.Current, dir, 5, 25f));
                yield return new WaitForMillisecondFrames(260);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 5; i++) {
                var pos = GetFirePos(2);
                var speed = 7f - 0.2f * i;
                var dir = rand + 4.8f * i * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, speed, BulletPivot.Current, dir, 7, 19f));
                yield return new WaitForMillisecondFrames(210);
            }
        }
        else {
            for (int i = 0; i < 7; i++) {
                var pos = GetFirePos(2);
                var speed = 7f - 0.2f * i;
                var dir = rand + 4.8f * i * _patternIndex;
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, speed, BulletPivot.Current, dir, 7, 19f));
                yield return new WaitForMillisecondFrames(150);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5b_Turret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5b_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1800, 1400, 1200 };
        BulletAccel accel = new BulletAccel(7.4f, 900);
        
        while(true) {
            var pos = GetFirePos(0);
            var dir = Random.Range(-3, 3f);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 2.4f, BulletPivot.Current, dir, accel, 5, 16f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 2.4f, BulletPivot.Current, dir, accel, 5, 12f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 2.4f, BulletPivot.Current, dir, accel, 5, 12f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5b_Turret_B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5b_Turret_B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.5f, BulletPivot.Current, 0f, 11, 8f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.2f, BulletPivot.Current, 0f, 11, 8f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.3f, BulletPivot.Current, 0f, 11, 8f));
        }
        else {
            var pos = GetFirePos(0); // 5.1 ~ 7.8
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.0f, BulletPivot.Current, 0f, 15, 7f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.4f, BulletPivot.Current, 0f, 15, 7f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.0f, BulletPivot.Current, 0f, 15, 7f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.8f, BulletPivot.Current, 0f, 15, 7f));
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.8f, BulletPivot.Current, 0f, 15, 7f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}
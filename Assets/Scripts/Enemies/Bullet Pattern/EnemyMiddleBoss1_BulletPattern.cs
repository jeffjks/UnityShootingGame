using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss1_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss1_1A(EnemyObject enemyObject, UnityAction<int> action = null) : base(enemyObject, action) {}

    private Vector3 CurrentPos => _enemyObject.transform.position;
    
    public IEnumerator ExecutePattern(int patternIndex = 0)
    {
        yield return new WaitForMillisecondFrames(600);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 5, 17f));
                yield return new WaitForMillisecondFrames(1520);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 6, 17f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 6, 17f));
                yield return new WaitForMillisecondFrames(1520);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 7, 17f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 7, 17f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 7, 17f));
                yield return new WaitForMillisecondFrames(3120); // --------------------------
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 24, 15f));
                yield return new WaitForMillisecondFrames(1200);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 24, 15f));
                yield return new WaitForMillisecondFrames(3600);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 8, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 8, 10f));
                yield return new WaitForMillisecondFrames(1400);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 9, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 9, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 9, 10f));
                yield return new WaitForMillisecondFrames(1400);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 10f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 10f));
                yield return new WaitForMillisecondFrames(2200); // --------------------------
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 40, 9f));
                yield return new WaitForMillisecondFrames(600);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 40, 9f));
                yield return new WaitForMillisecondFrames(600);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 40, 9f));
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Hell) {
            while(true) {
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 10, 8f));
                yield return new WaitForMillisecondFrames(1400);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 11, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 11, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 11, 8f));
                yield return new WaitForMillisecondFrames(1400);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 12, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 12, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 12, 8f));
                yield return new WaitForMillisecondFrames(120);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 5f, BulletPivot.Fixed, 0f, 12, 8f));
                yield return new WaitForMillisecondFrames(2000); // --------------------------
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, 0f, 45, 8f));
                yield return new WaitForMillisecondFrames(600);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, 0f, 45, 8f));
                yield return new WaitForMillisecondFrames(600);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueSmall, 6.6f, BulletPivot.Fixed, 0f, 45, 8f));
                yield return new WaitForMillisecondFrames(2200);
            }
        }
    }
}

public class BulletPattern_EnemyMiddleBoss1_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss1_2A(EnemyObject enemyObject, UnityAction<int> action = null) : base(enemyObject, action) {}

    private Vector3 CurrentPos => _enemyObject.transform.position;
    
    public IEnumerator ExecutePattern(int patternIndex = 0)
    {
        while(true) {
            CreateBullet(new BulletProperty(CurrentPos, BulletImage.PinkLarge, 7f, BulletPivot.Player, 0f, 8, 13f));
            yield return new WaitForMillisecondFrames(2000);
        }
    }
}

public class BulletPattern_EnemyMiddleBoss1_Turret_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss1_Turret_1A(EnemyObject enemyObject, UnityAction<int> action = null) : base(enemyObject, action) {}

    private Vector3 CurrentPos => _enemyObject.m_FirePosition[0].position;
    
    public IEnumerator ExecutePattern(int patternIndex = 0)
    {
        _action?.Invoke(0);
        yield return new WaitForMillisecondFrames(1500);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                _enemyObject.IsRotatable = true;
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 4f, BulletPivot.Current, 0f));
                _enemyObject.IsRotatable = false;
                yield return new WaitForMillisecondFrames(2000 + Random.Range(0, 1000));
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                _enemyObject.IsRotatable = true;
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 4.2f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 4.2f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 4.2f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 4.2f, BulletPivot.Current, 0f));
                _enemyObject.IsRotatable = false;
                yield return new WaitForMillisecondFrames(1200 + Random.Range(0, 500));
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Hell) {
            while(true) {
                _enemyObject.IsRotatable = true;
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 5.6f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 5.6f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 5.6f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 5.6f, BulletPivot.Current, 0f));
                _enemyObject.IsRotatable = false;
                yield return new WaitForMillisecondFrames(1000 + Random.Range(0, 400));
            }
        }
    }
}

public class BulletPattern_EnemyMiddleBoss1_Turret_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss1_Turret_2A(EnemyObject enemyObject, UnityAction<int> action = null) : base(enemyObject, action) {}

    private Vector3 CurrentPos => _enemyObject.m_FirePosition[0].position;
    
    public IEnumerator ExecutePattern(int patternIndex = 0)
    {
        _action?.Invoke(1);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            BulletAccel accel = new BulletAccel(5.5f, 1000);
            while(true) {
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 0f, accel));
                yield return new WaitForMillisecondFrames(200);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Expert) {
            BulletAccel accel = new BulletAccel(5.5f, 1000);
            while(true) {
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 0f, accel));
                yield return new WaitForMillisecondFrames(50);
            }
        }
        if (SystemManager.Difficulty == GameDifficulty.Hell) {
            while(true) {
                BulletAccel accel1 = new BulletAccel(6.1f, 1000);
                BulletAccel accel2 = new BulletAccel(5f, 1000);
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 3f, accel1));
                CreateBullet(new BulletProperty(CurrentPos, BulletImage.BlueNeedle, 2f, BulletPivot.Current, -3f, accel2));
                yield return new WaitForMillisecondFrames(50);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyBoss1_1B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_1B(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted) // Blue Bomb
    {
        BulletAccel accel = new BulletAccel(0.1f, 800);
        BulletSpawnTiming bulletSpawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 800);
        int random_value = Random.Range(0, 2);

        for (int i = 0; i < 2; i++) {
            var pos = _enemyObject.m_FirePosition[random_value + i*(1 - 2*random_value)].position;
            BulletProperty bulletProperty = new BulletProperty(pos, BulletImage.BlueLarge, 8.2f, BulletPivot.Fixed, 0f, accel);
            BulletProperty newBulletProperty;
            var random_dir = Random.Range(0f, 360f);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 5.4f, BulletPivot.Fixed, random_dir, 15, 24f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 5.4f, BulletPivot.Fixed, random_dir, 20, 18f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 4.2f, BulletPivot.Fixed, random_dir + 9f, 20, 18f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
            }
            else {
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 5.4f, BulletPivot.Fixed, random_dir, 24, 15f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 4.6f, BulletPivot.Fixed, random_dir + 7.5f, 24, 15f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
                newBulletProperty = new BulletProperty(Vector3.zero, BulletImage.BlueLarge, 3.8f, BulletPivot.Fixed, random_dir, 24, 15f);
                CreateBullet(bulletProperty, bulletSpawnTiming, newBulletProperty);
            }
            yield return new WaitForMillisecondFrames(500);
        }
        yield return new WaitForMillisecondFrames(2000);
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_1C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_1C(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 150, 100, 83 };
        int[] fire_number = { 20, 30, 36 };
        yield return new WaitForMillisecondFrames(1000);
        
        for (int i = 0; i < fire_number[(int) SystemManager.Difficulty]; i++) {
            CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueNeedle, 4.8f, BulletPivot.Player, Random.Range(-52f, 52f)));
            CreateBullet(new BulletProperty(GetFirePos(3), BulletImage.BlueNeedle, 4.8f, BulletPivot.Player, Random.Range(-52f, 52f)));
                
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        yield return new WaitForMillisecondFrames(500);
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_Turret1_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret1_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkLarge, 7f, BulletPivot.Current, Random.Range(-2f, 2f), 7, 14f));
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss1_Turret1_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret1_2A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        const float gap = 0.03f;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 3; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 1f*i, BulletPivot.Current, -3f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 1f*i, BulletPivot.Current, 3f));
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 6; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, -14f));
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, -3f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, 3f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, 14f));
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else {
            for (int i = 0; i < 6; i++) {
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, -18f));
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, -10f));
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, -3f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, 3f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 0.7f * i, BulletPivot.Current, 10f));
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.BlueLarge, 5f + 0.7f*i, BulletPivot.Current, 18f));
                
                if (i == 5) {
                    for (int j = -1; j < 2; j += 2) {
                        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 9f, BulletPivot.Current, -26f*j));
                        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 8.4f, BulletPivot.Current, -34f*j));
                        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 7.8f, BulletPivot.Current, -43f*j));
                        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 7.1f, BulletPivot.Current, -47f*j));
                        CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 6.3f, BulletPivot.Current, -52f*j));
                    }
                }
                yield return new WaitForMillisecondFrames(60);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_Turret2_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret2_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int random_value = Random.Range(0, 2);
        Vector3 pos = GetFirePos(0);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.6f, BulletPivot.Current, 0f, 5, 16f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            if (random_value == 0) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.6f, BulletPivot.Current, -1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Current, 0f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.2f, BulletPivot.Current, 1.2f, 7, 12f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.6f, BulletPivot.Current, 1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Current, 0f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.2f, BulletPivot.Current, -1.2f, 7, 12f));
            }
        }
        else {
            if (random_value == 0) {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.8f, BulletPivot.Current, -2.4f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.6f, BulletPivot.Current, -1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Current, 0f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.2f, BulletPivot.Current, 1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.0f, BulletPivot.Current, 2.4f, 7, 12f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.8f, BulletPivot.Current, 2.4f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.6f, BulletPivot.Current, 1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Current, 0f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.2f, BulletPivot.Current, -1.2f, 7, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.0f, BulletPivot.Current, -2.4f, 7, 12f));
            }
        }
        onCompleted?.Invoke();
        yield break;
    }
}

public class BulletPattern_EnemyBoss1_Turret2_2A : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;

    public BulletPattern_EnemyBoss1_Turret2_2A(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.1f, BulletPivot.Current, 0f, 3, 25f));
            yield return new WaitForMillisecondFrames(540);
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.1f, BulletPivot.Current, 0f, 3, 25f));
            yield return new WaitForMillisecondFrames(540);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, 0f, 4, 17f));
            yield return new WaitForMillisecondFrames(270);
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, 0f, 4, 17f));
            yield return new WaitForMillisecondFrames(270);
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, 0f, 4, 17f));
            yield return new WaitForMillisecondFrames(270);
            CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.5f, BulletPivot.Current, 0f, 4, 17f));
            yield return new WaitForMillisecondFrames(270);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.7f, BulletPivot.Current, 0f, 5 - _patternIndex, 17f));
                yield return new WaitForMillisecondFrames(270);
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkNeedle, 6.7f, BulletPivot.Current, 0f, 4 + _patternIndex, 17f));
                yield return new WaitForMillisecondFrames(270);
            }
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_Turret2_2B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret2_2B(EnemyObject enemyObject) : base(enemyObject) {}

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        for (int i = 0; i < 2; i++) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 6f, BulletPivot.Fixed, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 5.5f, BulletPivot.Fixed, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 6.6f, BulletPivot.Fixed, 0f));
            }
            else {
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 6f, BulletPivot.Fixed, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.PinkSmall, 7.2f, BulletPivot.Fixed, 0f));
            }
            yield return new WaitForMillisecondFrames(400);
        }
        onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_Turret3_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret3_1A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(8.8f, 1000);
        int[] fireDelay = { 2250, 1500, 1000 };
        
        while(true) {
            Vector3 pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 5f, BulletPivot.Current, Random.Range(-1f, 1f)));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 3f, BulletPivot.Current, Random.Range(-1f, 1f), accel, 3, 18f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 3f, BulletPivot.Current, Random.Range(-1f, 1f), accel, 3, 18f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyBoss1_Turret3_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyBoss1_Turret3_2A(EnemyObject enemyObject) : base(enemyObject) {}
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        Vector3 pos = GetFirePos(0);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.75f, BulletPivot.Current, 0f, 15, 24f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.75f, BulletPivot.Current, 0f, 18, 20f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.8f, BulletPivot.Current, 0f, 18, 20f));
        }
        else {
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6f, BulletPivot.Current, 0f, 24, 15f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.1f, BulletPivot.Current, 0f, 24, 15f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}


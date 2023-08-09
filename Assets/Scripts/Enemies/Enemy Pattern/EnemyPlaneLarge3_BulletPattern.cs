using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneLarge3_BulletPattern_A : BulletFactory, IBulletPattern
{
    private readonly EnemyPlaneLarge3 _typedEnemyObject;

    public EnemyPlaneLarge3_BulletPattern_A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyPlaneLarge3;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1600, 1200, 1000 };
        yield return new WaitForMillisecondFrames(2300);

        while(true) {
            _typedEnemyObject.m_Turret.StartPattern("A", new EnemyPlaneLarge3_BulletPattern_Turret_A(_typedEnemyObject.m_Turret));
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                for (int i = 0; i < 2; i++)
                {
                    var pos = GetFirePos(0);
                    var dir1 = Random.Range(-8f, 8f);
                    var dir2 = Random.Range(-8f, 8f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Current, dir1, 7, 23f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.2f, BulletPivot.Current, dir2, 6, 23f));
                    yield return new WaitForMillisecondFrames(800);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (int i = 0; i < 3; i++)
                {
                    var pos = GetFirePos(0);
                    var dir1 = Random.Range(-5f, 5f);
                    var dir2 = Random.Range(-5f, 5f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Current, dir1, 9, 17f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.2f, BulletPivot.Current, dir2, 8, 17f));
                    yield return new WaitForMillisecondFrames(500);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    var pos = GetFirePos(0);
                    var dir1 = Random.Range(-5f, 5f);
                    var dir2 = Random.Range(-5f, 5f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Current, dir1, 11, 14f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.2f, BulletPivot.Current, dir2, 10, 14f));
                    yield return new WaitForMillisecondFrames(500);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);

            bool rand = Random.Range(0, 2) == 0;
            
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++)
                {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    var num1 = rand ? 5 : 6;
                    var num2 = rand ? 5 : 6;
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 7.5f, BulletPivot.Current, 0f, num1, 19f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 7.5f, BulletPivot.Current, 0f, num2, 19f));
                    yield return new WaitForMillisecondFrames(180);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 6; i++)
                {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    var num1 = rand ? 7 : 8;
                    var num2 = rand ? 7 : 8;
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 8f, BulletPivot.Current, 0f, num1, 12f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 8f, BulletPivot.Current, 0f, num2, 12f));
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else {
                for (int i = 0; i < 8; i++)
                {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    var num1 = rand ? 7 : 8;
                    var num2 = rand ? 7 : 8;
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 8.3f, BulletPivot.Current, 0f, num1, 12f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 8.3f, BulletPivot.Current, 0f, num2, 12f));
                    yield return new WaitForMillisecondFrames(100);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty] + 500);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge3_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge3_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 2; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.2f, BulletPivot.Current, 0f, 6, 25f));
                yield return new WaitForMillisecondFrames(800);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 3; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.2f, BulletPivot.Current, 0f, 8, 20f));
                yield return new WaitForMillisecondFrames(500);
            }
        }
        else {
            for (int i = 0; i < 3; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6f, BulletPivot.Current, 0f, 11, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.2f, BulletPivot.Current, 0f, 10, 16f));
                yield return new WaitForMillisecondFrames(500);
            }
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge3_BulletPattern_Old : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge3_BulletPattern_Old(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            for (int i = 0; i < 3; i++)
            {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 3, 29f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 3, 29f));
                yield return new WaitForMillisecondFrames(400);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            for (int i = 0; i < 4; i++)
            {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 5, 21f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 5, 21f));
                yield return new WaitForMillisecondFrames(300);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 5, 21f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 5.4f, BulletPivot.Fixed, 0f, 5, 21f));
                yield return new WaitForMillisecondFrames(240);
            }
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge3_BulletPattern_Turret_Old : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge3_BulletPattern_Turret_Old(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 600, 600, 500 };
        var accel = new BulletAccel(0.1f, 800);
        yield return new WaitForMillisecondFrames(2300);

        while(true) {
            for (int i = 0; i < 3; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    var pos = GetFirePos(0);
                    var property = new BulletProperty(pos, BulletImage.BlueLarge, 8.3f, BulletPivot.Current, 0f, accel);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
                    var subProperty= new BulletProperty(Vector3.zero, BulletImage.BlueSmall, 4.3f, BulletPivot.Player, 0f);
                    CreateBullet(property, spawnTiming, subProperty);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    var pos = GetFirePos(0);
                    var property = new BulletProperty(pos, BulletImage.BlueLarge, 8.3f, BulletPivot.Current, 0f, accel, 2, 100f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
                    var subProperty= new BulletProperty(Vector3.zero, BulletImage.BlueSmall, 4.3f, BulletPivot.Player, 0f, 3, 16f);
                    CreateBullet(property, spawnTiming, subProperty);
                }
                else {
                    var pos = GetFirePos(0);
                    var property = new BulletProperty(pos, BulletImage.BlueLarge, 8.3f, BulletPivot.Current, 0f, accel, 2, 100f);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
                    var subProperty= new BulletProperty(Vector3.zero, BulletImage.BlueSmall, 4.3f, BulletPivot.Player, 0f, 3, 16f);
                    CreateBullet(property, spawnTiming, subProperty);
                }
                yield return new WaitForMillisecondFrames(280);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
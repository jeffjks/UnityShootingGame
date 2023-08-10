using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankLarge1_BulletPattern_B : BulletFactory, IBulletPattern
{
    public EnemyTankLarge1_BulletPattern_B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(800);
        while(true)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int j = 0; j < 2; j++)
                {
                    var pos = GetFirePos(j);
                    var property = new BulletProperty(pos, BulletImage.PinkLarge, 7f, BulletPivot.Player, 0f, 3, 22f);
                    var enemyBullets = CreateBullet(property);
                    foreach (var enemyBullet in enemyBullets)
                    {
                        enemyBullet.ClampDirection(_enemyObject.CurrentAngle, 90f);
                    }
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var pos = GetFirePos(j);
                        var property = new BulletProperty(pos, BulletImage.PinkLarge, 6f + 0.8f * i, BulletPivot.Player, 0f, 3, 22f);
                        var enemyBullets = CreateBullet(property);
                        foreach (var enemyBullet in enemyBullets)
                        {
                            enemyBullet.ClampDirection(_enemyObject.CurrentAngle, 90f);
                        }
                    }
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        var pos = GetFirePos(j);
                        var property1 = new BulletProperty(pos, BulletImage.PinkSmall, 6f + 0.8f * i, BulletPivot.Player, 0f, 2, 22f);
                        var property2 = new BulletProperty(pos, BulletImage.PinkLarge, 6f + 0.8f * i, BulletPivot.Player, 0f, 3, 22f);
                        var enemyBullets1 = CreateBullet(property1);
                        var enemyBullets2 = CreateBullet(property2);
                        foreach (var enemyBullet in enemyBullets1)
                        {
                            enemyBullet.ClampDirection(_enemyObject.CurrentAngle, 90f);
                        }
                        foreach (var enemyBullet in enemyBullets2)
                        {
                            enemyBullet.ClampDirection(_enemyObject.CurrentAngle, 90f);
                        }
                    }
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            yield return new WaitForMillisecondFrames(2500);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyTankLarge1_BulletPattern_SubTurret_A : BulletFactory, IBulletPattern
{
    private readonly EnemyTankLarge1 _typedEnemyObject;

    public EnemyTankLarge1_BulletPattern_SubTurret_A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyTankLarge1;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(90f));
                for (int i = 0; i < 7; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.5f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(137);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(48f));
                yield return new WaitForMillisecondFrames(900);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(750);
                
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(-90f));
                for (int i = 0; i < 7; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.5f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(137);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-48f));
                yield return new WaitForMillisecondFrames(750);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(900);
            }

            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(90f));
                for (int i = 0; i < 10; i++)
                {
                    var pos = GetFirePos(0);
                    for (int j = 0; j < 5; j++)
                    {
                        var speed = 4.2f + 0.8f * j;
                        var dir = 0.8f * j;
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed, BulletPivot.Current, -dir));
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(48f));
                yield return new WaitForMillisecondFrames(800);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(600);
                
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(-90f));
                for (int i = 0; i < 10; i++)
                {
                    var pos = GetFirePos(0);
                    for (int j = 0; j < 5; j++) {
                        var speed = 4.2f + 0.8f * j;
                        var dir = 0.8f * j;
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed, BulletPivot.Current, dir));
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-48f));
                yield return new WaitForMillisecondFrames(600);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(800);
            }

            else {
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(90f));
                for (int i = 0; i < 12; i++)
                {
                    var pos = GetFirePos(0);
                    for (int j = 0; j < 6; j++) {
                        var speed = 4f + 0.8f * j;
                        var dir = 3f * j;
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed, BulletPivot.Current, -dir));
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(48f));
                yield return new WaitForMillisecondFrames(800);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(600);
                
                _enemyObject.SetRotatePattern(new RotatePattern_RotateAround(-90f));
                for (int i = 0; i < 12; i++)
                {
                    var pos = GetFirePos(0);
                    for (int j = 0; j < 6; j++) {
                        var speed = 4f + 0.8f * j;
                        var dir = 3f * j;
                        CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed, BulletPivot.Current, dir));
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(100f, 100f).SetOffsetAngle(-48f));
                yield return new WaitForMillisecondFrames(600);
                
                _typedEnemyObject.m_BackTurret.StartPattern("A", new EnemyTankLarge1_BulletPattern_BackTurret_A(_typedEnemyObject.m_BackTurret));
                yield return new WaitForMillisecondFrames(800);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyTankLarge1_BulletPattern_BackTurret_A : BulletFactory, IBulletPattern
{
    public EnemyTankLarge1_BulletPattern_BackTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel1 = new BulletAccel(0f, 500);
        BulletAccel accel2 = new BulletAccel(7.4f, 800);

        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            for (var i = 0; i < 3; i++)
            {
                foreach (var posTransform in _enemyObject.m_FirePosition)
                {
                    var pos = posTransform.position;
                    var speed = Random.Range(5f, 12f);
                    var dir = _enemyObject.CurrentAngle + Random.Range(-20f, 20f);
                    var property = new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir, accel1);
                    var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 500);
                    var subDir = Random.Range(-25f, 25f);
                    var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkLarge, 0.1f, BulletPivot.Player, subDir, accel2);
                    CreateBullet(property, spawnTiming, subProperty);
                }
                yield return new WaitForMillisecondFrames(320);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var k = 0; k < 3; k++) {
                    foreach (var posTransform in _enemyObject.m_FirePosition)
                    {
                        var pos = posTransform.position;
                        var speed = Random.Range(5f, 12f);
                        var dir = _enemyObject.CurrentAngle + Random.Range(-20f, 20f);
                        var property = new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir, accel1);
                        var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 500);
                        var subDir = Random.Range(-25f, 25f);
                        var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkLarge, 0.1f, BulletPivot.Player, subDir, accel2);
                        CreateBullet(property, spawnTiming, subProperty);
                    }
                }
                yield return new WaitForMillisecondFrames(240);
            }
        }
        else
        {
            for (var i = 0; i < 3; i++)
            {
                for (var k = 0; k < 4; k++) {
                    foreach (var posTransform in _enemyObject.m_FirePosition)
                    {
                        var pos = posTransform.position;
                        var speed = Random.Range(5f, 12f);
                        var dir = _enemyObject.CurrentAngle + Random.Range(-20f, 20f);
                        var property = new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir, accel1);
                        var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 500);
                        var subDir = Random.Range(-25f, 25f);
                        var subProperty = new BulletProperty(Vector3.zero, BulletImage.PinkLarge, 0.1f, BulletPivot.Player, subDir, accel2);
                        CreateBullet(property, spawnTiming, subProperty);
                    }
                }
                yield return new WaitForMillisecondFrames(240);
            }
        }
        onCompleted?.Invoke();
    }
}
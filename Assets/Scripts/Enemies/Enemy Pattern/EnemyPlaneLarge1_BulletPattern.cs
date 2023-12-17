using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneLarge1_BulletPattern_1A : BulletFactory, IBulletPattern
{
    private readonly EnemyPlaneLarge1 _typedEnemyObject;

    public EnemyPlaneLarge1_BulletPattern_1A(EnemyObject enemyObject) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyPlaneLarge1;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1600, 800, 600 };
        var state = Random.Range(0, 2) * 2 - 1;
        yield return new WaitForMillisecondFrames(200);

        while(!_enemyObject.TimeLimitState)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var rand = Random.Range(-8f, 0f);
                var targetAngle = _enemyObject.AngleToPlayer;
                for (var i = 0; i < 4; i++)
                {
                    var pos = GetFirePos(0);
                    var speed = 5.6f + 0.32f * i;
                    var dir = targetAngle + (rand + 2f * i) * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Fixed, dir, 5, 20f));
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(280);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var rand = Random.Range(-8f, 0f);
                var targetAngle = _enemyObject.AngleToPlayer;
                for (var i = 0; i < 6; i++)
                {
                    var pos = GetFirePos(0);
                    var speed = 6f + 0.4f * i;
                    var dir = targetAngle + (rand + 2f * i) * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Fixed, dir, 5, 20f));
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(140);
            }
            else
            {
                var rand = Random.Range(-8f, 0f);
                var targetAngle = _enemyObject.AngleToPlayer;
                for (var i = 0; i < 8; i++)
                {
                    var pos = GetFirePos(0);
                    var speed = 5.6f + 0.4f * i;
                    var dir = targetAngle + (rand + 2f * i) * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed, BulletPivot.Fixed, dir, 7, 20f));
                    yield return new WaitForMillisecondFrames(70);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            state *= -1;


            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var rand = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    var pos = GetFirePos(0);
                    var dir = rand + 12f * i * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.2f, BulletPivot.Fixed, dir, 8, 45f));
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                var rand = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    var pos = GetFirePos(0);
                    var dir = rand + 9f * i * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.2f, BulletPivot.Fixed, dir, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, dir - 28f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, dir - 14f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, dir + 14f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, dir + 28f, 4, 90f));
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else
            {
                var rand = Random.Range(0f, 360f);
                for (int i = 0; i < 9; i++) {
                    var pos = GetFirePos(0);
                    var dir = rand + 9f * i * state;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Fixed, dir, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.6f, BulletPivot.Fixed, dir - 28f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.6f, BulletPivot.Fixed, dir - 14f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.6f, BulletPivot.Fixed, dir + 14f, 4, 90f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.6f, BulletPivot.Fixed, dir + 28f, 4, 90f));
                    yield return new WaitForMillisecondFrames(120);
                }
            }
            yield return new WaitForMillisecondFrames(200);

            _typedEnemyObject.m_Turret[0].StartPattern("A", new EnemyPlaneLarge1_BulletPattern_Turret_1A(_typedEnemyObject.m_Turret[0]));
            _typedEnemyObject.m_Turret[1].StartPattern("A", new EnemyPlaneLarge1_BulletPattern_Turret_1A(_typedEnemyObject.m_Turret[1]));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge1_BulletPattern_2A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge1_BulletPattern_2A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1500, 1000, 800 };
        yield return new WaitForMillisecondFrames(800);

        while(!_enemyObject.TimeLimitState)
        {
            var pos = GetFirePos(0);
            var rand = Random.Range(-3f, 3f);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.1f, BulletPivot.Player, rand, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.4f, BulletPivot.Player, rand, 6, 16f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Player, rand, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.1f, BulletPivot.Player, rand, 12, 8f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.6f, BulletPivot.Player, rand, 6, 16f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.1f, BulletPivot.Player, rand, 6, 16f));
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 5.6f, BulletPivot.Player, rand, 8, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.1f, BulletPivot.Player, rand, 14, 7f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.6f, BulletPivot.Player, rand, 8, 14f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 7.1f, BulletPivot.Player, rand, 8, 14f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge1_BulletPattern_2B : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge1_BulletPattern_2B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1350, 900, 700 };
        yield return new WaitForMillisecondFrames(1500);

        while(!_enemyObject.TimeLimitState)
        {
            var pos = GetFirePos(0);
            var rand = Random.Range(0f, 360f);
            
            if (SystemManager.Difficulty == GameDifficulty.Hell)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5f, BulletPivot.Fixed, rand, 20, 18f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5.4f, BulletPivot.Fixed, rand + 9f, 20, 18f));
                yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
                
                rand = Random.Range(0f, 360f);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5f, BulletPivot.Fixed, rand, 20, 18f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.4f, BulletPivot.Fixed, rand + 9f, 20, 18f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneLarge1_BulletPattern_Turret_1A : BulletFactory, IBulletPattern
{
    public EnemyPlaneLarge1_BulletPattern_Turret_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            for (var i = 0; i < 6; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.5f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(80);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            for (var i = 0; i < 10; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 0f, 2, 28f));
                yield return new WaitForMillisecondFrames(80);
            }
        }
        else {
            for (var i = 0; i < 12; i++)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 0f, 2, 28f));
                yield return new WaitForMillisecondFrames(80);
            }
        }
        onCompleted?.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss2_1A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss2_1A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(2500);
        while(true)
        {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.5f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 18, 20f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.5f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 18, 20f));
                yield return new WaitForMillisecondFrames(2500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.5f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 18, 20f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.5f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 18, 20f));
                yield return new WaitForMillisecondFrames(2500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(2000);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 20, 18f));
                yield return new WaitForMillisecondFrames(2000);
            }
            else
            {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(1500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(500);
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.PinkSmall, 6.8f, BulletPivot.Fixed, GameManager.RandomTest(0f, 360f), 24, 15f));
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss2_2A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss2_2A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 110, 70, 40 };
        while(true) {
            var dir = _enemyObject.m_CustomDirection[0];
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 6.4f, BulletPivot.Fixed, dir));
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueLarge, 6.4f, BulletPivot.Fixed, -dir));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueSmall, 6.7f, BulletPivot.Fixed, dir, 2, 180f));
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueSmall, 6.7f, BulletPivot.Fixed, -dir, 2, 180f));
            }
            else {
                CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueSmall, 6.7f, BulletPivot.Fixed, dir, 2, 180f));
                CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueSmall, 6.7f, BulletPivot.Fixed, -dir, 2, 180f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss2_2B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss2_2B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1800, 900, 500 };
        yield return new WaitForMillisecondFrames(1000);
        
        while(true)
        {
            var dir = GameManager.RandomTest(0f, 360f);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Player, dir, 20, 18f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (var i = 0; i < 3; i++) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f + i*0.6f, BulletPivot.Player, dir, 24, 15f));
                    yield return new WaitForMillisecondFrames(80);
                }
            }
            else {
                for (var i = 0; i < 3; i++) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f + i*0.6f, BulletPivot.Player, dir, 30, 12f));
                    yield return new WaitForMillisecondFrames(80);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss2_MainTurret_0 : BulletFactory, IBulletPattern
{
    private readonly Animator _barrelAnimator;
    private readonly int _barrelAnimationTrigger = Animator.StringToHash("BarrelShoot");

    public BulletPattern_EnemyMiddleBoss2_MainTurret_0(EnemyObject enemyObject) : base(enemyObject)
    {
        _barrelAnimator = (_enemyObject as EnemyMiddleBoss2_MainTurret)?.m_BarrelAnimator;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 3000, 2400, 1800 };
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                for (var i = 0; i < 4; i++)
                {
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 6.4f - 0.5f*i, BulletPivot.Current, - 64 + i*8f));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 6.4f - 0.5f*i, BulletPivot.Current, 64 - i*8f));
                    yield return new WaitForMillisecondFrames(240);
                }
                for (var i = 0; i < 3; i++)
                {
                    var randomValue = GameManager.RandomTest(-2f, 2f);
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 4.4f + 1.1f*i, BulletPivot.Current, randomValue));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 4.4f + 1.1f*i, BulletPivot.Current, -randomValue));
                }
                _barrelAnimator.SetTrigger(_barrelAnimationTrigger);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (var i = 0; i < 7; i++)
                {
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 6.4f - 0.3f*i, BulletPivot.Current, - 64 + i*8f));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 6.4f - 0.3f*i, BulletPivot.Current, 64 - i*8f));
                    yield return new WaitForMillisecondFrames(170);
                }
                for (var i = 0; i < 6; i++)
                {
                    var randomValue = GameManager.RandomTest(-2f, 2f);
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 4f + 0.8f*i, BulletPivot.Current, randomValue));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 4f + 0.8f*i, BulletPivot.Current, -randomValue));
                }
                _barrelAnimator.SetTrigger(_barrelAnimationTrigger);
            }
            else
            {
                for (var i = 0; i < 7; i++)
                {
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 7.2f - 0.3f*i, BulletPivot.Current, - 66 + i*8f));
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 6f - 0.3f*i, BulletPivot.Current, - 62 + i*8f));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 6f - 0.3f*i, BulletPivot.Current, 62 - i*8f));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 7.2f - 0.3f*i, BulletPivot.Current, 66 - i*8f));
                    yield return new WaitForMillisecondFrames(170);
                }
                for (var i = 0; i < 6; i++)
                {
                    var randomValue = GameManager.RandomTest(-1f, 1f);
                    CreateBullet(new BulletProperty(GetFirePos(0), BulletImage.BlueLarge, 4f + 0.8f*i, BulletPivot.Current, randomValue));
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueLarge, 4f + 0.8f*i, BulletPivot.Current, -randomValue));
                }
                _barrelAnimator.SetTrigger(_barrelAnimationTrigger);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss2_SubTurret_0 : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss2_SubTurret_0(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 1500, 1250 };
        const float gap = 0.25f;
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 5.5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 5.5f, BulletPivot.Current, 0f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 5f + 0.9f*i, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 5f + 0.9f*i, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else {
                for (int i = 0; i < 6; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(0, -gap), BulletImage.PinkNeedle, 5f + 0.8f*i, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(0, gap), BulletImage.PinkNeedle, 5f + 0.8f*i, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
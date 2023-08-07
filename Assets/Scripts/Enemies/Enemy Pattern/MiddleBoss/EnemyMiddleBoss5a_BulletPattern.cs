using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletPattern_EnemyMiddleBoss5a_MainTurret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_MainTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while(true)
        {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5f, BulletPivot.Current, Random.Range(-4f, 4f), 10, 13f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 3.5f, BulletPivot.Current, Random.Range(-3f, 3f), 11, 12f));
                for (var i = 0; i < 4; i++)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5f, BulletPivot.Current, - 2.25f + 1.5f * i, 7, 15f));
                }
            }
            else
            {
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 3f, BulletPivot.Current, Random.Range(-3f, 3f), 11, 12f));
                for (var i = 0; i < 4; i++)
                {
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.5f, BulletPivot.Current, - 2.25f + 1.5f * i, 9, 12f));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Current, - 2.25f + 1.5f * i, 10, 12f));
                }
            }
            yield return new WaitForMillisecondFrames(1000);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5a_MainTurret_B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_MainTurret_B(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 3.8f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.6f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(210);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 3.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.2f, BulletPivot.Current,  -3.7f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.2f, BulletPivot.Current,  3.7f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5f, BulletPivot.Current,  0f));
                yield return new WaitForMillisecondFrames(150);
            }
        }
        else {
            while(true) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 3.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.2f, BulletPivot.Current,  -3.7f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 4.2f, BulletPivot.Current,  3.7f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5f, BulletPivot.Current,  0f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5a_MainTurret_C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_MainTurret_C(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, 0f, 4, 1.2f));
                yield return new WaitForMillisecondFrames(1000);
            }
        }
        else {
            while(true) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Current, 0f, 6, 1.2f));
                yield return new WaitForMillisecondFrames(800);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5a_MainTurret_D : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_MainTurret_D(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while(true) {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                var dir = Random.Range(-4f, 4f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5f, BulletPivot.Current, dir, 10, 14f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                var dir = Random.Range(-3f, 3f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.5f, BulletPivot.Current, dir, 15, 9f));
            }
            else {
                var dir = Random.Range(-3f, 3f);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6f, BulletPivot.Current, dir, 15, 9f));
            }
            yield return new WaitForMillisecondFrames(1000);
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5a_SubTurret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_SubTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            while(true)
            {
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 180f));
                yield return new WaitForFrames(7);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            while(true)
            {
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 0f, 2, 9f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 180f, 2, 9f));
                yield return new WaitForFrames(3);
            }
        }
        else
        {
            while(true)
            {
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 0f, 2, 6f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 2f, BulletPivot.Current, 180f, 2, 6f));
                yield return new WaitForFrames(2);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyMiddleBoss5a_SubTurret_B : BulletFactory, IBulletPattern
{
    private readonly int _patterIndex;

    public BulletPattern_EnemyMiddleBoss5a_SubTurret_B(EnemyObject enemyObject, int patterIndex) : base(enemyObject)
    {
        _patterIndex = patterIndex;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        int[] fireDelay = { 3000, 1500, 1000 };
        
        if (_patterIndex == 1) {
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty] / 2);
        }

        while (true) {
            _enemyObject.SetRotatePattern(new RotatePattern_Stop());
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++)
                {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 5.5f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 5.5f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 5.5f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

public class BulletPattern_EnemyMiddleBoss5a_SubTurret_C : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMiddleBoss5a_SubTurret_C(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            while(true)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.4f, BulletPivot.Current, 0f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            while(true)
            {
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 6.4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkSmall, 6f, BulletPivot.Current, 180f));
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else
        {
            while(true)
            {
                var pos0 = GetFirePos(0);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5f, BulletPivot.Current, 0f, 2, 8f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 6.4f, BulletPivot.Current, 0f, 2, 8f));
                
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkSmall, 5f, BulletPivot.Current, 180f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkSmall, 6.4f, BulletPivot.Current, 180f));
                yield return new WaitForMillisecondFrames(60);
            }
        }
    }
}
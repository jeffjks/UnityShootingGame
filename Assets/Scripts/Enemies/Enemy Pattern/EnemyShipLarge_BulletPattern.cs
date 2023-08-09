using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShipLarge_BulletPattern_2A : BulletFactory, IBulletPattern // TODO. 패턴 리뉴얼 필요
{
    public EnemyShipLarge_BulletPattern_2A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 900, 360, 240 };
        var accel = new BulletAccel(0.1f, 600);
        
        while(true)
        {
            for (int i = 0; i < 2; i++)
            {
                var num = SystemManager.Difficulty == GameDifficulty.Hell ? 3 : 1;
                var pos = GetFirePos(i);
                var dir = Random.Range(0f, 360f);
                var newDir = Random.Range(-18f, 18f);
                var property = new BulletProperty(pos, BulletImage.PinkLarge, 3.6f, BulletPivot.Fixed, dir, accel);
                var spawnTiming = new BulletSpawnTiming(BulletSpawnType.EraseAndCreate, 600);
                var subProperty = new BulletProperty(Vector3.zero, BulletImage.BlueNeedle, 8f, BulletPivot.Player, newDir, num, 25f);
                CreateBullet(property, spawnTiming, subProperty);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyShipLarge_BulletPattern_FrontTurret_A : BulletFactory, IBulletPattern
{
    public EnemyShipLarge_BulletPattern_FrontTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 2000, 1600 };
        const float gap = 0.32f;

        while(true) {
            var pos0 = GetFirePos(0);
            var pos1 = GetFirePos(0, -gap);
            var pos2 = GetFirePos(0, gap);
            
            if (SystemManager.Difficulty <= GameDifficulty.Expert)
            {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, 0f));
            }
            else {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, 3f, 2, 2f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, 5.6f, BulletPivot.Current, -3f, 2, 2f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyShipLarge_BulletPattern_BackTurret_A : BulletFactory, IBulletPattern
{
    public EnemyShipLarge_BulletPattern_BackTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 2000, 1600 };
        
        while(true)
        {
            var pos = GetFirePos(0);
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.2f, BulletPivot.Current, 0, 3, 20f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.6f, BulletPivot.Current, 0, 5, 13f));
            }
            else {
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6.9f, BulletPivot.Current, 0, 9, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 7.2f, BulletPivot.Current, 0, 9, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 7.5f, BulletPivot.Current, 0, 9, 12f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyShipLarge_BulletPattern_SubTurret_A : BulletFactory, IBulletPattern
{
    private readonly int _patternIndex;

    public EnemyShipLarge_BulletPattern_SubTurret_A(EnemyObject enemyObject, int patternIndex) : base(enemyObject)
    {
        _patternIndex = patternIndex;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 1000, 500 };
        
        while(true)
        {
            var pos = GetFirePos(0);
            var dir = _patternIndex * 12f;
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, dir));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
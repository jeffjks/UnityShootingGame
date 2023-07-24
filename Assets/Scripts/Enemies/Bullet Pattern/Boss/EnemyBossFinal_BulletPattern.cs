using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyBossFinal_BulletPattern_1A1 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1A1(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        while (true) {
            var pos = _enemyObject.transform.position;
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 4f, BulletPivot.Player, 40f, 6, 4f));
            
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.4f, BulletPivot.Player, -40f, 6, 6f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 4.8f, BulletPivot.Player, 0f, 6, 6f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 5.4f, BulletPivot.Player, 40f, 6, 6f));
            yield return new WaitForMillisecondFrames(1600);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyBossFinal_BulletPattern_1A2 : BulletFactory, IBulletPattern
{
    public EnemyBossFinal_BulletPattern_1A2(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var dir = Random.Range(0f, 360f);
        int timer;
        var timerDelta = 110;
        const float randomDistribution = 2f;
        
        const int bulletNum1 = 20;
        const float interval1 = 360f / bulletNum1;
        const int bulletNum2 = 25;
        const float interval2 = 360f / bulletNum1;

        timer = 0;
        while (timer < 4000)
        {
            var pos = _enemyObject.transform.position;
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f, BulletPivot.Fixed, dir, bulletNum1, interval1));
            dir += Random.Range(180f/bulletNum1 - randomDistribution, interval1/2 + randomDistribution);
            yield return new WaitForFrames(8);
            timer += timerDelta;
        }

        yield return new WaitForMillisecondFrames(500);
        dir = Random.Range(0f, 360f);

        timer = 0;
        while (timer < 3500)
        {
            var pos = _enemyObject.transform.position;
            CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 8.1f, BulletPivot.Fixed, dir, bulletNum2, interval2));
            dir += Random.Range(180f/bulletNum2 - randomDistribution, interval2/2 + randomDistribution);
            yield return new WaitForFrames(7);
            timer += timerDelta;
        }
        onCompleted?.Invoke();
    }
}
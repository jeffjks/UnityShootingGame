using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankSmall3_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankSmall3_BulletPattern_Turret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

public class EnemyTankSmall3_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyTankSmall3_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 3000, 2000, 1500 };
        float[] speedArray = { 6f, 7.4f, 7.4f };
        yield return new WaitForMillisecondFrames(Random.Range(0, 500));
        
        BulletAccel accel = new BulletAccel(5.2f, 1400);
        yield return new WaitForMillisecondFrames(Random.Range(0, 2000));
        
        while(true)
        {
            var pos0 = GetFirePos(0);
            var pos1 = GetFirePos(1); // Center
            var pos2 = GetFirePos(2);
            var speed = speedArray[(int)SystemManager.Difficulty];
            var dir = Random.Range(-1f, 1f);
            
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 35f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 7.7f, BulletPivot.Current, dir, accel));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 35f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 40f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 20f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 7.7f, BulletPivot.Current, dir, accel));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 20f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 40f));
            }
            else {
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 40f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 36f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 30f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 22f));
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkLarge, speed, BulletPivot.Current, dir - 12f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkLarge, 7.7f, BulletPivot.Current, dir, accel));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 12f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 22f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 30f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 36f));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkLarge, speed, BulletPivot.Current, dir + 40f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
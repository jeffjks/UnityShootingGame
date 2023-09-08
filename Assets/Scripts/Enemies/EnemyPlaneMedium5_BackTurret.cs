using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneMedium5_BackTurret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());
        StartPattern("A", new EnemyPlaneMedium5_BulletPattern_BackTurret_A(this));
    }
}


public class EnemyPlaneMedium5_BulletPattern_BackTurret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneMedium5_BulletPattern_BackTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 2300, 2200 };
        yield return new WaitForMillisecondFrames(1000);

        while (true)
        {
            var rand = Random.Range(0, 2) * 2 - 1;

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Current, 0f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7f, BulletPivot.Current, -rand * 2f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.6f, BulletPivot.Current, rand * 2f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.5f, BulletPivot.Current, 0f, 3, 30f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.9f, BulletPivot.Current, 0f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.5f, BulletPivot.Current, -rand * 3f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.1f, BulletPivot.Current, rand * 3f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.7f, BulletPivot.Current, 0f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.3f, BulletPivot.Current, -rand * 1.5f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.9f, BulletPivot.Current, rand * 1.5f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 9.5f, BulletPivot.Current, 0f, 3, 30f));
            }
            else {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.9f, BulletPivot.Current, 0f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.5f, BulletPivot.Current, -rand * 3f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.1f, BulletPivot.Current, rand * 3f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 7.7f, BulletPivot.Current, 0f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.3f, BulletPivot.Current, -rand * 1.5f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 8.9f, BulletPivot.Current, rand * 1.5f, 3, 30f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 9.5f, BulletPivot.Current, 0f, 3, 30f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
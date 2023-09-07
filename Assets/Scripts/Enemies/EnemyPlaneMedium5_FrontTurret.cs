using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneMedium5_FrontTurret : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyPlaneMedium5_BulletPattern_FrontTurret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

public class EnemyPlaneMedium5_BulletPattern_FrontTurret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneMedium5_BulletPattern_FrontTurret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            for (int i = 0; i < 3; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.1f, BulletPivot.Current, -6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.1f, BulletPivot.Current, 6f, 4, 30f));
                    break;
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.2f, BulletPivot.Current, -6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.2f, BulletPivot.Current, 6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Current, -6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 7.5f, BulletPivot.Current, 6f, 4, 30f));
                }
                else {
                    var pos = GetFirePos(0);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.8f, BulletPivot.Current, -6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 6.8f, BulletPivot.Current, 6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8.2f, BulletPivot.Current, -6f, 4, 30f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 8.2f, BulletPivot.Current, 6f, 4, 30f));
                }
                yield return new WaitForMillisecondFrames(600);
            }
            yield return new WaitForMillisecondFrames(1800);
        }
        //onCompleted?.Invoke();
    }
}

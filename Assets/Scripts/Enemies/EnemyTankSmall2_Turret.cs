using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankSmall2_Turret : EnemyUnit
{
    private readonly IRotatePattern _defaultRotatePattern = new RotatePattern_TargetPlayer(72f, 100f);
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankSmall2_BulletPattern_Turret_A(this, _defaultRotatePattern));
        SetRotatePattern(new RotatePattern_TargetPlayer(72f, 100f));
    }
}

public class EnemyTankSmall2_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    private readonly IRotatePattern _defaultRotatePattern;
    private readonly IRotatePattern _stopRotatePattern = new RotatePattern_Stop();

    public EnemyTankSmall2_BulletPattern_Turret_A(EnemyObject enemyObject, IRotatePattern defaultRotatePattern) :
        base(enemyObject)
    {
        _defaultRotatePattern = defaultRotatePattern;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1200, 600 };
        float[] speedArray = {6.6f, 7.8f, 7.8f};
        yield return new WaitUntil(() => _enemyObject.IsInteractable());
        yield return new WaitForMillisecondFrames(Random.Range(0, 500));
        
        while(true) {
            var dir = Mathf.Floor((_enemyObject.CurrentAngle + 5f)/10f) * 10f;
            var speed = speedArray[(int)SystemManager.Difficulty];

            _enemyObject.SetRotatePattern(_stopRotatePattern);

            var pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir));
            yield return new WaitForMillisecondFrames(130);
            pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir));
            yield return new WaitForMillisecondFrames(130);
            pos = GetFirePos(0);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Fixed, dir));

            _enemyObject.SetRotatePattern(_defaultRotatePattern);
            
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
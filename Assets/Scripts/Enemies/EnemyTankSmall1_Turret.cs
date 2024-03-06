using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankSmall1_Turret : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTankSmall1_BulletPattern_Turret_A(this));
        SetRotatePattern(new RotatePattern_TargetPlayer(60f, 100f));
    }
}

public class EnemyTankSmall1_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyTankSmall1_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 2400, 1200, 600 };
        float[] speedArray = { 6.6f, 7.8f, 7.8f };
        yield return new WaitUntil(() => _enemyObject.IsInteractable());
        yield return new WaitForMillisecondFrames(Random.Range(0, 500));
        
        while (true)
        {
            var pos = GetFirePos(0);
            var dir = Mathf.Floor((_enemyObject.CurrentAngle + 5f)/10f) * 10f;
            var speed = speedArray[(int)SystemManager.Difficulty];
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Fixed, dir));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
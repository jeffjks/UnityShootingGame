using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShipSmall1 : EnemyUnit
{
    protected override void Start()
    {
        base.Start();

        CurrentAngle = m_MoveVector.direction;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
}

public class BulletPattern_EnemyShipSmall1_Turret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyShipSmall1_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1200, 600 };
        float[] speedArray = { 6.6f, 7.8f, 7.8f };

        while (true)
        {
            var pos = GetFirePos(0);
            var speed = speedArray[(int)SystemManager.Difficulty];
            var dir = Mathf.Floor((_enemyObject.CurrentAngle + 5f)/10f) * 10f;
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Fixed, dir));
            
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
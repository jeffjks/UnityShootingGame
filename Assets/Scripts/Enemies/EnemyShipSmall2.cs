using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShipSmall2 : EnemyUnit
{
    private void Start()
    {
        CurrentAngle = m_MoveVector.direction;
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
}

public class BulletPattern_EnemyShipSmall2_Turret_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyShipSmall2_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1200, 600 };
        float[] speedArray = { 6.2f, 7.3f, 7.3f };
        
        while (true)
        {
            var pos = GetFirePos(0);
            var speed = speedArray[(int)SystemManager.Difficulty];
            var dir = Mathf.Floor((_enemyObject.CurrentAngle + 5f)/10f) * 10f;
            
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed * 0.9f, BulletPivot.Fixed, dir));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Fixed, dir));
            if (SystemManager.Difficulty >= GameDifficulty.Expert)
                CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed * 1.1f, BulletPivot.Fixed, dir));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
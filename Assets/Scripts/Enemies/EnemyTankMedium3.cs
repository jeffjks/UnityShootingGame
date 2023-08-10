using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankMedium3 : EnemyUnit
{
    void Start()
    {
        StartPattern("A", new EnemyTankMedium3_BulletPattern(this));
        m_CustomDirection = new CustomDirection();
        m_CustomDirection[0] = Random.Range(0f, 360f);
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    
    protected override void Update()
    {
        base.Update();
        
        m_CustomDirection[0] += 90f / Application.targetFrameRate * Time.timeScale;
    }
}

public class EnemyTankMedium3_BulletPattern : BulletFactory, IBulletPattern
{
    public EnemyTankMedium3_BulletPattern(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 400, 200, 150 };
        float[] speedArray = { 5.2f, 5.2f, 6.4f };
        while(true)
        {
            var pos0 = GetFirePos(0);
            var pos1 = GetFirePos(1);
            var dir = _enemyObject.m_CustomDirection[0];
            var speed = speedArray[(int)SystemManager.Difficulty];
            CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed, BulletPivot.Fixed, dir));
            CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed, BulletPivot.Fixed, dir + 180f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankMedium3 : EnemyUnit
{
    private void Start()
    {
        m_CustomDirection = new CustomDirection();
        StartPattern("A", new EnemyTankMedium3_BulletPattern_A(this));
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    protected override void Update()
    {
        base.Update();
        
        m_CustomDirection[0] += 90f / Application.targetFrameRate * Time.timeScale;
    }
}

public class EnemyTankMedium3_BulletPattern_A : BulletFactory, IBulletPattern
{
    public EnemyTankMedium3_BulletPattern_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 400, 160, 70 };
        float[] speedArray = { 5.2f, 5.2f, 6.4f };
        _enemyObject.m_CustomDirection[0] = Random.Range(0f, 360f);
        
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
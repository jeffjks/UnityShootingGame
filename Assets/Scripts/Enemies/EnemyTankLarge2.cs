using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTankLarge2 : EnemyUnit
{
    private void Start()
    {
        m_CustomDirection = new CustomDirection();
        StartPattern("A", new EnemyTankLarge2_BulletPattern_A(this));
        SetRotatePattern(new RotatePattern_MoveDirection());
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            m_CustomDirection[0] += 97f / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_CustomDirection[0] += 123f / Application.targetFrameRate * Time.timeScale;
        }
    }
}

public class EnemyTankLarge2_BulletPattern_A : BulletFactory, IBulletPattern
{
    public EnemyTankLarge2_BulletPattern_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1000, 400, 400 };
        
        while(true)
        {
            var pos0 = GetFirePos(0);
            var pos1 = GetFirePos(1);
            var dir = _enemyObject.m_CustomDirection[0];
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueLarge, 6f, BulletPivot.Current, dir, 3, 120f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6f, BulletPivot.Current, -dir, 3, 120f));
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(new BulletProperty(pos0, BulletImage.BlueLarge, 6.8f, BulletPivot.Current, dir, 4, 90f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.8f, BulletPivot.Current, -dir, 4, 90f));
            }
            else {
                for (int i = 0; i < 3; i++)
                {
                    var speed = 6.3f + 0.5f * i;
                    CreateBullet(new BulletProperty(pos0, BulletImage.BlueLarge, speed, BulletPivot.Current, dir, 4, 90f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, speed, BulletPivot.Current, -dir, 4, 90f));
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

public class EnemyTankLarge2_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    private readonly IRotatePattern _defaultRotatePattern;
    private readonly IRotatePattern _stopRotatePattern = new RotatePattern_Stop();

    public EnemyTankLarge2_BulletPattern_Turret_A(EnemyObject enemyObject, IRotatePattern defaultRotatePattern) : base(enemyObject)
    {
        _defaultRotatePattern = defaultRotatePattern;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] number1 = {1, 1, 1, 1, 1};
        int[] number2 = {3, 3, 5, 5, 5, 3, 3};
        int[] number3 = {5, 5, 7, 7, 7, 5, 5};
        
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                _enemyObject.SetRotatePattern(_stopRotatePattern);
                for (int i = 0; i < 5; i++)
                {
                    var pos0 = GetFirePos(0);
                    var pos1 = GetFirePos(1);
                    var dir = 12f - 8f * i;
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, dir, number1[i], 12f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, -dir, number1[i], 12f));
                    yield return new WaitForMillisecondFrames(210);
                }
                _enemyObject.SetRotatePattern(_defaultRotatePattern);
                yield return new WaitForMillisecondFrames(2200);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                _enemyObject.SetRotatePattern(_stopRotatePattern);
                for (int i = 0; i < 7; i++)
                {
                    var pos0 = GetFirePos(0);
                    var pos1 = GetFirePos(1);
                    var dir = 16f - 8f * i;
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, dir, number2[i], 8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, -dir, number2[i], 8f));
                    yield return new WaitForMillisecondFrames(140);
                }
                _enemyObject.SetRotatePattern(_defaultRotatePattern);
                yield return new WaitForMillisecondFrames(1800);
            }
            else
            {
                _enemyObject.SetRotatePattern(_stopRotatePattern);
                for (int i = 0; i < 7; i++)
                {
                    var pos0 = GetFirePos(0);
                    var pos1 = GetFirePos(1);
                    var dir = 6f - 6f * i;
                    CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, dir, number3[i], 6f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6.8f, BulletPivot.Current, -dir, number3[i], 6f));
                    yield return new WaitForMillisecondFrames(140);
                }
                _enemyObject.SetRotatePattern(_defaultRotatePattern);
                yield return new WaitForMillisecondFrames(1800);
            }
        }
        //onCompleted?.Invoke();
    }
}
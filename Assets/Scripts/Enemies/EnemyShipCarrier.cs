using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyShipCarrier : EnemyUnit
{
    private EnemyUnit[] _enemyUnits;
    
    private void Start()
    {
        _enemyUnits = GetComponentsInChildren<EnemyUnit>();
        CurrentAngle = m_MoveVector.direction;
        SetRotatePattern(new RotatePattern_MoveDirection());
        m_CustomDirection = new CustomDirection(2);
        m_CustomDirection[0] = GameManager.RandomTest(0f, 360f);
        StartPattern("A", new BulletPattern_EnemyShipCarrier_A(this));
        StartPattern("B", new BulletPattern_EnemyShipCarrier_B(this));
        UpdateCargoUnitDirection();
    }
    
    protected override void Update()
    {
        base.Update();
        
        m_CustomDirection[0] += 120f / Application.targetFrameRate * Time.timeScale;
        m_CustomDirection[1] += 180f / Application.targetFrameRate * Time.timeScale;
        UpdateCargoUnitDirection();
    }

    private void UpdateCargoUnitDirection()
    {
        foreach (var enemyUnit in _enemyUnits)
        {
            if (enemyUnit != null)
            {
                enemyUnit.m_MoveVector.direction = m_MoveVector.direction;
            }
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.SetBulletFreeState(2000);

        foreach (var enemyUnit in _enemyUnits)
        {
            if (enemyUnit != null)
                enemyUnit.m_EnemyDeath.KillEnemy();
        }
        
        yield break;
    }
}

public class BulletPattern_EnemyShipCarrier_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyShipCarrier_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true)
            {
                var pos = GetFirePos(0);
                var dir = _enemyObject.m_CustomDirection[0];
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Fixed, dir, 2, 180f));
                yield return new WaitForMillisecondFrames(160);
            }

        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                var pos = GetFirePos(0);
                var dir = _enemyObject.m_CustomDirection[0];
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Fixed, dir, 3, 120f));
                yield return new WaitForMillisecondFrames(110);
            }

        }
        else {
            while(true) {
                var pos = GetFirePos(0);
                var dir = _enemyObject.m_CustomDirection[0];
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 5.7f, BulletPivot.Fixed, dir, 4, 90f));
                yield return new WaitForMillisecondFrames(70);
            }
        }
        //onCompleted?.Invoke();
    }
}

public class BulletPattern_EnemyShipCarrier_B : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyShipCarrier_B(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true)
            {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                var dir = _enemyObject.m_CustomDirection[1];
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, dir));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, -dir));
                yield return new WaitForMillisecondFrames(200);
            }

        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                var dir = _enemyObject.m_CustomDirection[1];
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, dir, 2, 180f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, -dir, 2, 180f));
                yield return new WaitForMillisecondFrames(120);
            }

        }
        else {
            while(true) {
                var pos1 = GetFirePos(1);
                var pos2 = GetFirePos(2);
                var dir = _enemyObject.m_CustomDirection[1];
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, dir, 2, 30f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, dir + 180f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, -dir, 2, 30f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6.2f, BulletPivot.Fixed, -dir + 180f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        //onCompleted?.Invoke();
    }
}
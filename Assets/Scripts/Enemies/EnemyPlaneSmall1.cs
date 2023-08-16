using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneSmall1 : EnemyUnit
{
    private bool _isTargetingPlayer = true;
    private const float DEFAULT_SPEED = 6.8f;

    void Start()
    {
        StartPattern("A", new BulletPattern_EnemyPlaneSmall1_A(this));
        CurrentAngle = AngleToPlayer;
        m_MoveVector = new MoveVector(DEFAULT_SPEED, AngleToPlayer);
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    protected override void Update()
    {
        base.Update();

        if (_isTargetingPlayer)
        {
            float player_distance = Vector2.Distance(transform.position, PlayerManager.GetPlayerPosition());
            m_MoveVector.direction = AngleToPlayer;

            if (player_distance < 5f) {
                _isTargetingPlayer = false;
            }
        }
    }
}

public class BulletPattern_EnemyPlaneSmall1_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyPlaneSmall1_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2000, 1000, 500 };
        float[] speedArray = { 7.7f, 9.1f, 9.1f };
        yield return new WaitForMillisecondFrames(500);
        
        while (true)
        {
            var pos = GetFirePos(0);
            var speed = speedArray[(int)SystemManager.Difficulty];
            CreateBullet(new BulletProperty(pos, BulletImage.PinkNeedle, speed, BulletPivot.Current, 0f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}
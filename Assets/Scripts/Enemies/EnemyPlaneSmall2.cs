using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneSmall2 : EnemyUnit
{

    private bool _isTargetingPlayer = true;
    private const float DEFAULT_SPEED = 7.2f;

    private void Start()
    {
        StartPattern("A", new BulletPattern_EnemyPlaneSmall2_A(this));
        CurrentAngle = AngleToPlayer;
        m_MoveVector = new MoveVector(DEFAULT_SPEED, AngleToPlayer);
        SetRotatePattern(new RotatePattern_TargetPlayer(0f, 100f));
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        if (_isTargetingPlayer) {
            float player_distance = Vector2.Distance(transform.position, PlayerManager.GetPlayerPosition());
            m_MoveVector.direction = AngleToPlayer;

            if (player_distance < 5f) {
                _isTargetingPlayer = false;
            }
        }
    }
}

public class BulletPattern_EnemyPlaneSmall2_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyPlaneSmall2_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 5000, 2100, 1200 };
        float[] speedArray = { 8.2f, 9.8f, 9.8f };
        yield return new WaitForMillisecondFrames(500);
        
        while (true) {
            var pos = GetFirePos(0);
            var speed = speedArray[(int)SystemManager.Difficulty];
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, speed, BulletPivot.Current, 0f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Current, 0f, 2, 28f));
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}

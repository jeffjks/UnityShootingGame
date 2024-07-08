using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTurret3_Turret : EnemyUnit
{
    public Animator m_BarrelAnimator;
    private readonly int _barrelAnimationTrigger = Animator.StringToHash("BarrelShoot");
    
    private void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartPattern("A", new EnemyTurret3_BulletPattern_Turret_A(this, _barrelAnimationTrigger));
        SetRotatePattern(new RotatePattern_TargetPlayer());
    }
}

public class EnemyTurret3_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    private readonly EnemyTurret3_Turret _typedEnemyObject;
    private readonly int _animationHash;
    private readonly int[] _fireDelay = { 2000, 2000, 1800 };

    public EnemyTurret3_BulletPattern_Turret_A(EnemyObject enemyObject, int animationHash) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyTurret3_Turret;
        _animationHash = animationHash;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        List<EnemyBullet> enemyBullets = new (16);
        
        var delay = GetFireDelay();
        yield return new WaitForMillisecondFrames(delay);
        while(true)
        {
            var pos0 = GetFirePos(0);
            var pos1 = GetFirePos(1);

            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                for (var i = 0; i < 4; i++) {
                    var speed = 4.8f + 0.3f * i;
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed, BulletPivot.Current, 0f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed, BulletPivot.Current, 0f)));
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (var i = 0; i < 4; i++) {
                    var speed1 = 4.6f + 0.3f * i;
                    var speed2 = 5f + 0.3f * i;
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed2, BulletPivot.Current, -2f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed1, BulletPivot.Current, 1.5f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed1, BulletPivot.Current, -1.5f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed2, BulletPivot.Current, 2f)));
                }
            }
            else
            {
                for (var i = 0; i < 4; i++) {
                    var speed1 = 5f + 0.33f * i;
                    var speed2 = 5.5f + 0.33f * i;
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed2, BulletPivot.Current, -2f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos0, BulletImage.BlueNeedle, speed1, BulletPivot.Current, 1.5f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed1, BulletPivot.Current, -1.5f)));
                    enemyBullets.AddRange(CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, speed2, BulletPivot.Current, 2f)));
                }
            }
            if (enemyBullets.Count > 0)
            {
                _typedEnemyObject.m_BarrelAnimator.SetTrigger(_animationHash);
                enemyBullets.Clear();
            }
            delay = GetFireDelay();
            yield return new WaitForMillisecondFrames(delay);
        }
    }

    private int GetFireDelay()
    {
        var delay = Random.Range(0, _fireDelay[(int) SystemManager.Difficulty]);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (SystemManager.IsInGame && ReplayManager.DebugLog)
            ReplayManager.WriteReplayLogFile($"Debug {_typedEnemyObject.name}: {delay}");
#endif
        return delay;
    }
}

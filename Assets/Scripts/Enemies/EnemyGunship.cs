﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EnemyGunship : EnemyUnit, ITargetPosition
{
    private const int TIME_LIMIT = 8000;
    private IEnumerator _timeLimitCoroutine;

    private void Start()
    {
        StartPattern("A", new BulletPattern_EnemyGunship(this));
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer());

        _timeLimitCoroutine = TimeLimit(TIME_LIMIT);
        StartCoroutine(_timeLimitCoroutine);
    }

    protected override void Retreat()
    {
        if (!TimeLimitState) // Retreat when boss or middle boss state
        {
            if (_timeLimitCoroutine != null)
                StopCoroutine(_timeLimitCoroutine);
            _timeLimitCoroutine = TimeLimit();
            StartCoroutine(_timeLimitCoroutine);
            TimeLimitState = true;
        }
    }

    public void MoveTowardsToTarget(Vector2 targetVector, int duration) {
        StartCoroutine(MoveTowardsToTargetSequence(targetVector, duration));
    }

    private IEnumerator MoveTowardsToTargetSequence(Vector2 targetVector, int duration) {
        Vector3 initPosition = transform.position;
        Vector3 targetPosition = new Vector3(targetVector.x, targetVector.y, Depth.ENEMY);
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(initPosition, targetPosition, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator TimeLimit(int timeLimit = 0)
    {
        yield return new WaitForMillisecondFrames(timeLimit);
        TimeLimitState = true;
        var leaveDirection = Mathf.Sign(transform.position.x);
        m_MoveVector.direction = leaveDirection > 0f ? Random.Range(80f, 100f) : Random.Range(-80f, -100f);
        
        float initSpeed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(initSpeed, 5.4f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
    }
}

public class BulletPattern_EnemyGunship : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyGunship(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 2000, 1500, 1000 };
        yield return new WaitForMillisecondFrames(1200);
        
        while (!_enemyObject.TimeLimitState) {
            for (var i = 0; i < 4; i++) {
                var pos1 = GetFirePos(0);
                var pos2 = GetFirePos(1);
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                {
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 6.7f, BulletPivot.Current, -8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 6.7f, BulletPivot.Current, 8f));
                    break;
                }
                if (SystemManager.Difficulty == GameDifficulty.Expert)
                {
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8f, BulletPivot.Current, -18f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8f, BulletPivot.Current, -8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8f, BulletPivot.Current, 8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8f, BulletPivot.Current, 18f));
                }
                else {
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, -38f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, -28f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, -18f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, -8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 8f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 18f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 28f));
                    CreateBullet(new BulletProperty(pos1, BulletImage.BlueNeedle, 8.5f, BulletPivot.Current, 38f));
                }
                yield return new WaitForMillisecondFrames(140);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

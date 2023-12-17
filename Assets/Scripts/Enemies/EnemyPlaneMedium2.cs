using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneMedium2 : EnemyUnit
{
    private const int APPEARANCE_TIME = 1600;
    private const int TIME_LIMIT = 8000;
    private float m_VSpeed = 1.1f;
    private IEnumerator _timeLimitCoroutine;

    private void Start()
    {
        m_MoveVector.speed = 4.4f;

        StartPattern("A", new EnemyPlaneMedium2_BulletPattern_A(this, APPEARANCE_TIME));
        StartPattern("B", new EnemyPlaneMedium2_BulletPattern_B(this, APPEARANCE_TIME));

        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARANCE_TIME / 2) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, m_VSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        _timeLimitCoroutine = TimeLimit(TIME_LIMIT);
        StartCoroutine(_timeLimitCoroutine);
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;

        float init_speed = m_MoveVector.speed;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
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
}

public class EnemyPlaneMedium2_BulletPattern_A : BulletFactory, IBulletPattern
{
    private readonly int _appearanceTime;

    public EnemyPlaneMedium2_BulletPattern_A(EnemyObject enemyObject, int appearanceTime) : base(enemyObject)
    {
        _appearanceTime = appearanceTime;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(_appearanceTime);
        while(!_enemyObject.TimeLimitState) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(1000);
                var pos = GetFirePos(0);
                for (var i = 0; i < 2; ++i)
                {
                    var speed1 = 5f + Random.Range(0f, 1.2f);
                    var speed2 = 5f + Random.Range(0f, 1.2f);
                    var dir1 = Random.Range(-24f, 24f);
                    var dir2 = Random.Range(-24f, 24f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed1, BulletPivot.Player, dir1));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed2, BulletPivot.Player, dir2));
                }
                yield return new WaitForMillisecondFrames(2000);
            }

            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(800);
                var pos = GetFirePos(0);
                for (var i = 0; i < 4; ++i)
                {
                    var speed1 = 5f + Random.Range(0f, 1.2f);
                    var speed2 = 5f + Random.Range(0f, 1.2f);
                    var dir1 = Random.Range(-24f, 24f);
                    var dir2 = Random.Range(-24f, 24f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed1, BulletPivot.Player, dir1));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed2, BulletPivot.Player, dir2));
                }
                yield return new WaitForMillisecondFrames(1600);
            }

            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(1), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(2), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(600);

                for (int i = 0; i < 4; i++) {
                    CreateBullet(new BulletProperty(GetFirePos(3), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    CreateBullet(new BulletProperty(GetFirePos(4), BulletImage.BlueNeedle, 6.2f, BulletPivot.Current, 0f));
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(600);
            }
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneMedium2_BulletPattern_B : BulletFactory, IBulletPattern
{
    private readonly int _appearanceTime;

    public EnemyPlaneMedium2_BulletPattern_B(EnemyObject enemyObject, int appearanceTime) : base(enemyObject)
    {
        _appearanceTime = appearanceTime;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForMillisecondFrames(_appearanceTime);
        while(!_enemyObject.TimeLimitState) {
            yield return new WaitForMillisecondFrames(Random.Range(0, 1500));

            if (SystemManager.Difficulty == GameDifficulty.Hell) {
                var pos = GetFirePos(0);
                for (int i = 0; i < 6; i++) {
                    var speed1 = 5.2f + Random.Range(0f, 1.8f);
                    var speed2 = 5.2f + Random.Range(0f, 1.8f);
                    var dir1 = Random.Range(-24f, 24f);
                    var dir2 = Random.Range(-24f, 24f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, speed1, BulletPivot.Player, dir1));
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, speed2, BulletPivot.Player, dir2));
                }
            }
            yield return new WaitForMillisecondFrames(1600);
        }
        onCompleted?.Invoke();
    }
}

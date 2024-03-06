using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneMedium4 : EnemyUnit
{
    public EnemyPlaneMedium4_Turret[] m_Turret = new EnemyPlaneMedium4_Turret[2];
    
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 12500;
    private float m_VSpeed = 0.4f;
    private IEnumerator _timeLimitCoroutine;

    private void Start()
    {
        m_MoveVector.speed = 3.8f;

        StartPattern("A", new EnemyPlaneMedium4_BulletPattern_A(this, APPEARANCE_TIME));

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

public class EnemyPlaneMedium4_BulletPattern_A : BulletFactory, IBulletPattern
{
    private readonly EnemyPlaneMedium4 _typedEnemyObject;
    private readonly int _appearanceTime;

    public EnemyPlaneMedium4_BulletPattern_A(EnemyObject enemyObject, int appearanceTime) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyPlaneMedium4;
        _appearanceTime = appearanceTime;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();
        int[] fireDelay = { 1600, 1600, 1000 };
        yield return new WaitForMillisecondFrames(_appearanceTime);

        while(!_enemyObject.TimeLimitState) {
            _typedEnemyObject.m_Turret[0].StartPattern("A", new EnemyPlaneMedium4_BulletPattern_Turret_A(_typedEnemyObject.m_Turret[0]));
            _typedEnemyObject.m_Turret[1].StartPattern("A", new EnemyPlaneMedium4_BulletPattern_Turret_A(_typedEnemyObject.m_Turret[1]));
            yield return new WaitForMillisecondFrames(2000);

            _typedEnemyObject.m_Turret[0].StopPattern("A");
            _typedEnemyObject.m_Turret[1].StopPattern("A");
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
            
            if (SystemManager.Difficulty <= GameDifficulty.Expert) {
                for (int i = 0; i < 10; i++)
                {
                    var pos = _enemyObject.transform.position;
                    var speed = 7.2f + 0.7f * i;
                    var dir = Random.Range(-1f, 1f);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed, BulletPivot.Player, dir));
                    yield return new WaitForFrames(3);
                }
                yield return new WaitForFrames(18);
            }
            else {
                for (int i = 0; i < 15; i++) {
                    var pos = _enemyObject.transform.position;
                    var speed1 = 6.8f + 0.7f * i;
                    var speed2 = 6.6f + 0.7f * i;
                    var dir = Random.Range(-1f, 1f);
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, speed1, BulletPivot.Player, dir, 2, 24f));
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, speed2, BulletPivot.Player, dir));
                    yield return new WaitForFrames(3);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneMedium4_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneMedium4_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        yield return new WaitForEndOfFrame();

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                var factor = SpeedOval(_enemyObject.CurrentAngle);
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, -180f));
                yield return new WaitForMillisecondFrames(160);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                var factor = SpeedOval(_enemyObject.CurrentAngle);
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, 0f, 2, 1f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, -180f, 2, 1f));
                yield return new WaitForMillisecondFrames(100);
            }
        }
        else {
            while(true) {
                var factor = SpeedOval(_enemyObject.CurrentAngle);
                var pos0 = GetFirePos(0);
                var pos1 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos0, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, 0f, 2, 2f));
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6f * factor, BulletPivot.Current, -180f, 2, 2f));
                yield return new WaitForMillisecondFrames(80);
            }
        }
        //onCompleted?.Invoke();
    }

    private float SpeedOval(float angle) {
        var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        var result = Mathf.Pow(cos, 2) + Mathf.Pow(sin, 2) * 0.7f;
        return result;
    }
}
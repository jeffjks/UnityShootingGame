using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPlaneMedium3 : EnemyUnit
{
    public EnemyPlaneMedium3Turret[] m_Turret = new EnemyPlaneMedium3Turret[2];
    
    private const int APPEARANCE_TIME = 1200;
    private const int TIME_LIMIT = 7800;
    private float m_VSpeed = 1.2f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 5.4f;

        StartPattern("A", new EnemyPlaneMedium3_BulletPattern_A(this, APPEARANCE_TIME));

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += DestroyTurrets;
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
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
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
        yield break;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!TimeLimitState) { // Retreat when boss or middle boss state
            if (SystemManager.PlayState != PlayState.OnField) {
                if (m_TimeLimit != null)
                    StopCoroutine(m_TimeLimit);
                m_TimeLimit = TimeLimit();
                StartCoroutine(m_TimeLimit);
                TimeLimitState = true;
            }
        }
    }

    private void DestroyTurrets() {
        m_Turret[0].m_EnemyDeath.OnDying();
        m_Turret[1].m_EnemyDeath.OnDying();
    }
}

public class EnemyPlaneMedium3_BulletPattern_A : BulletFactory, IBulletPattern
{
    private readonly EnemyPlaneMedium3 _typedEnemyObject;
    private readonly int _appearanceTime;

    public EnemyPlaneMedium3_BulletPattern_A(EnemyObject enemyObject, int appearanceTime) : base(enemyObject)
    {
        _typedEnemyObject = enemyObject as EnemyPlaneMedium3;
        _appearanceTime = appearanceTime;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 2400, 1800, 1200 };
        yield return new WaitForMillisecondFrames(_appearanceTime + Random.Range(-500, 500));

        while(!_enemyObject.TimeLimitState) {
            var dir = Random.Range(-2f, 2f);
            
            if (_typedEnemyObject.m_Turret[0] != null) {
                _typedEnemyObject.m_Turret[0].StartPattern("A", new EnemyPlaneMedium3_BulletPattern_Turret_A(_typedEnemyObject.m_Turret[0]));
            }
            if (_typedEnemyObject.m_Turret[1] != null) {
                _typedEnemyObject.m_Turret[1].StartPattern("A", new EnemyPlaneMedium3_BulletPattern_Turret_A(_typedEnemyObject.m_Turret[1]));
            }

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 3; i++)
                {
                    var pos = _enemyObject.transform.position;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 6f, BulletPivot.Current, dir));
                    yield return new WaitForFrames(3);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    var pos = _enemyObject.transform.position;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 7f, BulletPivot.Current, dir, 3, 10f));
                    yield return new WaitForFrames(2);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    var pos = _enemyObject.transform.position;
                    CreateBullet(new BulletProperty(pos, BulletImage.PinkSmall, 8.5f, BulletPivot.Current, dir, 3, 10f));
                    yield return new WaitForFrames(2);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty] * Random.Range(85, 115) / 100);
        }
        onCompleted?.Invoke();
    }
}

public class EnemyPlaneMedium3_BulletPattern_Turret_A : BulletFactory, IBulletPattern
{
    public EnemyPlaneMedium3_BulletPattern_Turret_A(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var dir = Random.Range(-2f, 2f);
        _enemyObject.SetRotatePattern(new RotatePattern_Stop());

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 3; i++) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 6f, BulletPivot.Current, dir));
                yield return new WaitForFrames(3);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 4; i++) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 7f, BulletPivot.Current, dir));
                yield return new WaitForFrames(2);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueNeedle, 8f, BulletPivot.Current, dir, 3, 12f));
                yield return new WaitForFrames(2);
            }
        }
        _enemyObject.SetRotatePattern(new RotatePattern_TargetPlayer());
        onCompleted?.Invoke();
    }
}
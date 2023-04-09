using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium4 : EnemyUnit
{
    public EnemyPlaneMedium4Turret[] m_Turret = new EnemyPlaneMedium4Turret[2];
    private int[] m_FireDelay = { 1600, 1600, 1000 };
    
    private bool m_TimeLimitState = false;
    private const int APPEARNCE_TIME = 1500;
    private const int TIME_LIMIT = 12500;
    private float m_VSpeed = 0.4f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 3.8f;

        StartCoroutine(Pattern1());

        StartCoroutine(AppearanceSequence());
        
        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -1.8f + m_VSpeed*APPEARNCE_TIME, APPEARNCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));*/
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARNCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARNCE_TIME / 2) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, m_VSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;

        float init_speed = m_MoveVector.speed;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override void Update()
    {
        if (!m_TimeLimitState) { // Retreat when boss or middle boss state
            if (m_SystemManager.m_PlayState > 0) {
                if (m_TimeLimit != null)
                    StopCoroutine(m_TimeLimit);
                m_TimeLimit = TimeLimit();
                StartCoroutine(m_TimeLimit);
                m_TimeLimitState = true;
            }
        }
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        yield return new WaitForMillisecondFrames(APPEARNCE_TIME);

        while(!m_TimeLimitState) {
            m_Turret[0].StartPattern();
            m_Turret[1].StartPattern();
            yield return new WaitForMillisecondFrames(2000);

            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
            
            if (m_SystemManager.GetDifficulty() <= 1) {
                for (int i = 0; i < 10; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.GetPlayerPosition());
                    random_value = Random.Range(-1f, 1f);
                    CreateBullet(2, pos, 7.2f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(60 * 5);
            }
            else {
                for (int i = 0; i < 15; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.GetPlayerPosition());
                    random_value = Random.Range(-1f, 1f);
                    CreateBulletsSector(0, pos, 6.8f+i*0.7f, target_angle + random_value, accel, 2, 24f);
                    CreateBullet(2, pos, 6.6f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(1f, 1f));
        ExplosionEffect(0, -1, new Vector2(-1f, 1f));
        ExplosionEffect(0, -1, new Vector2(0f, -1.5f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium4 : EnemyUnit
{
    public EnemyPlaneMedium4Turret[] m_Turret = new EnemyPlaneMedium4Turret[2];
    private int[] m_FireDelay = { 1600, 1600, 1000 };
    
    private const int APPEARANCE_TIME = 1500;
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
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -1.8f + m_VSpeed*APPEARANCE_TIME, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));*/
    }

    public IEnumerator AppearanceSequence() {
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
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        while(!TimeLimitState) {
            m_Turret[0].StartPattern();
            m_Turret[1].StartPattern();
            yield return new WaitForMillisecondFrames(2000);

            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
            
            if (SystemManager.Difficulty <= GameDifficulty.Expert) {
                for (int i = 0; i < 10; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                    random_value = Random.Range(-1f, 1f);
                    CreateBullet(2, pos, 7.2f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(60 * 5);
            }
            else {
                for (int i = 0; i < 15; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                    random_value = Random.Range(-1f, 1f);
                    CreateBulletsSector(0, pos, 6.8f+i*0.7f, target_angle + random_value, accel, 2, 24f);
                    CreateBullet(2, pos, 6.6f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
        yield break;
    }
}

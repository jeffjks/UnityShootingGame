using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3 : EnemyUnit
{
    public EnemyPlaneLarge3Turret m_Turret;
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 1900, 1400, 1100 };
    
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 14000;
    //private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.3f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 4f;
        
        StartCoroutine(Pattern1());

        StartCoroutine(AppearanceSequence());

        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -1.8f + m_VSpeed*APPEARANCE_TIME, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));
        */
    }

    public IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARANCE_TIME / 2) * Application.targetFrameRate / 1000;

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
        m_Turret.StopPattern1();

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
        base.Update();
        
        if (!m_TimeLimitState) { // Retreat when boss or middle boss state
            if (m_SystemManager.m_PlayState > 0) {
                if (m_TimeLimit != null)
                    StopCoroutine(m_TimeLimit);
                m_TimeLimit = TimeLimit();
                StartCoroutine(m_TimeLimit);
                m_TimeLimitState = true;
            }
        }
    }

    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3[] pos = new Vector3[2];
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        while(!m_TimeLimitState) {
            if (m_SystemManager.GetDifficulty() == 0) {
                for (int i = 0; i < 3; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 3, 29f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 3, 29f);
                    yield return new WaitForMillisecondFrames(400);
                }
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 4; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 5, 21f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 5, 21f);
                    yield return new WaitForMillisecondFrames(300);
                }
            }
            else {
                for (int i = 0; i < 5; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 5, 21f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 5, 21f);
                    yield return new WaitForMillisecondFrames(240);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium2 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[5];

    private const int APPEARANCE_TIME = 1600;
    private const int TIME_LIMIT = 8000;
    private float m_VSpeed = 1.1f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 4.4f;

        StartCoroutine(Pattern1());
        StartCoroutine(Pattern2());

        StartCoroutine(AppearanceSequence());

        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2.5f + m_VSpeed*APPEARANCE_TIME, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));*/
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
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);
        while(!m_TimeLimitState) {
            Vector3[] pos = new Vector3[m_FirePosition.Length];
            float target_angle;

            if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, m_CurrentAngle, accel);
                    CreateBullet(4, pos[2], 6.2f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(1000);
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
                pos[0] = m_FirePosition[0].position;
                CreateBullet(3, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                CreateBullet(5, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                yield return new WaitForMillisecondFrames(2000);
            }

            else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, 0f, accel);
                    CreateBullet(4, pos[2], 6.2f, 0f, accel);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(800);
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
                pos[0] = m_FirePosition[0].position;
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                    CreateBullet(5, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                }
                yield return new WaitForMillisecondFrames(1600);
            }

            else {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, 0f, accel);
                    CreateBullet(4, pos[2], 6.2f, 0f, accel);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(600);

                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[3], 6.2f, 0f, accel);
                    CreateBullet(4, pos[4], 6.2f, 0f, accel);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(600);
            }
        }
        yield break;
    }

    private IEnumerator Pattern2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float target_angle;

        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);
        while(!m_TimeLimitState) {
            yield return new WaitForMillisecondFrames(Random.Range(0, 1500));

            if (m_SystemManager.GetDifficulty() == GameDifficulty.Hell) {
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
                pos = m_FirePosition[0].position;
                for (int i = 0; i < 5; i++) {
                    CreateBullet(3, pos, 5f + Random.Range(0f, 1.8f), target_angle + Random.Range(-24f, 24f), accel);
                    CreateBullet(5, pos, 5f + Random.Range(0f, 1.8f), target_angle + Random.Range(-24f, 24f), accel);
                }
            }
            yield return new WaitForMillisecondFrames(1600);
        }
        yield break;
    }

    private void SetBulletVariables(ref Vector3 pos1, ref Vector3 pos2, ref Vector3 pos3, ref Vector3 pos4) {
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;
        pos3 = m_FirePosition[3].position;
        pos4 = m_FirePosition[4].position;
    }
}

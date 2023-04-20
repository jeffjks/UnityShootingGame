using UnityEngine;
using System.Collections;

public class EnemyPlaneMedium1 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[3];
    
    private bool m_TimeLimitState = false;
    private const int APPEARANCE_TIME = 1600;
    private const int TIME_LIMIT = 10000;
    //private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.2f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 4.3f;
        
        StartCoroutine(Pattern1());

        StartCoroutine(AppearanceSequence());

        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2.2f + m_VSpeed*APPEARANCE_TIME, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));*/
    }

    private IEnumerator AppearanceSequence() {
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
        EnemyBulletAccel accel1 = new EnemyBulletAccel(7.2f, 1000);
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        while(!m_TimeLimitState) {
            Vector3 pos0 = m_FirePosition[0].position;
            Vector3 pos1 = m_FirePosition[1].position;
            Vector3 pos2 = m_FirePosition[2].position;
            float target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBulletsSector(3, pos0, 6.4f, target_angle, accel, 6, 12);
                CreateBulletsSector(5, pos0, 5.2f, target_angle, accel, 5, 12);
                yield return new WaitForMillisecondFrames(1000);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 6.0f, 0f, accel);
                    CreateBullet(1, pos2, 6.0f, 0f, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 3; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBulletsSector(3, pos0, 6.4f, target_angle + Random.Range(-3f, 3f), accel, 9, 10f);
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(1000);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 0f, accel1);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(1200);
            }
            else {
                CreateBulletsSector(3, pos0, 6.4f, target_angle - 1.5f, accel, 10, 9f);
                CreateBulletsSector(3, pos0, 6.4f, target_angle + 1.5f, accel, 10, 9f);
                yield return new WaitForMillisecondFrames(200);
                for (int i = 0; i < 4; i++) {
                    float random_value = Random.Range(-3f, 3f);
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBulletsSector(3, pos0, 6.4f, target_angle + random_value, accel, 9, 10f);
                    
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(100);
                SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                CreateBulletsSector(5, pos0, 6.4f, target_angle, accel, 13, 8f);
                yield return new WaitForMillisecondFrames(1000);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 4.2f, -40f, accel1);
                    CreateBullet(1, pos1, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 40f, accel1);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(1200);
            }
        }
        yield break;
    }

    private void SetBulletVariables(ref Vector3 pos0, ref Vector3 pos1, ref Vector3 pos2, ref float target_angle) {
        pos0 = m_FirePosition[0].position;
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;
        target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(2f, 0f));
        ExplosionEffect(0, -1, new Vector2(-2f, 0f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}

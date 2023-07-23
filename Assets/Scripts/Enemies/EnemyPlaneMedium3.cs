using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium3 : EnemyUnit
{
    public EnemyPlaneMedium3Turret[] m_Turret = new EnemyPlaneMedium3Turret[2];
    private int[] m_FireDelay = { 2400, 1800, 1200 };
    
    private const int APPEARANCE_TIME = 1200;
    private const int TIME_LIMIT = 7800;
    private float m_VSpeed = 1.2f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 5.4f;

        StartCoroutine(Pattern1());

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += DestroyTurrets;

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
        m_TimeLimitState = true;

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
        
        if (!m_TimeLimitState) { // Retreat when boss or middle boss state
            if (SystemManager.PlayState != PlayState.OnField) {
                if (m_TimeLimit != null)
                    StopCoroutine(m_TimeLimit);
                m_TimeLimit = TimeLimit();
                StartCoroutine(m_TimeLimit);
                m_TimeLimitState = true;
            }
        }
    }
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME + Random.Range(-500, 500));

        while(!m_TimeLimitState) {
            pos = transform.position;
            target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
            random_value = Random.Range(-2f, 2f);
            if (m_Turret[0] != null) {
                m_Turret[0].StartPattern();
            }
            if (m_Turret[1] != null) {
                m_Turret[1].StartPattern();
            }

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 3; i++) {
                    CreateBullet(2, pos, 6f, target_angle + random_value, accel);
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(2, pos, 7f, target_angle + random_value, accel, 3, 10f);
                    yield return new WaitForMillisecondFrames(43);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(2, pos, 8.5f, target_angle + random_value, accel, 3, 10f);
                    yield return new WaitForMillisecondFrames(35);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty] * Random.Range(85, 115) / 100);
        }
        yield break;
    }

    private void DestroyTurrets() {
        m_Turret[0].m_EnemyDeath.OnDying();
        m_Turret[1].m_EnemyDeath.OnDying();
    }
}
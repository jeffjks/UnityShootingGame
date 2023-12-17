using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3 : EnemyUnit
{
    public EnemyPlaneLarge3_Turret m_Turret;
    
    private const int APPEARANCE_TIME = 1700;
    private const int TIME_LIMIT = 12000;
    //private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.06f;
    private IEnumerator _timeLimitCoroutine;

    private void Start()
    {
        m_MoveVector.speed = 4f;

        StartPattern("A", new EnemyPlaneLarge3_BulletPattern_A(this));

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
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        m_Turret.StopAllPatterns();

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
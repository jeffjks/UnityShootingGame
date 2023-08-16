using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyPlaneMedium5 : EnemyUnit
{
    public EnemyUnit m_FrontTurret;
    public EnemyUnit m_BackTurret;
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 8500;
    private IEnumerator m_TimeLimit;
    private int _side;

    void Start ()
    {
        if (transform.position.x < 0)
            _side = -1;
        else
            _side = 1;
        m_MoveVector = new MoveVector(5.25f, -72f * _side); // 원래 7f에 1000ms 대기 없었음

        StartCoroutine(AppearanceSequence());
        SetRotatePattern(new RotatePattern_MoveDirection());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(750);

        float init_speed = m_MoveVector.speed;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 0f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        //TimeLimitState = true;
        m_FrontTurret.StopPattern("A");
        m_BackTurret.StopPattern("A");

        MoveVector init_moveVector = m_MoveVector;
        int frame = 1500 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            float t_dir = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_moveVector.speed, 6.4f, t_spd);
            m_MoveVector.direction = Mathf.Lerp(init_moveVector.direction, 96f * _side, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }
    }
}

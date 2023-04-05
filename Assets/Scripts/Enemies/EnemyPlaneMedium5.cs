using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5 : EnemyUnit
{
    public EnemyPlaneMedium5Turret0 m_Turret0;
    public EnemyPlaneMedium5Turret1 m_Turret1;
    //private bool m_TimeLimitState = false;
    private const int APPEARNCE_TIME = 3000;
    private const int TIME_LIMIT = 8500;
    private IEnumerator m_TimeLimit;
    private int m_Side;

    void Start ()
    {
        if (transform.position.x < 0)
            m_Side = -1;
        else
            m_Side = 1;
        m_MoveVector = new MoveVector(7f, -72f * m_Side);

        StartCoroutine(AppearanceSequence());

        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.AppendInterval(2.2f);
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, APPEARNCE_TIME).SetEase(Ease.OutQuad);*/
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(2200);

        float init_speed = m_MoveVector.speed;
        int frame = APPEARNCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 0f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        //m_TimeLimitState = true;
        m_Turret0.StopPattern();
        m_Turret1.StopPattern();

        MoveVector init_moveVector = m_MoveVector;
        int frame = 1500 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            float t_dir = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_moveVector.speed, 6.4f, t_spd);
            m_MoveVector.direction = Mathf.Lerp(init_moveVector.direction, 96f * m_Side, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);
        
        base.Update();
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(0f, 1.5f));
        ExplosionEffect(0, -1, new Vector2(0f, -2f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3 : EnemyUnit
{
    public EnemyPlaneLarge3Turret m_Turret;
    public Transform[] m_FirePositionBarrel = new Transform[2];
    private readonly int[] m_FireDelay = { 1600, 1200, 1000 };
    
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
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, m_VSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        //m_Turret.StopPattern1();

        float init_speed = m_MoveVector.speed;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
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

    
    private IEnumerator Pattern1()
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(2300);

        while(true) {
            Vector3 pos;
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 2; i++)
                {
                    pos = m_Turret.m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 7.2f, CurrentAngle, accel, 6, 25f);
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 6.0f, CurrentAngle + Random.Range(-8f, 8f), accel, 7, 23f);
                    CreateBulletsSector(3, pos, 7.2f, CurrentAngle + Random.Range(-8f, 8f), accel, 6, 23f);
                    yield return new WaitForMillisecondFrames(800);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 3; i++)
                {
                    pos = m_Turret.m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 7.2f, CurrentAngle, accel, 8, 20f);
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 6.0f, CurrentAngle + Random.Range(-5f, 5f), accel, 9, 17f);
                    CreateBulletsSector(5, pos, 7.2f, CurrentAngle + Random.Range(-5f, 5f), accel, 8, 17f);
                    yield return new WaitForMillisecondFrames(500);
                }
            }
            else {
                for (int i = 0; i < 3; i++)
                {
                    pos = m_Turret.m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 6.0f, CurrentAngle, accel, 11, 16f);
                    CreateBulletsSector(5, pos, 7.2f, CurrentAngle, accel, 10, 16f);
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 6.0f, CurrentAngle + Random.Range(-5f, 5f), accel, 11, 14f);
                    CreateBulletsSector(5, pos, 7.2f, CurrentAngle + Random.Range(-5f, 5f), accel, 10, 14f);
                    yield return new WaitForMillisecondFrames(500);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);

            bool rand = Random.Range(0, 2) == 0;
            
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++)
                {
                    CreateBulletsSector(1, m_FirePositionBarrel[0].position, 7.5f, CurrentAngle, accel, rand ? 5 : 6, 19f);
                    CreateBulletsSector(1, m_FirePositionBarrel[1].position, 7.5f, CurrentAngle, accel, rand ? 5 : 6, 19f);
                    yield return new WaitForMillisecondFrames(180);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 6; i++)
                {
                    CreateBulletsSector(1, m_FirePositionBarrel[0].position, 8f, CurrentAngle, accel, rand ? 7 : 8, 12f);
                    CreateBulletsSector(1, m_FirePositionBarrel[1].position, 8f, CurrentAngle, accel, rand ? 7 : 8, 12f);
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else {
                for (int i = 0; i < 8; i++)
                {
                    CreateBulletsSector(1, m_FirePositionBarrel[0].position, 8.3f, CurrentAngle, accel, rand ? 7 : 8, 12f);
                    CreateBulletsSector(1, m_FirePositionBarrel[1].position, 8.3f, CurrentAngle, accel, rand ? 7 : 8, 12f);
                    yield return new WaitForMillisecondFrames(100);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty] + 500);
        }
    }
}

            
/*
if (SystemManager.Difficulty == GameDifficulty.Normal) {
    for (int i = 0; i < 3; i++) {
        pos[0] = m_FirePosition[0].position;
        pos[1] = m_FirePosition[1].position;
        CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 3, 29f);
        CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 3, 29f);
        yield return new WaitForMillisecondFrames(400);
    }
}
else if (SystemManager.Difficulty == GameDifficulty.Expert) {
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
}*/
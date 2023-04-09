using UnityEngine;
using System.Collections;

public class EnemyGunship : HasTargetPosition
{
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 2000, 1500, 1000 };
    
    private bool m_TimeLimitState = false;
    private const int TIME_LIMIT = 8000;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        float leave_direction = Mathf.Sign(transform.position.x);
        if (leave_direction == 1) {
            m_MoveVector.direction = Random.Range(80f, 100f);
        }
        else {
            m_MoveVector.direction = Random.Range(-80f, -100f);
        }
        
        float init_speed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5.4f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1200);
        
        while (!m_TimeLimitState) {
            for (int i = 0; i < 4; i++) {
                Vector3 pos1 = m_FirePosition[0].position;
                Vector3 pos2 = m_FirePosition[1].position;
                if (m_SystemManager.GetDifficulty() == 0) {
                    CreateBullet(4, pos1, 6.7f, m_CurrentAngle + 2.5f, accel);
                    CreateBullet(4, pos2, 6.7f, m_CurrentAngle - 2.5f, accel);
                    break;
                }
                else if (m_SystemManager.GetDifficulty() == 1) {
                    CreateBullet(4, pos1, 8f, m_CurrentAngle + 2.5f, accel);
                    CreateBullet(4, pos2, 8f, m_CurrentAngle - 2.5f, accel);
                }
                else {
                    CreateBullet(4, pos1, 8.5f, m_CurrentAngle + 2.5f, accel);
                    CreateBullet(4, pos1, 8.5f, m_CurrentAngle + 9f, accel);
                    CreateBullet(4, pos2, 8.5f, m_CurrentAngle - 9f, accel);
                    CreateBullet(4, pos2, 8.5f, m_CurrentAngle - 2.5f, accel);
                }
                yield return new WaitForMillisecondFrames(140);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }
}

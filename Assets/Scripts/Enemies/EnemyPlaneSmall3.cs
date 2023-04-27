using UnityEngine;
using System.Collections;

public class EnemyPlaneSmall3 : HasTargetPosition
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 4000, 2000, 1700 };
    
    private IEnumerator m_TimeLimit;
    private const int TIME_LIMIT = 5000;

    void Start()
    {
        StartCoroutine(Pattern1(1000));
        RotateImmediately(m_PlayerPosition);
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    protected override void Update()
    {
        base.Update();
        
        if (!m_TimeLimitState) {
            if (m_PlayerManager.m_PlayerIsAlive)
                RotateImmediately(m_PlayerPosition);
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);

        float init_speed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 8f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Pattern1(int millisecond) {
        float gap = 0.3f;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            if (!m_TimeLimitState) {
                if (m_SystemManager.GetDifficulty() <= 1) {
                    Vector3 pos1 = m_FirePosition.TransformPoint(Vector3.right * gap);
                    Vector3 pos2 = m_FirePosition.TransformPoint(Vector3.left * gap);

                    CreateBullet(3, pos1, 4f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 4f, m_CurrentAngle, accel);
                    CreateBullet(3, pos1, 6f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 6f, m_CurrentAngle, accel);
                }
                else {
                    Vector3 pos1 = m_FirePosition.TransformPoint(Vector3.right * gap);
                    Vector3 pos2 = m_FirePosition.TransformPoint(Vector3.left * gap);

                    CreateBullet(3, pos1, 4f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 4f, m_CurrentAngle, accel);
                    CreateBullet(3, pos1, 5f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 5f, m_CurrentAngle, accel);
                    CreateBullet(3, pos1, 6f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 6f, m_CurrentAngle, accel);
                    CreateBullet(3, pos1, 7f, m_CurrentAngle, accel);
                    CreateBullet(3, pos2, 7f, m_CurrentAngle, accel);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
    }
}

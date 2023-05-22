using UnityEngine;
using System.Collections;

public class EnemyGunship : EnemyUnit, ITargetPosition
{
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 2000, 1500, 1000 };
    
    private const int TIME_LIMIT = 8000;

    void Start()
    {
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    public void MoveTowardsToTarget(Vector2 target_vec2, int duration) {
        StartCoroutine(MoveTowardsToTargetSequence(target_vec2, duration));
    }

    private IEnumerator MoveTowardsToTargetSequence(Vector2 target_vec2, int duration) {
        Vector3 init_position = transform.position;
        Vector3 target_position = new Vector3(target_vec2.x, target_vec2.y, Depth.ENEMY);
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
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
                if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
                    CreateBullet(4, pos1, 6.7f, m_CurrentAngle + 2.5f, accel);
                    CreateBullet(4, pos2, 6.7f, m_CurrentAngle - 2.5f, accel);
                    break;
                }
                else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
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
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }
}

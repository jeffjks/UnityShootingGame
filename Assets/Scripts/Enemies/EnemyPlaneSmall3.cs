using UnityEngine;
using System.Collections;

public class EnemyPlaneSmall3 : EnemyUnit, ITargetPosition
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 4000, 2000, 1700 };
    
    private IEnumerator m_TimeLimit;
    private const int TIME_LIMIT = 5000;

    void Start()
    {
        StartCoroutine(Pattern1(1000));
        RotateImmediately(PlayerManager.GetPlayerPosition());
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    protected override void Update()
    {
        base.Update();
        
        if (!m_TimeLimitState) {
            if (PlayerManager.IsPlayerAlive)
                RotateImmediately(PlayerManager.GetPlayerPosition());
            else
                RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        }
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
        m_MoveVector.direction = GetAngleToTarget(m_Position2D, PlayerManager.GetPlayerPosition());

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
                if (SystemManager.Difficulty <= GameDifficulty.Expert) {
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
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

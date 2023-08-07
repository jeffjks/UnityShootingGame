using UnityEngine;
using System.Collections;

public class EnemyPlaneSmall3 : EnemyUnit, ITargetPosition
{
    private int[] m_FireDelay = { 4000, 2000, 1700 };
    
    private IEnumerator m_TimeLimit;
    private const int TIME_LIMIT = 5000;

    void Start()
    {
        StartCoroutine(Pattern1(1000));
        RotateUnit(AngleToPlayer);
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    protected override void Update()
    {
        base.Update();
        
        if (!TimeLimitState) {
            if (PlayerManager.IsPlayerAlive)
                RotateUnit(AngleToPlayer);
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
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        m_MoveVector.direction = AngleToPlayer;

        float init_speed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 8f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Pattern1(int millisecond) {
        float gap = 0.3f;
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            if (!TimeLimitState) {
                if (SystemManager.Difficulty <= GameDifficulty.Expert) {
                    Vector3 pos1 = m_FirePosition[0].TransformPoint(Vector3.right * gap);
                    Vector3 pos2 = m_FirePosition[0].TransformPoint(Vector3.left * gap);

                    CreateBullet(3, pos1, 4f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 4f, CurrentAngle, accel);
                    CreateBullet(3, pos1, 6f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 6f, CurrentAngle, accel);
                }
                else {
                    Vector3 pos1 = m_FirePosition[0].TransformPoint(Vector3.right * gap);
                    Vector3 pos2 = m_FirePosition[0].TransformPoint(Vector3.left * gap);

                    CreateBullet(3, pos1, 4f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 4f, CurrentAngle, accel);
                    CreateBullet(3, pos1, 5f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 5f, CurrentAngle, accel);
                    CreateBullet(3, pos1, 6f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 6f, CurrentAngle, accel);
                    CreateBullet(3, pos1, 7f, CurrentAngle, accel);
                    CreateBullet(3, pos2, 7f, CurrentAngle, accel);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

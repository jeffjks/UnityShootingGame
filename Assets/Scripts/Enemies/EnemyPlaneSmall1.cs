using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall1 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1000, 500 };

    private bool m_TargetPlayer = true;
    private float m_Speed = 6.8f;

    void Start()
    {
        StartCoroutine(Pattern1(800));
        RotateImmediately(m_PlayerPosition);
        float target_angle = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        m_MoveVector = new MoveVector(m_Speed, target_angle);
    }

    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);

        if (m_TargetPlayer) {
            float player_distance = ((Vector2) transform.position - m_PlayerPosition).magnitude;
            m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);

            if (player_distance < 5f) {
                m_TargetPlayer = false;
            }
        }
    }

    private IEnumerator Pattern1(int millisecond) {
        float[] speed = {7.7f, 9.1f, 9.1f};
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            Vector3 pos = m_FirePosition.position;
            EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
            CreateBullet(1, pos, speed[(int) m_SystemManager.GetDifficulty()], m_CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) m_SystemManager.GetDifficulty()]);
        }
    }
}

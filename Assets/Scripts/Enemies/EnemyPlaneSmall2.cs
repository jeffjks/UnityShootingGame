using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall2 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 5000, 2100, 1200 };

    private bool m_TargetPlayer = true;
    private float m_Speed = 7.2f;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1(800));
        RotateImmediately(m_PlayerPosition);
        float target_angle = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        m_MoveVector = new MoveVector(m_Speed, target_angle);
    }

    protected override void Update()
    {
        if (m_TargetPlayer) {
            float player_distance = ((Vector2) transform.position - m_PlayerPosition).magnitude;
            m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);

            if (player_distance < 5f) {
                m_TargetPlayer = false;
            }
        }
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_MoveVector.direction);
        else
            RotateSlightly(m_MoveVector.direction, 100f);
        
        base.Update();
    }

    private IEnumerator Pattern1(int millisecond) {
        float[] speed = {8.2f, 9.8f, 9.8f};
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            Vector3 pos = m_FirePosition.position;
            EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
            CreateBullet(0, pos, speed[m_SystemManager.GetDifficulty()], m_CurrentAngle, accel);
            CreateBulletsSector(2, pos, speed[m_SystemManager.GetDifficulty()], m_CurrentAngle, accel, 2, 28f);
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
    }
}

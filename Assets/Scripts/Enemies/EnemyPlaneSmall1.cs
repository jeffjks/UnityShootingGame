using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall1 : EnemyUnit
{
    public Transform m_FirePosition;
    public Transform m_Rotator;
    private int[] m_FireDelay = { 2000, 1000, 500 };

    private bool m_TargetPlayer = true;
    private float m_Speed = 6.8f;
    private float m_MaxTilt = 30f;
    private float m_CurrentTilt;

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
        RotateImmediately(m_MoveVector.direction);

        Vector2 previous_vector = m_MoveVector.GetVector();
        float target_tilt;

        if (m_TargetPlayer) {
            float player_distance = ((Vector2) transform.position - m_PlayerPosition).magnitude;
            m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);

            if (player_distance < 5f) {
                m_TargetPlayer = false;
            }
        }

        Vector2 after_vector = m_MoveVector.GetVector();
        
        if (previous_vector == after_vector)
            target_tilt = 0;
        else
            target_tilt = Mathf.Sign(Vector2.SignedAngle(previous_vector, after_vector)) * m_MaxTilt;
        m_CurrentTilt = Mathf.MoveTowards(m_CurrentTilt, target_tilt, 72f / Application.targetFrameRate * Time.timeScale);
        
        Turn(m_CurrentTilt);
        
        base.Update();
    }

    private void Turn(float angle) {
        m_Rotator.localRotation = Quaternion.AngleAxis(angle, Vector3.down);
    }

    private IEnumerator Pattern1(int millisecond) {
        float[] speed = {7.7f, 9.1f, 9.1f};
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            Vector3 pos = m_FirePosition.position;
            EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
            CreateBullet(1, pos, speed[m_SystemManager.m_Difficulty], m_CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

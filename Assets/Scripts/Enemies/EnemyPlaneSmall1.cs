using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall1 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private Transform m_Rotator = null;

    private bool m_TargetPlayer = true;
    private float m_Speed = 6.4f;

    void Start()
    {
        GetCoordinates();
        InvokeRepeating("Pattern1", 0.8f, m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);
        float target_angle = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        m_MoveVector = new MoveVector(m_Speed, target_angle);
    }

    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);

        Vector2 previous_vector = m_MoveVector.GetVector();

        if (m_TargetPlayer == true) {
            float player_distance = ((Vector2) transform.position - m_PlayerPosition).magnitude;
            m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);

            if (player_distance < 5f) {
                m_TargetPlayer = false;
            }
        }

        Vector2 after_vector = m_MoveVector.GetVector();
        float max_tilt = 30f;
        float max_angle = 0.6f; // -0.5 -> 0, 0.5 -> 1
        float angle = Vector2.SignedAngle(previous_vector, after_vector) / (max_angle * 2) + max_angle / 2;

        float tilt = Mathf.Lerp(-max_tilt, max_tilt, angle);
        Turn(tilt);
        
        base.Update();
    }

    private void Turn(float angle) {
        m_Rotator.localRotation = Quaternion.AngleAxis(angle, Vector3.down);
    }

    private void Pattern1() {
        Vector3 pos = m_FirePosition.position;
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(1, pos, 6.4f, m_CurrentAngle, accel);
    }
}

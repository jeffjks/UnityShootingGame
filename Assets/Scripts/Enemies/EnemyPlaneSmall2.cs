using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall2 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private Transform m_Rotator = null;

    private bool m_TargetPlayer = true;

    void Start()
    {
        GetCoordinates();
        InvokeRepeating("Pattern1", 0.8f, m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);
        float target_angle = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        m_MoveVector = new MoveVector(5.2f, target_angle);
    }

    protected override void Update()
    {
        Vector2 previous_vector = m_MoveVector.GetVector();

        if (m_TargetPlayer == true) {
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

        Vector2 after_vector = m_MoveVector.GetVector();
        float max_tilt = 30f;
        float max_rotation = 0.6f; // -0.5 -> 0, 0.5 -> 1
        float tilt_lerp = Vector2.SignedAngle(previous_vector, after_vector) / (max_rotation * 2) + max_rotation / 2;

        float tilt = Mathf.Lerp(-max_tilt, max_tilt, tilt_lerp);
        Turn(tilt);
        
        base.Update();
    }

    private void Turn(float angle) {
        m_Rotator.localRotation = Quaternion.AngleAxis(angle, Vector3.down);
    }

    private void Pattern1() {
        Vector3 pos = m_FirePosition.position;
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(0, pos, 6f, m_CurrentAngle, accel);
        CreateBulletsSector(2, pos, 6f, m_CurrentAngle, accel, 2, 28f);
    }
}

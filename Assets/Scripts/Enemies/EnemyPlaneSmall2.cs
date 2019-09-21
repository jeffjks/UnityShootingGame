using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall2 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private Transform m_Rotator = null;

    private bool m_TargetPlayer = true;
    private float m_Speed = 7.2f;
    private float m_MaxTilt = 30f;
    private float m_CurrentTilt;

    void Start()
    {
        GetCoordinates();
        InvokeRepeating("Pattern1", 0.8f, m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);
        float target_angle = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        m_MoveVector = new MoveVector(m_Speed, target_angle);
    }

    protected override void FixedUpdate()
    {
        Vector2 previous_vector = m_MoveVector.GetVector();
        float target_tilt;

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

        Vector2 after_vector = m_MoveVector.GetVector();
        
        if (previous_vector == after_vector)
            target_tilt = 0;
        else
            target_tilt = Mathf.Sign(Vector2.SignedAngle(previous_vector, after_vector)) * m_MaxTilt;
        m_CurrentTilt = Mathf.MoveTowards(m_CurrentTilt, target_tilt, 72f*Time.fixedDeltaTime);
        
        Turn(m_CurrentTilt);
        
        base.FixedUpdate();
    }

    private void Turn(float angle) {
        m_Rotator.localRotation = Quaternion.AngleAxis(angle, Vector3.down);
    }

    private void Pattern1() {
        Vector3 pos = m_FirePosition.position;
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(0, pos, 7.5f, m_CurrentAngle, accel);
        CreateBulletsSector(2, pos, 7.5f, m_CurrentAngle, accel, 2, 28f);
    }
}

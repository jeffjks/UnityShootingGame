using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyPlaneSmall3 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private Transform m_Rotator = null;
    
    private bool m_TimeLimitState = false;

    void Start()
    {
        float time_limit = 5f;
        
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);

        InvokeRepeating("Pattern1", 1f, m_FireDelay[m_SystemManager.m_Difficulty]);
        Invoke("TimeLimit", time_limit);
    }

    protected override void Update()
    {
        Vector2 previous_vector = m_MoveVector.GetVector();

        if (!m_TimeLimitState) {
            if (m_PlayerManager.m_PlayerIsAlive)
                RotateImmediately(m_PlayerPosition);
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }

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
    

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 8f, 0.8f).SetEase(Ease.OutQuad);
    }

    private void Pattern1() {
        Vector3 pos = m_FirePosition.position;
        float gap = 0.3f;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        if (!m_TimeLimitState) {
            if (m_SystemManager.m_Difficulty <= 1) {
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
    }
}

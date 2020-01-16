using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret3Turret : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private Transform m_TurretAnimation = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    private float m_InitaialTurretPosition, m_CurrentTurretPosition, m_TargetTurretPosition = -1f;
    private bool m_Active = false;

    void Start()
    {
        GetCoordinates();
        m_InitaialTurretPosition = m_TurretAnimation.localPosition.z;
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                InvokeRepeating("Pattern1", Random.Range(0f, m_FireDelay[m_SystemManager.m_Difficulty]), m_FireDelay[m_SystemManager.m_Difficulty]);
                m_Active = true;
            }
        }

        m_CurrentTurretPosition = Mathf.MoveTowards(m_CurrentTurretPosition, m_InitaialTurretPosition, 0.02f);
        m_TurretAnimation.localPosition = new Vector3(m_TurretAnimation.localPosition.x, m_TurretAnimation.localPosition.y, m_CurrentTurretPosition);
        
        base.Update();
    }

    private void Pattern1() {
        Vector3 pos1 = GetScreenPosition(m_FirePosition[0].position);
        Vector3 pos2 = GetScreenPosition(m_FirePosition[1].position);
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 4; i++) {
                CreateBullet(4, pos1, 4.8f + i*0.3f, m_CurrentAngle, accel);
                CreateBullet(4, pos2, 4.8f + i*0.3f, m_CurrentAngle, accel);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                CreateBullet(4, pos1, 4.6f + i*0.3f, m_CurrentAngle + 2f, accel);
                CreateBullet(4, pos2, 4.6f + i*0.3f, m_CurrentAngle - 2f, accel);
                CreateBullet(4, pos1, 5f + i*0.3f, m_CurrentAngle - 1.5f, accel);
                CreateBullet(4, pos2, 5f + i*0.3f, m_CurrentAngle + 1.5f, accel);
            }
        }
        m_CurrentTurretPosition = m_TargetTurretPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret2Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    
    private bool m_Active = false;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 90f);
        else
            RotateSlightly(m_PlayerPosition, 100f);

        if (2 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                InvokeRepeating("Pattern1", Random.Range(0f, m_FireDelay[m_SystemManager.m_Difficulty]), m_FireDelay[m_SystemManager.m_Difficulty]);
                m_Active = true;
            }
        }
        
        base.Update();
    }

    private void Pattern1() {
        Vector3 pos = GetScreenPosition(m_FirePosition.position);
        float[] speed = {5.7f, 6.8f, 6.8f};
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(2, pos, speed[m_SystemManager.m_Difficulty], m_CurrentAngle, accel);
    }
}

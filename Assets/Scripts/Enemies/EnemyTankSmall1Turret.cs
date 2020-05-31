using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall1Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    void Start()
    {
        GetCoordinates();
        InvokeRepeating("Pattern1", Random.Range(0f, 0.5f), m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (2 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 60f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private void Pattern1() {
        Vector3 pos = GetScreenPosition(m_FirePosition.position);
        float target_angle = Mathf.Floor((m_CurrentAngle + 5f)/10f) * 10f;
        float[] speed = {6.6f, 7.8f, 7.8f};
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(2, pos, speed[m_SystemManager.m_Difficulty], target_angle, accel);
    }
}

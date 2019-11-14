using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall2Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    void Start()
    {
        GetCoordinates();
        InvokeRepeating("Pattern1", 0f, m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 60f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private void Pattern1() {
        Vector3 pos = GetScreenPosition(m_FirePosition.position);
        float target_angle = Mathf.Floor((m_CurrentAngle + 5f)/10f) * 10f;
        
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        CreateBullet(2, pos, 5.0f, target_angle, accel);
        CreateBullet(2, pos, 5.6f, target_angle, accel);
        CreateBullet(2, pos, 6.2f, target_angle, accel);
    }
}

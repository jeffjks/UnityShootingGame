using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret0 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 2000, 1400 };

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 50f);
        else
            RotateSlightly(m_PlayerPosition, 100f);

        if (3 * m_ParentEnemy.m_Health < m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float gap = 0.32f;

        while(true) {
            if (m_SystemManager.m_Difficulty <= 1) {
                pos[0] = GetScreenPosition(m_FirePosition.position);
                pos[1] = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos[2] = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(0, pos[0], 5.6f, m_CurrentAngle, accel);
                CreateBullet(0, pos[1], 5.6f, m_CurrentAngle, accel);
                CreateBullet(0, pos[2], 5.6f, m_CurrentAngle, accel);
            }
            else {
                pos[0] = GetScreenPosition(m_FirePosition.position);
                pos[1] = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos[2] = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBulletsSector(0, pos[0], 5.6f, m_CurrentAngle, accel, 3, 2f);
                CreateBulletsSector(0, pos[1], 5.6f, m_CurrentAngle - 3f, accel, 2, 2f);
                CreateBulletsSector(0, pos[2], 5.6f, m_CurrentAngle + 3f, accel, 2, 2f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret1 : EnemyUnit
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
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);

        if (3 * m_ParentEnemy.m_Health < m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while(true) {
            if (m_SystemManager.m_Difficulty <= 1) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(2, pos, 6.2f, m_CurrentAngle, accel, 3, 20f);
            }
            else {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(2, pos, 6.6f, m_CurrentAngle, accel, 5, 13f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

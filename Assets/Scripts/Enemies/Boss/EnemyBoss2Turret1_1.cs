using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_1 : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    [HideInInspector] public bool m_InPattern = false;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        m_InPattern = true;

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 2; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.6f, m_CurrentAngle, accel, 15, 14f);
                CreateBulletsSector(4, pos, 4.4f, m_CurrentAngle, accel, 10, 14f);
                yield return new WaitForSeconds(2.4f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 3; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.3f, m_CurrentAngle, accel, 18, 10f);
                CreateBulletsSector(4, pos, 4f, m_CurrentAngle, accel, 15, 10f);
                CreateBulletsSector(4, pos, 4.7f, m_CurrentAngle, accel, 12, 10f);
                yield return new WaitForSeconds(1.6f);
            }
        }
        else {
            for (int i = 0; i < 3; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.3f, m_CurrentAngle, accel, 24, 8f);
                CreateBulletsSector(4, pos, 4f, m_CurrentAngle, accel, 19, 8f);
                CreateBulletsSector(4, pos, 4.7f, m_CurrentAngle, accel, 16, 8f);
                yield return new WaitForSeconds(1.6f);
            }
        }
        m_InPattern = false;
        yield return null;
    }
}
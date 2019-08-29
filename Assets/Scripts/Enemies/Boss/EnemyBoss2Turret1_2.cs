using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_2 : EnemyUnit
{
    public Transform m_FirePosition = null;
    
    private IEnumerator m_CurrentPattern;

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

    public void StartPattern(byte num, bool side = true) {
        if (num == 1)
            m_CurrentPattern = Pattern1(side);
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 0)
            m_CurrentPattern = Pattern0();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern0() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos = m_FirePosition.position;
        while (true) {
            if (m_PlayerManager.m_Player.transform.position.y >= -7f) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 6.6f, m_CurrentAngle, accel, 12, 2.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Pattern1(bool side)
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(5f, 0.5f);
        Vector3 pos = m_FirePosition.position;
        yield return new WaitForSeconds(1.5f);
        
        if (side)
            yield return new WaitForSeconds(0.75f);
        for (int j = 0; j < 2; j++) {
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(1.5f);
                for (int i = 0; i < 2; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 3f + i*0.7f, m_CurrentAngle, accel, 7, 13f);
                }
                break;
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 2; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 3f + i*0.7f, m_CurrentAngle, accel, 9, 11f);
                }
            }
            else {
                for (int i = 0; i < 2; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 3f + i*0.7f, m_CurrentAngle, accel, 11, 9f);
                }
            }
            yield return new WaitForSeconds(1.5f);
        }
        
        yield return null;
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float gap = 0.03f;
        pos = m_FirePosition.position;
        yield return null;
    }
}
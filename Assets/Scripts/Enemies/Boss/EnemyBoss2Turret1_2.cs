using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_2 : EnemyUnit
{
    public Transform m_FirePosition;
    
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
        if (num == 0)
            m_CurrentPattern = Pattern0();
        else if (num == 1)
            m_CurrentPattern = Pattern1(side);
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
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
        
        if (side)
            yield return new WaitForSeconds(1.5f);
        if (m_SystemManager.m_Difficulty == 0) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle, accel, 5, 20f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 20f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 20f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 40f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 40f, accel, 8, 0.8f);
            yield return new WaitForSeconds(3f);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 50f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 50f, accel, 8, 0.8f);
        }
        else {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 20f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 20f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 40f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 40f, accel, 10, 0.8f);
            yield return new WaitForSeconds(3f);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle - 50f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, m_CurrentAngle + 50f, accel, 8, 0.8f);
        }
        
        yield return null;
    }
}
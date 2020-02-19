using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4MainTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive) {
            RotateSlightly(m_PlayerPosition, 120f);
        }
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    public void StartPattern(byte num) {
        if (num == 1) {
            m_CurrentPattern1 = Pattern1A();
            m_CurrentPattern2 = Pattern1B();
        }
        else
            return;
        if (m_CurrentPattern1 != null)
            StartCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StartCoroutine(m_CurrentPattern2);
    }

    public void StopPattern() {
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
    }

    private IEnumerator Pattern1A()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos0, pos1, pos2;
        float gap = 0.64f, rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            if (m_SystemManager.m_Difficulty == 0) {
                pos0 = GetScreenPosition(m_FirePosition.position);
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 15f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 30f, accel, 2, 15f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 30f, accel, 2, 15f);
                yield return new WaitForSeconds(0.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos0 = GetScreenPosition(m_FirePosition.position);
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 12f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 26f, accel, 2, 12f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 26f, accel, 2, 12f);
                yield return new WaitForSeconds(0.55f);
            }
            else {
                pos0 = GetScreenPosition(m_FirePosition.position);
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 10f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 25f, accel, 3, 10f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 25f, accel, 3, 10f);
                yield return new WaitForSeconds(0.45f);
            }
        }
    }

    private IEnumerator Pattern1B()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            if (m_SystemManager.m_Difficulty == 0) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 4, 12f);
                yield return new WaitForSeconds(0.12f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 10f);
                yield return new WaitForSeconds(0.7f);
            }
            else {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 8f);
                yield return new WaitForSeconds(0.6f);
            }
        }
    }
}
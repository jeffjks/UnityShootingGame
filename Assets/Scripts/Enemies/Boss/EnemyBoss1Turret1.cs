using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret1 : EnemyUnit
{
    public Transform m_FirePosition = null;
    
    private IEnumerator m_CurrentPattern = null;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (((EnemyBoss1) m_ParentEnemy).m_Phase == 0) {
            if (m_PlayerManager.m_PlayerIsAlive)
                RotateImmediately(m_PlayerPosition);
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
        else {
            RotateSlightly(0f, 100f);
        }
        
        base.Update();
    }

    public void StartPattern(byte num, bool right = true) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2(right);
        else if (num == 3)
            m_CurrentPattern = Pattern3();
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
        int random_value = Random.Range(0, 2);
        pos = m_FirePosition.position;

        if (m_SystemManager.m_Difficulty == 0) {
            CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 5, 16f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            if (random_value == 0) {
                CreateBulletsSector(3, pos, 6.6f, m_CurrentAngle - 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.2f, m_CurrentAngle + 1.2f, accel, 7, 12f);
            }
            else {
                CreateBulletsSector(3, pos, 6.6f, m_CurrentAngle + 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.2f, m_CurrentAngle - 1.2f, accel, 7, 12f);
            }
        }
        else {
            if (random_value == 0) {
                CreateBulletsSector(3, pos, 6.8f, m_CurrentAngle + 2.4f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.6f, m_CurrentAngle + 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.2f, m_CurrentAngle - 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.0f, m_CurrentAngle - 2.4f, accel, 7, 12f);
            }
            else {
                CreateBulletsSector(3, pos, 6.8f, m_CurrentAngle - 2.4f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.6f, m_CurrentAngle - 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.2f, m_CurrentAngle + 1.2f, accel, 7, 12f);
                CreateBulletsSector(3, pos, 6.0f, m_CurrentAngle + 2.4f, accel, 7, 12f);
            }
        }
        yield return null;
    }

    private IEnumerator Pattern2(bool right)
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        int random_value = Random.Range(0, 2);

        if (m_SystemManager.m_Difficulty == 0) {
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.1f, m_CurrentAngle, accel, 3, 25f);
            yield return new WaitForSeconds(0.54f);
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.1f, m_CurrentAngle, accel, 3, 25f);
            yield return new WaitForSeconds(0.54f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 4, 17f);
            yield return new WaitForSeconds(0.27f);
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 4, 17f);
            yield return new WaitForSeconds(0.27f);
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 4, 17f);
            yield return new WaitForSeconds(0.27f);
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 4, 17f);
            yield return new WaitForSeconds(0.27f);
        }
        else {
            if (right) {
                for (int i = 0; i < 2; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(1, pos, 6.7f, m_CurrentAngle, accel, 4, 17f);
                    yield return new WaitForSeconds(0.27f);
                    pos = m_FirePosition.position;
                    CreateBulletsSector(1, pos, 6.7f, m_CurrentAngle, accel, 5, 17f);
                    yield return new WaitForSeconds(0.27f);
                }
            }
            else {
                for (int i = 0; i < 2; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(1, pos, 6.7f, m_CurrentAngle, accel, 5, 17f);
                    yield return new WaitForSeconds(0.27f);
                    pos = m_FirePosition.position;
                    CreateBulletsSector(1, pos, 6.7f, m_CurrentAngle, accel, 4, 17f);
                    yield return new WaitForSeconds(0.27f);
                }
            }
        }
        yield return null;
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;

        for (int i = 0; i < 2; i++) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos = m_FirePosition.position;
                CreateBullet(2, pos, 6f, 0f, accel);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                CreateBullet(2, pos, 5.5f, 0f, accel);
                CreateBullet(2, pos, 6.6f, 0f, accel);
            }
            else {
                pos = m_FirePosition.position;
                CreateBullet(2, pos, 5.8f, 0f, accel);
                CreateBullet(2, pos, 7.0f, 0f, accel);
            }
            yield return new WaitForSeconds(0.4f);
        }
        yield return null;
    }
}
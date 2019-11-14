using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret2 : EnemyUnit
{
    public float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
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

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel;
        if (m_SystemManager.m_Difficulty <= 0)
            accel = new EnemyBulletAccel(0f, 0f);
        else
            accel = new EnemyBulletAccel(8.8f, 1f);
        Vector3 pos;
        
        while(true) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(1, pos, 5f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                CreateBulletsSector(1, pos, 3f, m_CurrentAngle + Random.Range(-1f, 1f), accel, 3, 18f);
            }
            else {
                pos = m_FirePosition.position;
                CreateBulletsSector(1, pos, 3f, m_CurrentAngle + Random.Range(-1f, 1f), accel, 3, 18f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        Vector3 pos = m_FirePosition.position;
        if (m_SystemManager.m_Difficulty == 0) {
            CreateBulletsSector(0, pos, 5.75f, m_CurrentAngle, accel, 15, 24f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            CreateBulletsSector(0, pos, 5.75f, m_CurrentAngle, accel, 18, 20f);
            CreateBulletsSector(0, pos, 6.8f, m_CurrentAngle, accel, 18, 20f);
        }
        else {
            CreateBulletsSector(0, pos, 6f, m_CurrentAngle, accel, 24, 15f);
            CreateBulletsSector(0, pos, 7.1f, m_CurrentAngle, accel, 24, 15f);
        }
        yield break;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3Turret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    private int m_RotateState;
    private int m_RandomValue;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (80f < m_CurrentAngle && m_CurrentAngle < 180f) {
            m_RandomValue = 1;
        }
        else if (180f < m_CurrentAngle && m_CurrentAngle < 280f) {
            m_RandomValue = -1;
        }

        switch (m_RotateState) {
            case 0:
                RotateSlightly(m_PlayerPosition, 100f);
                break;
            case 1:
                RotateSlightly(80f*m_RandomValue, 240f);
                break;
            case 2:
                RotateSlightly(m_CurrentAngle - 10f*m_RandomValue, 100f);
                break;
        }
        
        base.Update();
    }

    public void StartPattern(byte num, int random_value = 1) {
        m_RandomValue = random_value;
        if (num == 1) {
            m_RotateState = 1;
            m_CurrentPattern = Pattern1();
        }
        else if (num == 2) {
            m_CurrentPattern = Pattern2();
        }
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        m_RotateState = 0;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        yield return new WaitForSeconds(1.5f);
        
        m_RotateState = 2;

        while(true) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle + Random.Range(-1f, 1f), accel, 8, 20.6f);
                yield return new WaitForSeconds(0.64f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 11, 15f);
                yield return new WaitForSeconds(0.32f);
            }
            else {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle + Random.Range(-5f, 5f), accel, 13, 13f);
                yield return new WaitForSeconds(0.27f);
            }
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float[] fire_delay = { 2.4f, 1.8f, 1.8f };
        float random_value;
        yield return new WaitForSeconds(1f);

        if (m_RandomValue == 0)
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]*0.5f);

        while(true) {
            pos = m_FirePosition.position;
            random_value = Random.Range(-3f, 3f);
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 4; i++)
                    CreateBullet(4, pos, 6f + i*0.7f, m_CurrentAngle + random_value, accel);
                yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++)
                    CreateBullet(4, pos, 6f + i*0.7f, m_CurrentAngle + random_value, accel);
                yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
            }
            else {
                for (int i = 0; i < 4; i++)
                    CreateBulletsSector(4, pos, 6f + i*0.7f, m_CurrentAngle + random_value, accel, 3, 18f);
                yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
            }
        }
    }
}
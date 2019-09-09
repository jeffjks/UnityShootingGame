using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3Turret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    private int m_RotateState;
    private int m_Side;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        Debug.Log(m_CurrentAngle);
        if (80f < m_CurrentAngle && m_CurrentAngle < 280f) {
            if (m_RotateState == 2)
                m_RotateState = 3;
            else if (m_RotateState == 3)
                m_RotateState = 2;
        }

        switch (m_RotateState) {
            case 0:
                RotateSlightly(m_PlayerPosition, 100f);
                break;
            case 1:
                RotateSlightly(80f*m_Side, 240f);
                break;
            case 2:
                RotateSlightly(m_CurrentAngle - 100f*m_Side, 100f);
                break;
            case 3:
                RotateSlightly(m_CurrentAngle + 100f*m_Side, 100f);
                break;
        }
        
        base.Update();
    }

    public void StartPattern(byte num, int side = 1) {
        if (num == 1) {
            m_Side = side;
            m_CurrentPattern = Pattern1();
            m_RotateState = 1;
        }
        else if (num == 2)
            m_CurrentPattern = Pattern2();
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

        if (m_Side == 1)
            m_RotateState = 2;
        else
            m_RotateState = 3;

        while(true) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 9, 18.7f);
                yield return new WaitForSeconds(0.45f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 11, 15f);
                yield return new WaitForSeconds(0.32f);
            }
            else {
                CreateBulletsSector(1, pos, 6.5f, m_CurrentAngle, accel, 13, 13f);
                yield return new WaitForSeconds(0.27f);
            }
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float gap = 0.03f;
        pos = m_FirePosition.position;

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 3f, accel);
                yield return new WaitForSeconds(0.06f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 14f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 14f, accel);
                yield return new WaitForSeconds(0.06f);
            }
        }
        else {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 18f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 10f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 10f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, m_CurrentAngle + 18f, accel);
                if (i == 5) {
                    for (int j = -1; j < 2; j += 2) {
                        pos = m_FirePosition.position;
                        CreateBullet(3, pos, 9f, m_CurrentAngle - 26f*j, accel);
                        CreateBullet(3, pos, 8.4f, m_CurrentAngle - 34f*j, accel);
                        CreateBullet(3, pos, 7.8f, m_CurrentAngle - 43f*j, accel);
                        CreateBullet(3, pos, 7.1f, m_CurrentAngle - 47f*j, accel);
                        CreateBullet(3, pos, 6.3f, m_CurrentAngle - 52f*j, accel);
                    }
                }
            yield return new WaitForSeconds(0.06f);
            }
        }
        yield return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4FrontTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    [HideInInspector] public byte m_RotatePattern = 10;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_RotatePattern == 10) {
            if (m_PlayerManager.m_PlayerIsAlive)
                RotateSlightly(m_PlayerPosition, 180f);
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
        else if (m_RotatePattern == 20) {
            RotateSlightly(0f, 150f);
        }
        
        base.Update();
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        else
            return;
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
        if (m_SystemManager.m_Difficulty == 0) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 4f, m_CurrentAngle, accel, 3, 14f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 3.5f, m_CurrentAngle, accel, 2, 9f);
            CreateBulletsSector(0, pos, 4.1f, m_CurrentAngle, accel, 3, 14f);
        }
        else {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 3.5f, m_CurrentAngle, accel, 2, 9f);
            CreateBulletsSector(0, pos, 4.1f, m_CurrentAngle, accel, 3, 14f);
        }
        yield break;
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f;
        
        if (m_SystemManager.m_Difficulty == 0) {
                pos0 = GetScreenPosition(m_FirePosition.position);
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos0, 5.4f, m_CurrentAngle, accel);
                CreateBullet(1, pos1, 5.4f, m_CurrentAngle, accel);
                CreateBullet(1, pos2, 5.4f, m_CurrentAngle, accel);
        }
        else {
            for (int i = 0; i < 5; i++) {
                if (m_SystemManager.m_Difficulty == 1) {
                    pos0 = GetScreenPosition(m_FirePosition.position);
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                    CreateBullet(1, pos0, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForSeconds(0.05f);
                }
                else {
                    pos0 = GetScreenPosition(m_FirePosition.position);
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                    CreateBullet(1, pos0, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForSeconds(0.02f);
                }
            }
        }
        yield break;
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        if (m_SystemManager.m_Difficulty == 0) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6.4f, m_CurrentAngle, accel);
            CreateBulletsSector(0, pos, 6.1f, m_CurrentAngle, accel, 2, 16f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6.6f, m_CurrentAngle, accel);
            CreateBullet(0, pos, 7.2f, m_CurrentAngle, accel);
            CreateBulletsSector(0, pos, 6.3f, m_CurrentAngle, accel, 2, 15f);
            CreateBulletsSector(0, pos, 6.9f, m_CurrentAngle, accel, 2, 16.5f);
        }
        else {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 5.8f, m_CurrentAngle, accel);
            CreateBullet(0, pos, 6.6f, m_CurrentAngle, accel);
            CreateBullet(0, pos, 7.2f, m_CurrentAngle, accel);
            CreateBulletsSector(0, pos, 5.6f, m_CurrentAngle, accel, 2, 12f);
            CreateBulletsSector(0, pos, 6.3f, m_CurrentAngle, accel, 2, 15f);
            CreateBulletsSector(0, pos, 6.9f, m_CurrentAngle, accel, 2, 16.5f);
        }
        yield break;
    }

    protected override void KilledByPlayer() {
        m_Score = 2000;
    }
}
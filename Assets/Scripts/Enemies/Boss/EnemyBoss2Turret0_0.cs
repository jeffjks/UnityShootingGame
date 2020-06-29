using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret0_0 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition;

    [HideInInspector] public bool m_InPattern = false;
    
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        m_InPattern = true;

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 3; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(5, pos, 5f, m_CurrentAngle, accel, 5, 22.5f);
                CreateBulletsSector(5, pos, 5.8f, m_CurrentAngle, accel, 6, 22.5f);
                if (i > 0)
                    CreateBulletsSector(5, pos, 6.6f, m_CurrentAngle, accel, 5, 22.5f);
                yield return new WaitForSeconds(1.2f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 4; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(5, pos, 5f, m_CurrentAngle, accel, 8, 15f);
                CreateBulletsSector(5, pos, 5.6f, m_CurrentAngle, accel, 7, 15f);
                if (i > 0)
                    CreateBulletsSector(5, pos, 6.2f, m_CurrentAngle, accel, 8, 15f);
                if (i > 1)
                    CreateBulletsSector(5, pos, 6.8f, m_CurrentAngle, accel, 7, 15f);
                if (i > 2)
                    CreateBulletsSector(5, pos, 7.4f, m_CurrentAngle, accel, 8, 15f);
                yield return new WaitForSeconds(0.9f + i*0.1f);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(5, pos, 5f, m_CurrentAngle, accel, 13, 10f);
                CreateBulletsSector(5, pos, 5.6f, m_CurrentAngle, accel, 12, 10f);
                if (i > 0)
                    CreateBulletsSector(5, pos, 6.2f, m_CurrentAngle, accel, 13, 10f);
                if (i > 1)
                    CreateBulletsSector(5, pos, 6.8f, m_CurrentAngle, accel, 12, 10f);
                if (i > 2)
                    CreateBulletsSector(5, pos, 7.4f, m_CurrentAngle, accel, 13, 10f);
                yield return new WaitForSeconds(0.9f + i*0.1f);
            }
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern2()
    {
        Vector3 pos1, pos2, pos3;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float gap = 0.32f;
        while(true) {
            for (int i = 0; i < 3; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.position);
                pos3 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(0, pos1, 5.3f, m_CurrentAngle, accel);
                CreateBullet(0, pos2, 5.3f, m_CurrentAngle, accel);
                CreateBullet(0, pos3, 5.3f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.09f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret0_0 : EnemyUnit
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

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos = m_FirePosition.position;
        //CreateBulletsSector(0, pos, 7f, m_CurrentAngle + Random.Range(-2f, 2f), accel, 7, 14f);
        yield return null;
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
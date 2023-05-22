using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5bTurret : EnemyUnit
{
    public Transform m_FirePosition;

    private int[] m_FireDelay = { 1800, 1400, 1200 };
    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
        m_Pattern1 = Pattern1();
        m_Pattern2 = Pattern2();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    public void StartPattern1() {
        StartCoroutine(m_Pattern1);
    }

    public void StartPattern2() {
        StartCoroutine(m_Pattern2);
    }

    public void StopPattern1() {
        if (m_Pattern1 != null)
            StopCoroutine(m_Pattern1);
    }

    public void StopPattern2() {
        if (m_Pattern2 != null)
            StopCoroutine(m_Pattern2);
    }
    
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(7.4f, 900);
        Vector3 pos;
        
        while(true) {
            pos = m_FirePosition.position;
            if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 16f);
            }
            else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            else {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) m_SystemManager.GetDifficulty()]);
        }
    }
    
    
    private IEnumerator Pattern2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        
        if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
            yield break;
        }
        else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
            pos = m_FirePosition.position;
            CreateBulletsSector(5, pos, 5.5f, m_CurrentAngle, accel, 11, 8f);
            CreateBulletsSector(5, pos, 6.2f, m_CurrentAngle, accel, 11, 8f);
            CreateBulletsSector(5, pos, 7.3f, m_CurrentAngle, accel, 11, 8f);
        }
        else {
            pos = m_FirePosition.position; // 5.1 ~ 7.8
            CreateBulletsSector(5, pos, 5.0f, m_CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 5.4f, m_CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 6.0f, m_CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 6.8f, m_CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 7.8f, m_CurrentAngle, accel, 15, 7f);
        }
        yield break;
    }
}

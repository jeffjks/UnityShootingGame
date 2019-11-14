using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5bTurret : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    private IEnumerator m_Pattern1;

    void Start()
    {
        GetCoordinates();
        m_Pattern1 = Pattern1();
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

    public void StartPattern1() {
        StartCoroutine(m_Pattern1);
    }
    
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(7.4f, 0.9f);
        Vector3 pos;
        
        while(true) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 16f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            else {
                CreateBulletsSector(3, pos, 2.4f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
    
    
    private IEnumerator Pattern2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        
        if (m_SystemManager.m_Difficulty == 0) {
            yield break;
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 3; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(5, pos, 5.5f + i*0.9f, m_CurrentAngle, accel, 11, 7f);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(5, pos, 5.1f + i*0.9f, m_CurrentAngle, accel, 15, 7f);
            }
        }
        yield break;
    }
}

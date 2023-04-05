using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5Turret1 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2400, 2300, 2200 };
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int random_value;
        int[] random_index = {1, -1};
        Vector3 pos;
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            random_value = random_index[Random.Range(0, random_index.Length)];

            if (m_SystemManager.m_Difficulty == 0) {
                pos = m_FirePosition.position;
                CreateBulletsSector(3, pos, 6.4f, m_CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.0f, m_CurrentAngle - random_value*2f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.6f, m_CurrentAngle + random_value*2f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.5f, m_CurrentAngle, accel, 3, 30f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                CreateBulletsSector(3, pos, 5.9f, m_CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 6.5f, m_CurrentAngle - random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.1f, m_CurrentAngle + random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.7f, m_CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle - random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.9f, m_CurrentAngle + random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 9.5f, m_CurrentAngle, accel, 3, 30f);
            }
            else {
                pos = m_FirePosition.position;
                CreateBulletsSector(3, pos, 5.9f, m_CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 6.5f, m_CurrentAngle - random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.1f, m_CurrentAngle + random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.7f, m_CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle - random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.9f, m_CurrentAngle + random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 9.5f, m_CurrentAngle, accel, 3, 30f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

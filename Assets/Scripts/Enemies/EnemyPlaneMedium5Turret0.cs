using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5Turret0 : EnemyUnit
{
    public Transform m_FirePosition;
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
        Vector3 pos;
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            for (int i = 0; i < 3; i++) {
                if (m_SystemManager.GetDifficulty() == 0) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.1f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.1f, m_CurrentAngle + 6f, accel, 4, 30f);
                    break;
                }
                else if (m_SystemManager.GetDifficulty() == 1) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.2f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.2f, m_CurrentAngle + 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 7.5f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 7.5f, m_CurrentAngle + 6f, accel, 4, 30f);
                }
                else {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.8f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.8f, m_CurrentAngle + 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 8.2f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 8.2f, m_CurrentAngle + 6f, accel, 4, 30f);
                }
                yield return new WaitForMillisecondFrames(600);
            }
            yield return new WaitForMillisecondFrames(1800);
        }
    }
}

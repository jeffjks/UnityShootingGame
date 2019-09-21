using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5Turret1 : EnemyUnit
{
    public Transform m_FirePosition;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
        StartCoroutine(Pattern1());
    }

    protected override void FixedUpdate()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.FixedUpdate();
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        yield return new WaitForSeconds(1f);

        while(((EnemyPlaneMedium5) m_ParentEnemy).m_State <= 1) {
            for (int i = 0; i < 3; i++) {
                if (m_SystemManager.m_Difficulty == 0) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.1f, m_CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.1f, m_CurrentAngle + 6f, accel, 4, 30f);
                    break;
                }
                else if (m_SystemManager.m_Difficulty == 1) {
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
                yield return new WaitForSeconds(0.6f);
            }
            yield return new WaitForSeconds(1.8f);
        }
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2Turret0 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;

        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos = m_FirePosition.position;
                CreateBullet(2, pos, 6.4f, m_CurrentAngle, accel);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                CreateBulletsSector(2, pos, 6.5f, m_CurrentAngle, accel, 3, 16f);
            }
            else {
                pos = m_FirePosition.position;
                CreateBulletsSector(2, pos, 6.6f, m_CurrentAngle, accel, 3, 16f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium3Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;

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
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle, random_value;
        pos = m_FirePosition.position;
        target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
        random_value = Random.Range(-2f, 2f);

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBullet(4, pos, 6f, target_angle + random_value, accel);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBullet(4, pos, 7f, target_angle + random_value, accel);
                yield return new WaitForSeconds(0.07f);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(4, pos, 8f, target_angle + random_value, accel, 3, 12f);
                yield return new WaitForSeconds(0.06f);
            }
        }
        yield break;
    }
}

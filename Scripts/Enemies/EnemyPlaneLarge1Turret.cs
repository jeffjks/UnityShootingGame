using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;

    void Start()
    {
        GetCoordinates();
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 24f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }
    
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        
        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 6; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 7.5f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 10; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 8.5f, m_CurrentAngle, accel);
                CreateBulletsSector(4, pos, 8.5f, m_CurrentAngle, accel, 2, 28f);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else {
            for (int i = 0; i < 12; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 8.5f, m_CurrentAngle, accel);
                CreateBulletsSector(4, pos, 8.5f, m_CurrentAngle, accel, 2, 28f);
                yield return new WaitForSeconds(0.08f);
            }
        }
        yield return null;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        Destroy(gameObject);
        yield break;
    }
}

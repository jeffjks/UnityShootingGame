using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3Turret : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
        StartCoroutine(Pattern1());
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
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 0.8f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        yield return new WaitForSeconds(2.3f);

        while(true) {
            for (int i = 0; i < 3; i++) {
                if (m_SystemManager.m_Difficulty == 0) {
                    pos = m_FirePosition.position;
                    CreateBullet(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 0.6f,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 100f, 2, 0.6f,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2, 3, 16f);
                }
                else {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 100f, 2, 0.6f,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2, 3, 16f);
                }
                yield return new WaitForSeconds(0.28f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret1 : EnemyUnit
{
    public float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void FixedUpdate()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.FixedUpdate();
    }

    private IEnumerator Pattern1()
    {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float gap = 0.25f;
        yield return new WaitForSeconds(2.5f);

        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(1, pos1, 5.5f, m_CurrentAngle, accel);
                CreateBullet(1, pos2, 5.5f, m_CurrentAngle, accel);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++) {
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.9f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.9f, m_CurrentAngle, accel);
                    yield return new WaitForSeconds(0.06f);
                }
            }
            else {
                for (int i = 0; i < 6; i++) {
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForSeconds(0.05f);
                }
            }
            
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

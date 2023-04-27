using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret1 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1500, 1250 };
    
    private IEnumerator m_CurrentPattern;
    private int m_KillScore;

    void Start()
    {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
        m_KillScore = m_Score;
        m_Score = 0;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    private IEnumerator Pattern1()
    {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float gap = 0.25f;
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (m_SystemManager.GetDifficulty() == 0) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(1, pos1, 5.5f, m_CurrentAngle, accel);
                CreateBullet(1, pos2, 5.5f, m_CurrentAngle, accel);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 4; i++) {
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.9f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.9f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else {
                for (int i = 0; i < 6; i++) {
                    pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
    }

    protected override void KilledByPlayer() {
        m_Score = m_KillScore;
    }
}

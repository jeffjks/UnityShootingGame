using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret1Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 1600, 800, 400 };

    private bool m_Shooting = false;
    private bool m_Active = false; // 총알 생성 없이 총알 쏘는 모션 등 방지용

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive) {
            if (!m_Shooting)
                RotateSlightly(m_PlayerPosition, 90f);
        }
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                StartCoroutine(Pattern1());
                m_Active = true;
            }
        }
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        float gap = 0.18f;
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = { 5.5f, 6.5f, 6.5f };
        int[] delay = { 136, 115, 115 };
        yield return new WaitForMillisecondFrames(Random.Range(0, 1000));
        while(true) {
            m_Shooting = true;
            for (int i = 0; i < 4; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(5, pos1, speed[m_SystemManager.m_Difficulty], m_CurrentAngle, accel);
                CreateBullet(5, pos2, speed[m_SystemManager.m_Difficulty], m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(delay[m_SystemManager.m_Difficulty]);
            }
            m_Shooting = false;
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

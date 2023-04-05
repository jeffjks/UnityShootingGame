using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret2Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 1250, 500, 250 };
    
    private bool m_Active = false; // 총알 생성 없이 총알 쏘는 모션 등 방지용

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 90f);
        else
            RotateSlightly(m_PlayerPosition, 100f);

        if (2 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                StartCoroutine(Pattern1());
                m_Active = true;
            }
        }
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = {5.7f, 6.8f, 6.8f};
        yield return new WaitForMillisecondFrames(Random.Range(0, m_FireDelay[m_SystemManager.m_Difficulty]));
        while(true) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(2, pos, speed[m_SystemManager.m_Difficulty], m_CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

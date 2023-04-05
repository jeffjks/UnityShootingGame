using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall3Turret : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[3];
    private int[] m_FireDelay = { 3000, 2000, 1500 };

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1(Random.Range(0, m_FireDelay[m_SystemManager.m_Difficulty])));
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

    private IEnumerator Pattern1(int millisecond) {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(5.2f, 1400);
        yield return new WaitForMillisecondFrames(millisecond);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBullet(1, pos[0], 7.7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
                CreateBullet(1, pos[1], 7.7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
                CreateBullet(1, pos[2], 7.7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
            }
            else {
                for (int i = 0; i < 4; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    pos[2] = GetScreenPosition(m_FirePosition[2].position);
                    CreateBullet(1, pos[0], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    CreateBullet(1, pos[1], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    CreateBullet(1, pos[2], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    yield return new WaitForMillisecondFrames(100);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

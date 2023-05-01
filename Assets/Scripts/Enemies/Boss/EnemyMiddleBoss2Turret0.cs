using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret0 : EnemyUnit
{
    public EnemyMiddleBoss2Barrel m_EnemyMiddleBoss2Barrel;
    private int[] m_FireDelay = { 3000, 2400, 1800 };
    public Transform[] m_FirePosition = new Transform[2];
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 45f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (m_SystemManager.GetDifficulty() == 0) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.5f, m_CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.5f, m_CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(240);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 3; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4.4f + i*1.1f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4.4f + i*1.1f, m_CurrentAngle - random_value, accel);
                }
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.3f, m_CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.3f, m_CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(170);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, m_CurrentAngle - random_value, accel);
                }
            }
            else {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.8f - i*0.3f, m_CurrentAngle - 66 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6f - i*0.3f, m_CurrentAngle - 62 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.8f - i*0.3f, m_CurrentAngle + 66 - i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6f - i*0.3f, m_CurrentAngle + 62 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(170);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-1f, 1f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, m_CurrentAngle - random_value, accel);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
    }
}

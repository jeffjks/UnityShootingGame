using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret0 : EnemyUnit
{
    public EnemyMiddleBoss2Barrel m_EnemyMiddleBoss2Barrel;
    private int[] m_FireDelay = { 3000, 2400, 1800 };
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateUnit(AngleToPlayer);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(PlayerManager.GetPlayerPosition(), 45f);
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    private IEnumerator Pattern1()
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.5f, CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.5f, CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(240);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 3; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 4.4f + i*1.1f, CurrentAngle + random_value, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 4.4f + i*1.1f, CurrentAngle - random_value, accel);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.3f, CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.3f, CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(170);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, CurrentAngle + random_value, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, CurrentAngle - random_value, accel);
                }
            }
            else {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 6.8f - i*0.3f, CurrentAngle - 66 + i*8f, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 6f - i*0.3f, CurrentAngle - 62 + i*8f, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 6.8f - i*0.3f, CurrentAngle + 66 - i*8f, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 6f - i*0.3f, CurrentAngle + 62 - i*8f, accel);
                    yield return new WaitForMillisecondFrames(170);
                }
                StartCoroutine(m_EnemyMiddleBoss2Barrel.ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-1f, 1f);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, CurrentAngle + random_value, accel);
                    CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, CurrentAngle - random_value, accel);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

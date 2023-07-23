using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMedium1Turret : EnemyUnit
{
    void Start()
    {
        StartCoroutine(Pattern1());
        RotateUnit(AngleToPlayer);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(PlayerManager.GetPlayerPosition(), 32f);
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    private IEnumerator Pattern1() {
        Vector3 pos0, pos1, pos2;
        float gap = 0.07f;
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(Random.Range(0, 1500));
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.right * gap));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.left * gap));
            
                CreateBulletsSector(5, pos1, 7f, CurrentAngle, accel, 4, 20f);
                yield return new WaitForMillisecondFrames(2000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.right * gap));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.left * gap));
            
                CreateBulletsSector(5, pos1, 6f, CurrentAngle, accel, 3, 20f);
                CreateBulletsSector(5, pos1, 7.1f, CurrentAngle, accel, 4, 20f);
                yield return new WaitForMillisecondFrames(800);
            }
            else {
                pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.right * gap));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.left * gap));
                
                for (int i = -1; i < 2; i++) {
                    CreateBulletsSector(5, pos0, 6.1f, CurrentAngle + 3f*i, accel, 3, 20f);
                    CreateBulletsSector(5, pos1, 7.2f, CurrentAngle + 3f*i, accel, 4, 20f);
                }
                yield return new WaitForMillisecondFrames(800);
            }

            if (SystemManager.Difficulty != 0) {
                for (int i = 0; i < 6; i++) {
                    pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.right * gap));
                    pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i, CurrentAngle + Random.Range(-1f, 0f), accel);
                    CreateBullet(1, pos2, 5f + i, CurrentAngle + Random.Range(0f, 1f), accel);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(1000);
            }
        }
    }
}

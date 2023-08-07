using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3Turret : EnemyUnit
{
    private int[] m_FireDelay = { 2000, 1500, 1200 };

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 3, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 4, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 4, 19.5f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 15.5f);
            }
            else {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(0, pos, 7f, CurrentAngle, accel, 5, 15.5f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1500, 1200 };

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(PlayerManager.GetPlayerPosition());
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 19.5f);
                yield return new WaitForMillisecondFrames(300);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 19.5f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
            }
            else {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForMillisecondFrames(200);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

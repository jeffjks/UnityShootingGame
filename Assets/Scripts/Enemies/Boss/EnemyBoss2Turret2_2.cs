using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret2_2 : EnemyUnit
{
    private int[] m_FireDelay = { 1800, 900, 600 };
    public Transform m_FirePosition;

    public void StartPattern() {
        StartCoroutine(Pattern1());
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos1, pos2, pos3;
        float gap = 0.32f;

        while(true) {
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos3 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            CreateBullet(5, pos1, 7.3f, CurrentAngle, accel);
            CreateBullet(5, pos2, 7.6f, CurrentAngle, accel);
            CreateBullet(5, pos3, 7.3f, CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
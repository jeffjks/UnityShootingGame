using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret2_1 : EnemyUnit
{
    private int[] m_FireDelay = { 2100, 1500, 900 };
    public Transform m_FirePosition;
    
    private bool m_Activate = false;
    private bool m_Shooting = false;

    protected override void Update()
    {
        base.Update();
        
        if (m_Activate) {
            if (PlayerManager.IsPlayerAlive) {
                if (m_Shooting)
                    RotateSlightly(m_PlayerPosition, 60f);
                }
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
    }

    public void StartPattern() {
        StartCoroutine(Pattern1());
        m_Activate = true;
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos1, pos2;
        float gap = 0.07f;

        while(true) {
            m_Shooting = true;
            for (int i = 0; i < 3; i++) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(5, pos1, 6.1f, m_CurrentAngle, accel);
                CreateBullet(5, pos2, 6.1f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(70);
            }
            m_Shooting = false;
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
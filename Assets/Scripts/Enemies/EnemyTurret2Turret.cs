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
        RotateImmediately(PlayerManager.GetPlayerPosition());
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(PlayerManager.GetPlayerPosition(), 90f);
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                StartCoroutine(Pattern1());
                m_Active = true;
            }
        }
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = {5.7f, 6.8f, 6.8f};
        yield return new WaitForMillisecondFrames(Random.Range(0, m_FireDelay[(int) SystemManager.Difficulty]));
        while(true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(2, pos, speed[(int) SystemManager.Difficulty], CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

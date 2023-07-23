using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall2Turret : EnemyUnit
{
    private int[] m_FireDelay = { 2400, 1200, 600 };

    void Start()
    {
        StartCoroutine(Pattern1());
        RotateUnit(AngleToPlayer);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(PlayerManager.GetPlayerPosition(), 60f);
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        float[] speed = {6.2f, 7.3f, 7.3f};
        
        while (true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
            float target_angle = Mathf.Floor((CurrentAngle + 5f)/10f) * 10f;
        
            CreateBullet(2, pos, speed[(int) SystemManager.Difficulty]*0.9f, target_angle, accel);
            CreateBullet(2, pos, speed[(int) SystemManager.Difficulty], target_angle, accel);
            CreateBullet(2, pos, speed[(int) SystemManager.Difficulty]*1.1f, target_angle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

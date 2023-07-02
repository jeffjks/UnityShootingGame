using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall1Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2400, 1200, 600 };

    void Start()
    {
        StartCoroutine(Pattern1());
        RotateImmediately(PlayerManager.GetPlayerPosition());
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = {6.6f, 7.8f, 7.8f};

        while (true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            float target_angle = Mathf.Floor((CurrentAngle + 5f)/10f) * 10f;
            
            CreateBullet(2, pos, speed[(int) SystemManager.Difficulty], target_angle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

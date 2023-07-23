using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret2 : EnemyUnit
{
    private int[] m_FireDelay = { 2000, 1000, 500 };

    private int m_Side;

    void Start()
    {
        RotateUnit(AngleToPlayer);

        if (transform.localPosition.x > 0f)
            m_Side = -1;
        else
            m_Side = 1;
        
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
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
            CreateBullet(1, pos, 6.8f, CurrentAngle + m_Side*12f, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret1 : EnemyUnit
{
    private int[] m_FireDelay = { 2000, 2000, 1400 };

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
        BulletAccel accel = new BulletAccel(0f, 0);

        while(true) {
            if (SystemManager.Difficulty <= GameDifficulty.Expert) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(2, pos, 6.2f, CurrentAngle, accel, 3, 20f);
            }
            else {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(2, pos, 6.6f, CurrentAngle, accel, 5, 13f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

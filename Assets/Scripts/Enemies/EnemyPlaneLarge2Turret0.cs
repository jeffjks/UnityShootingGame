using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2Turret0 : EnemyUnit
{
    private int[] m_FireDelay = { 2000, 1500, 1000 };

    void Start()
    {
        StartCoroutine(Pattern1());
    }
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;

        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = m_FirePosition[0].position;
                CreateBullet(2, pos, 6.4f, CurrentAngle, accel);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(2, pos, 6.5f, CurrentAngle, accel, 3, 16f);
            }
            else {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(2, pos, 6.6f, CurrentAngle, accel, 3, 16f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

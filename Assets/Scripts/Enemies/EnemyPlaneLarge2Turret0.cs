using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2Turret0 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1500, 1000 };

    void Start()
    {
        StartCoroutine(Pattern1());
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;

        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = m_FirePosition.position;
                CreateBullet(2, pos, 6.4f, m_CurrentAngle, accel);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition.position;
                CreateBulletsSector(2, pos, 6.5f, m_CurrentAngle, accel, 3, 16f);
            }
            else {
                pos = m_FirePosition.position;
                CreateBulletsSector(2, pos, 6.6f, m_CurrentAngle, accel, 3, 16f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

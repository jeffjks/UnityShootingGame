using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5Turret0 : EnemyUnit
{
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateUnit(AngleToPlayer);
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            for (int i = 0; i < 3; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.1f, CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.1f, CurrentAngle + 6f, accel, 4, 30f);
                    break;
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.2f, CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.2f, CurrentAngle + 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 7.5f, CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 7.5f, CurrentAngle + 6f, accel, 4, 30f);
                }
                else {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.8f, CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 6.8f, CurrentAngle + 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 8.2f, CurrentAngle - 6f, accel, 4, 30f);
                    CreateBulletsSector(0, pos, 8.2f, CurrentAngle + 6f, accel, 4, 30f);
                }
                yield return new WaitForMillisecondFrames(600);
            }
            yield return new WaitForMillisecondFrames(1800);
        }
    }
}

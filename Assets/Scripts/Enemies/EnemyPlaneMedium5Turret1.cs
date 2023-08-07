using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium5Turret1 : EnemyUnit
{
    private int[] m_FireDelay = { 2400, 2300, 2200 };
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
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
        int random_value;
        int[] random_index = {1, -1};
        Vector3 pos;
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            random_value = random_index[Random.Range(0, random_index.Length)];

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(3, pos, 6.4f, CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.0f, CurrentAngle - random_value*2f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.6f, CurrentAngle + random_value*2f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.5f, CurrentAngle, accel, 3, 30f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(3, pos, 5.9f, CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 6.5f, CurrentAngle - random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.1f, CurrentAngle + random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.7f, CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.3f, CurrentAngle - random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.9f, CurrentAngle + random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 9.5f, CurrentAngle, accel, 3, 30f);
            }
            else {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(3, pos, 5.9f, CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 6.5f, CurrentAngle - random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.1f, CurrentAngle + random_value*3f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 7.7f, CurrentAngle, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.3f, CurrentAngle - random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 8.9f, CurrentAngle + random_value*1.5f, accel, 3, 30f);
                CreateBulletsSector(3, pos, 9.5f, CurrentAngle, accel, 3, 30f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2Turret2 : EnemyUnit
{
    private int[] m_FireDelay = { 2500, 1900, 1400 };

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        base.Update();
        
        float target_angle = GetAngleToTarget(transform.root.position, PlayerManager.GetPlayerPosition()); // Special (Parent 기준)
        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(target_angle, 50f);
        else
            RotateSlightly(target_angle, 100f);
    }
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        float target_angle;

        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(transform.root.position, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 13f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(transform.root.position, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(0, pos, 5.4f, target_angle, accel, 2, 12f);
                CreateBulletsSector(0, pos, 6.3f, target_angle, accel, 2, 12f);
            }
            else {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(transform.root.position, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(0, pos, 5.4f, target_angle - 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 5.4f, target_angle + 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 6.3f, target_angle - 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 6.3f, target_angle + 12f, accel, 2, 8f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

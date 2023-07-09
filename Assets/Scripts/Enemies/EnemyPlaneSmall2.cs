using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneSmall2 : EnemyUnit
{
    private int[] m_FireDelay = { 5000, 2100, 1200 };

    private bool m_TargetPlayer = true;
    private float m_Speed = 7.2f;

    void Start()
    {
        StartCoroutine(Pattern1(800));
        RotateImmediately(PlayerManager.GetPlayerPosition());
        float target_angle = GetAngleToTarget(m_Position2D, PlayerManager.GetPlayerPosition());
        m_MoveVector = new MoveVector(m_Speed, target_angle);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_TargetPlayer) {
            float player_distance = Vector2.Distance(transform.position, PlayerManager.GetPlayerPosition());
            m_MoveVector.direction = GetAngleToTarget(m_Position2D, PlayerManager.GetPlayerPosition());

            if (player_distance < 5f) {
                m_TargetPlayer = false;
            }
        }
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(m_MoveVector.direction);
        else
            RotateSlightly(m_MoveVector.direction, 100f);
    }

    private IEnumerator Pattern1(int millisecond) {
        float[] speed = {8.2f, 9.8f, 9.8f};
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            Vector3 pos = m_FirePosition[0].position;
            BulletAccel accel = new BulletAccel(0f, 0);
            CreateBullet(0, pos, speed[(int) SystemManager.Difficulty], CurrentAngle, accel);
            CreateBulletsSector(2, pos, speed[(int) SystemManager.Difficulty], CurrentAngle, accel, 2, 28f);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

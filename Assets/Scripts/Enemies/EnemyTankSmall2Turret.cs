using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall2Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2400, 1200, 600 };

    private bool m_Shooting = false;

    void Start()
    {
        StartCoroutine(Pattern1(Random.Range(0, 500)));
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();

        if (m_PlayerManager.m_PlayerIsAlive) {
            if (!m_Shooting)
                RotateSlightly(m_PlayerPosition, 72f);
        }
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    private IEnumerator Pattern1(int millisecond) {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = {6.6f, 7.8f, 7.8f};
        yield return new WaitForMillisecondFrames(millisecond);
        while(true) {
            float target_angle = Mathf.Floor((m_CurrentAngle + 5f)/10f) * 10f;

            m_Shooting = true;
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, speed[(int) m_SystemManager.GetDifficulty()], target_angle, accel);
            yield return new WaitForMillisecondFrames(130);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, speed[(int) m_SystemManager.GetDifficulty()], target_angle, accel);
            yield return new WaitForMillisecondFrames(130);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, speed[(int) m_SystemManager.GetDifficulty()], target_angle, accel);
            m_Shooting = false;
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) m_SystemManager.GetDifficulty()]);
        }
    }
}

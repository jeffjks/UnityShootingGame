using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall1Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2400, 1200, 600 };

    void Start()
    {
        StartCoroutine(Pattern1(Random.Range(0, 500)));
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 60f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    private IEnumerator Pattern1(int millisecond) {
        Vector2 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] speed = {6.6f, 7.8f, 7.8f};
        yield return new WaitForMillisecondFrames(millisecond);
        
        while (true) {
            pos = GetScreenPosition(m_FirePosition.position);
            float target_angle = Mathf.Floor((m_CurrentAngle + 5f)/10f) * 10f;
        
            CreateBullet(2, pos, speed[m_SystemManager.GetDifficulty()], target_angle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
    }
}

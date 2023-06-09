using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret2 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1000, 500 };

    private int m_Side;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);

        if (transform.localPosition.x > 0f)
            m_Side = -1;
        else
            m_Side = 1;
        
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }
    
    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while(true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(1, pos, 6.8f, m_CurrentAngle + m_Side*12f, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

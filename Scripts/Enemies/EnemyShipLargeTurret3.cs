using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLargeTurret3 : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    private int m_Side = 0;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);

        if (transform.localPosition.x > 0f)
            m_Side = -1;
        else
            m_Side = 1;
        
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while(true) {
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(1, pos, 6.8f, m_CurrentAngle + m_Side*12f, accel);
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

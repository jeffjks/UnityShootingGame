using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall3Turret : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[3];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);
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
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(4.7f, 1.4f);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBullet(1, pos[0], 7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
                CreateBullet(1, pos[1], 7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
                CreateBullet(1, pos[2], 7f, m_CurrentAngle + Random.Range(-1f, 1f), accel);
            }
            else {
                for (int i = 0; i < 4; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    pos[2] = GetScreenPosition(m_FirePosition[2].position);
                    CreateBullet(1, pos[0], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    CreateBullet(1, pos[1], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    CreateBullet(1, pos[2], 10f + Random.Range(-1f, 1f), m_CurrentAngle + Random.Range(-1f, 1f), accel);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

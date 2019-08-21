using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge2Turret : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    private bool m_Shooting = false;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive) {
            if (!m_Shooting)
                RotateSlightly(m_PlayerPosition, 64f);
            }
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        int[] number1 = {1, 1, 1, 1, 1};
        int[] number2 = {3, 3, 5, 5, 5, 3, 3};
        int[] number3 = {5, 5, 7, 7, 7, 5, 5};
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                m_Shooting = true;
                for (int i = 0; i < 5; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    CreateBulletsSector(1, pos[0], 6.8f, m_CurrentAngle + 12f - i*8f, accel, number1[i], 12f);
                    CreateBulletsSector(1, pos[1], 6.8f, m_CurrentAngle - 12f + i*8f, accel, number1[i], 12f);
                    yield return new WaitForSeconds(0.21f);
                }
                m_Shooting = false;
                yield return new WaitForSeconds(2.2f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                m_Shooting = true;
                for (int i = 0; i < 7; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    CreateBulletsSector(1, pos[0], 6.8f, m_CurrentAngle + 16f - i*8f, accel, number2[i], 8f);
                    CreateBulletsSector(1, pos[1], 6.8f, m_CurrentAngle - 16f + i*8f, accel, number2[i], 8f);
                    yield return new WaitForSeconds(0.14f);
                }
                m_Shooting = false;
                yield return new WaitForSeconds(1.8f);
            }
            else {
                m_Shooting = true;
                for (int i = 0; i < 7; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    CreateBulletsSector(1, pos[0], 6.8f, m_CurrentAngle + 6f - i*6f, accel, number3[i], 6f);
                    CreateBulletsSector(1, pos[1], 6.8f, m_CurrentAngle - 6f + i*6f, accel, number3[i], 6f);
                    yield return new WaitForSeconds(0.14f);
                }
                m_Shooting = false;
                yield return new WaitForSeconds(1.8f);
            }
        }
    }
}

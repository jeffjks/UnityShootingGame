using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret2_1 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition;
    
    private bool m_Activate = false;
    private bool m_Shooting = false;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_Activate) {
            if (m_PlayerManager.m_PlayerIsAlive) {
                if (m_Shooting)
                    RotateSlightly(m_PlayerPosition, 60f);
                }
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
        
        base.Update();
    }

    public void StartPattern() {
        StartCoroutine(Pattern1());
        m_Activate = true;
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos1, pos2;
        float gap = 0.07f;

        while(true) {
            m_Shooting = true;
            for (int i = 0; i < 3; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(5, pos1, 6.1f, m_CurrentAngle, accel);
                CreateBullet(5, pos2, 6.1f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.07f);
            }
            m_Shooting = false;
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}
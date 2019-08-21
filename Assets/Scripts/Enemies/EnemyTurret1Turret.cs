using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret1Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    private bool m_Shooting = false;
    private bool m_Active = false;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (!m_Shooting)
            RotateSlightly(m_PlayerPosition, 90f);
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                StartCoroutine(Pattern1());
                m_Active = true;
            }
        }
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        float gap = 0.18f;
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        while(true) {
            m_Shooting = true;
            for (int i = 0; i < 4; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(5, pos1, 5f, m_CurrentAngle, accel);
                CreateBullet(5, pos2, 5f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.15f);
            }
            m_Shooting = false;
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret2_2 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition;

    void Start()
    {
        GetCoordinates();
    }

    public void StartPattern() {
        StartCoroutine(Pattern1());
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos1, pos2, pos3;
        float gap = 0.32f;

        while(true) {
            pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = GetScreenPosition(m_FirePosition.position);
            pos3 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            CreateBullet(5, pos1, 7.3f, m_CurrentAngle, accel);
            CreateBullet(5, pos2, 7.6f, m_CurrentAngle, accel);
            CreateBullet(5, pos3, 7.3f, m_CurrentAngle, accel);
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}
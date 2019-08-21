using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2Turret2 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
        StartCoroutine(Pattern1());
    }

    protected override void Update()
    {
        float target_angle = GetAngleToTarget(m_ParentEnemy.transform.position, m_PlayerManager.m_Player.transform.position); // Special (Parent 기준)
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(target_angle, 50f);
        else
            RotateSlightly(target_angle, 100f);
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle;

        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(m_ParentEnemy.transform.position, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 13f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(m_ParentEnemy.transform.position, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(0, pos, 5.4f, target_angle, accel, 2, 12f);
                CreateBulletsSector(0, pos, 6.3f, target_angle, accel, 2, 12f);
            }
            else {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(m_ParentEnemy.transform.position, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(0, pos, 5.4f, target_angle - 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 5.4f, target_angle + 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 6.3f, target_angle - 12f, accel, 2, 8f);
                CreateBulletsSector(0, pos, 6.3f, target_angle + 12f, accel, 2, 8f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

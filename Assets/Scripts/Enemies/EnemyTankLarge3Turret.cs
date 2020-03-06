using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
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
            if (m_SystemManager.m_Difficulty == 0) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 19.5f);
                yield return new WaitForSeconds(0.3f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 19.5f);
                yield return new WaitForSeconds(0.3f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 19.5f);
                yield return new WaitForSeconds(0.3f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 19.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
            }
            else {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 3, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 4, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 6, 15.5f);
                yield return new WaitForSeconds(0.2f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(0, pos, 7f, m_CurrentAngle, accel, 5, 15.5f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}

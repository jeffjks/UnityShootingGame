using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4SmallTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
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

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else
            return;
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;

        while(true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(3f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(2f);
            }
            else {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    protected override void KilledByPlayer() {
        m_Score = 1000;
    }
}
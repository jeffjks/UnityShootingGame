using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4SmallTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    private int m_KillScore;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
        m_KillScore = m_Score;
        m_Score = 0;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;

        while(true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(3000);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(2000);
            }
            else {
                CreateBullet(2, pos, 4f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(1000);
            }
        }
    }

    protected override void KilledByPlayer() {
        m_Score = m_KillScore;
    }
}
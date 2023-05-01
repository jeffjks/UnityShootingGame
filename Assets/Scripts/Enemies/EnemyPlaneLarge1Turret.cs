using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private IEnumerator m_CurrentPattern;

    public void StartPattern() {
        m_CurrentPattern = PatternA();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null) {
            StopCoroutine(m_CurrentPattern);
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 24f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }
    
    
    private IEnumerator PatternA() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        
        if (m_SystemManager.GetDifficulty() == 0) {
            for (int i = 0; i < 6; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 7.5f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(80);
            }
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            for (int i = 0; i < 10; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 8.5f, m_CurrentAngle, accel);
                CreateBulletsSector(4, pos, 8.5f, m_CurrentAngle, accel, 2, 28f);
                yield return new WaitForMillisecondFrames(80);
            }
        }
        else {
            for (int i = 0; i < 12; i++) {
                pos = m_FirePosition.position;
                CreateBullet(3, pos, 8.5f, m_CurrentAngle, accel);
                CreateBulletsSector(4, pos, 8.5f, m_CurrentAngle, accel, 2, 28f);
                yield return new WaitForMillisecondFrames(80);
            }
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        
        m_EnemyDeath.OnDeath();
        yield break;
    }
}

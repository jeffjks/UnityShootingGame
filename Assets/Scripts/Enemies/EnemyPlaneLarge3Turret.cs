using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge3Turret : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 600, 600, 500 };
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern1() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 800);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        yield return new WaitForMillisecondFrames(2300);

        while(true) {
            for (int i = 0; i < 3; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    pos = m_FirePosition.position;
                    CreateBullet(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 600,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 100f, 2, 600,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2, 3, 16f);
                }
                else {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 8.3f, m_CurrentAngle, accel1, 2, 100f, 2, 600,
                    5, 4.3f, BulletDirection.PLAYER, 0f, accel2, 3, 16f);
                }
                yield return new WaitForMillisecondFrames(280);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}

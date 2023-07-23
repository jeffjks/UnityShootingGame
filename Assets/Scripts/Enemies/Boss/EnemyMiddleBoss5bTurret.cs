using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5bTurret : EnemyUnit
{
    public Transform m_FirePosition;

    private int[] m_FireDelay = { 1800, 1400, 1200 };
    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
        m_Pattern1 = Pattern1();
        m_Pattern2 = Pattern2();
        RotateUnit(AngleToPlayer);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }

    public void StartPattern1() {
        StartCoroutine(m_Pattern1);
    }

    public void StartPattern2() {
        StartCoroutine(m_Pattern2);
    }

    public void StopPattern1() {
        if (m_Pattern1 != null)
            StopCoroutine(m_Pattern1);
    }

    public void StopPattern2() {
        if (m_Pattern2 != null)
            StopCoroutine(m_Pattern2);
    }
    
    
    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(7.4f, 900);
        Vector3 pos;
        
        while(true) {
            pos = m_FirePosition.position;
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(3, pos, 2.4f, CurrentAngle + Random.Range(-3f, 3f), accel, 5, 16f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(3, pos, 2.4f, CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            else {
                CreateBulletsSector(3, pos, 2.4f, CurrentAngle + Random.Range(-3f, 3f), accel, 5, 12f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
    
    
    private IEnumerator Pattern2() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            yield break;
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            pos = m_FirePosition.position;
            CreateBulletsSector(5, pos, 5.5f, CurrentAngle, accel, 11, 8f);
            CreateBulletsSector(5, pos, 6.2f, CurrentAngle, accel, 11, 8f);
            CreateBulletsSector(5, pos, 7.3f, CurrentAngle, accel, 11, 8f);
        }
        else {
            pos = m_FirePosition.position; // 5.1 ~ 7.8
            CreateBulletsSector(5, pos, 5.0f, CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 5.4f, CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 6.0f, CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 6.8f, CurrentAngle, accel, 15, 7f);
            CreateBulletsSector(5, pos, 7.8f, CurrentAngle, accel, 15, 7f);
        }
        yield break;
    }
}

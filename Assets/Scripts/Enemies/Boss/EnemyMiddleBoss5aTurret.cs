using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5aTurret : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];
    
    private IEnumerator m_CurrentPattern;

    [HideInInspector] public byte m_RotatePattern = 10;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        switch (m_RotatePattern) {
            case 10:
                if (m_PlayerManager.m_PlayerIsAlive) {
                    RotateSlightly(m_PlayerPosition, 120f);
                }
                else {
                    RotateSlightly(m_PlayerPosition, 100f);
                }
                break;
            case 21:
                RotateSlightly(m_CurrentAngle + 90f, 240f);
                break;
            case 22:
                RotateSlightly(m_CurrentAngle - 90f, 240f);
                break;
            case 31:
                RotateSlightly(0f, 120f);
                break;
            case 32:
                RotateSlightly(180f, 120f);
                break;
        }
        
        base.Update();
    }

    public void StartPattern(byte num, int rand = 0) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2(rand);
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                CreateBullet(4, m_FirePosition[0].position, 2f, m_CurrentAngle, accel);
                CreateBullet(4, m_FirePosition[1].position, 2f, m_CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(120);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                CreateBulletsSector(4, m_FirePosition[0].position, 2f, m_CurrentAngle, accel, 2, 9f);
                CreateBulletsSector(4, m_FirePosition[1].position, 2f, m_CurrentAngle + 180f, accel, 2, 9f);
                yield return new WaitForMillisecondFrames(50);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(4, m_FirePosition[0].position, 2f, m_CurrentAngle, accel, 2, 6f);
                CreateBulletsSector(4, m_FirePosition[1].position, 2f, m_CurrentAngle + 180f, accel, 2, 6f);
                yield return new WaitForMillisecondFrames(30);
            }
        }
    }

    private IEnumerator Pattern2(int rand)
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int[] delay = {3000, 1500, 1000};
        if (rand == 1) {
            yield return new WaitForMillisecondFrames(delay[m_SystemManager.m_Difficulty] / 2);
        }

        while (true) {
            m_RotatePattern = 0;
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            m_RotatePattern = 10;
            yield return new WaitForMillisecondFrames(delay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                CreateBullet(0, m_FirePosition[0].position, 5f, m_CurrentAngle, accel);
                CreateBullet(0, m_FirePosition[0].position, 6.4f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(100);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                CreateBullet(0, m_FirePosition[0].position, 5f, m_CurrentAngle, accel);
                CreateBullet(0, m_FirePosition[0].position, 6.4f, m_CurrentAngle, accel);
                CreateBullet(2, m_FirePosition[1].position, 6f, m_CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(0, m_FirePosition[0].position, 5f, m_CurrentAngle, accel, 2, 8f);
                CreateBulletsSector(0, m_FirePosition[0].position, 6.4f, m_CurrentAngle, accel, 2, 8f);
                CreateBullet(2, m_FirePosition[1].position, 5f, m_CurrentAngle + 180f, accel);
                CreateBullet(2, m_FirePosition[1].position, 6.4f, m_CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
    }
}

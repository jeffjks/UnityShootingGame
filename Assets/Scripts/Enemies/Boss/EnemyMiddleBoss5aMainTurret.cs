using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5aMainTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
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
                    RotateSlightly(m_PlayerPosition, 150f);
                }
                else {
                    RotateSlightly(m_PlayerPosition, 100f);
                }
                break;
            case 20:
                if (m_PlayerManager.m_PlayerIsAlive) {
                    RotateSlightly(m_PlayerPosition, 160f);
                }
                else {
                    RotateSlightly(m_PlayerPosition, 100f);
                }
                break;
            case 21:
                RotateSlightly(m_PlayerPosition, 100f, -70f);
                break;
            case 22:
                RotateSlightly(m_PlayerPosition, 100f, 70f);
                break;
        }

        if (m_RotatePattern / 10 == 2) {
            float angle = Mathf.DeltaAngle(GetAngleToTarget(m_FirePosition.position, m_PlayerPosition), m_CurrentAngle);
            if (Mathf.Abs(angle) >= 120f) {
                m_RotatePattern = 20;
            }
            else if (angle >= 70f) {
                m_RotatePattern = 21;
            }
            else if (angle <= -70f) {
                m_RotatePattern = 22;
            }
        }
        
        base.Update();
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        else if (num == 4)
            m_CurrentPattern = Pattern4();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, m_FirePosition.position, 5f, m_CurrentAngle + Random.Range(-4f, 4f), accel, 10, 13f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, m_FirePosition.position, 3.5f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 11, 12f);
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(3, m_FirePosition.position, 5f, m_CurrentAngle - 2.25f + i*1.5f, accel, 7, 15f);
                }
            }
            else {
                CreateBulletsSector(3, m_FirePosition.position, 3f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 11, 12f);
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(3, m_FirePosition.position, 4.5f, m_CurrentAngle - 2.25f + i*1.5f, accel, 9, 12f);
                    CreateBulletsSector(3, m_FirePosition.position, 6f, m_CurrentAngle - 2.25f + i*1.5f, accel, 10, 12f);
                }
            }
            yield return new WaitForMillisecondFrames(1000);
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                CreateBullet(3, m_FirePosition.position, 3.8f, m_CurrentAngle, accel);
                CreateBullet(3, m_FirePosition.position, 4.6f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(210);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                CreateBullet(3, m_FirePosition.position, 3.4f, m_CurrentAngle, accel);
                CreateBullet(3, m_FirePosition.position, 4.2f, m_CurrentAngle - 3.7f, accel);
                CreateBullet(3, m_FirePosition.position, 4.2f, m_CurrentAngle + 3.7f, accel);
                CreateBullet(3, m_FirePosition.position, 5f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(150);
            }
        }
        else {
            while(true) {
                CreateBullet(3, m_FirePosition.position, 3.4f, m_CurrentAngle, accel);
                CreateBullet(3, m_FirePosition.position, 4.2f, m_CurrentAngle - 3.7f, accel);
                CreateBullet(3, m_FirePosition.position, 4.2f, m_CurrentAngle + 3.7f, accel);
                CreateBullet(3, m_FirePosition.position, 5f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(100);
            }
        }
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                CreateBullet(3, m_FirePosition.position, 5.7f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                CreateBulletsSector(3, m_FirePosition.position, 5.7f, m_CurrentAngle, accel, 4, 1.2f);
                yield return new WaitForMillisecondFrames(1000);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(3, m_FirePosition.position, 5.7f, m_CurrentAngle, accel, 6, 1.2f);
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }

    private IEnumerator Pattern4()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, m_FirePosition.position, 5f, m_CurrentAngle + Random.Range(-4f, 4f), accel, 10, 14f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, m_FirePosition.position, 5.5f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 15, 9f);
            }
            else {
                CreateBulletsSector(3, m_FirePosition.position, 6f, m_CurrentAngle + Random.Range(-3f, 3f), accel, 15, 9f);
            }
            yield return new WaitForMillisecondFrames(1000);
        }
    }
}

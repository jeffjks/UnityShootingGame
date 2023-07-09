using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss5aTurret : EnemyUnit
{
    private IEnumerator m_CurrentPattern;

    [HideInInspector] public byte m_RotatePattern = 10;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
    }

    protected override void Update()
    {
        base.Update();
        
        switch (m_RotatePattern) {
            case 10:
                if (PlayerManager.IsPlayerAlive) {
                    RotateSlightly(PlayerManager.GetPlayerPosition(), 120f);
                }
                else {
                    RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
                }
                break;
            case 21:
                RotateSlightly(CurrentAngle + 90f, 240f);
                break;
            case 22:
                RotateSlightly(CurrentAngle - 90f, 240f);
                break;
            case 31:
                RotateSlightly(0f, 120f);
                break;
            case 32:
                RotateSlightly(180f, 120f);
                break;
        }
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
        BulletAccel accel = new BulletAccel(0f, 0);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                CreateBullet(4, m_FirePosition[0].position, 2f, CurrentAngle, accel);
                CreateBullet(4, m_FirePosition[1].position, 2f, CurrentAngle + 180f, accel);
                yield return new WaitForFrames(7);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                CreateBulletsSector(4, m_FirePosition[0].position, 2f, CurrentAngle, accel, 2, 9f);
                CreateBulletsSector(4, m_FirePosition[1].position, 2f, CurrentAngle + 180f, accel, 2, 9f);
                yield return new WaitForFrames(3);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(4, m_FirePosition[0].position, 2f, CurrentAngle, accel, 2, 6f);
                CreateBulletsSector(4, m_FirePosition[1].position, 2f, CurrentAngle + 180f, accel, 2, 6f);
                yield return new WaitForFrames(2);
            }
        }
    }

    private IEnumerator Pattern2(int rand)
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        int[] delay = {3000, 1500, 1000};
        if (rand == 1) {
            yield return new WaitForMillisecondFrames(delay[(int) SystemManager.Difficulty] / 2);
        }

        while (true) {
            m_RotatePattern = 0;
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(4, m_FirePosition[0].position, 5.5f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(40);
                }
            }
            m_RotatePattern = 10;
            yield return new WaitForMillisecondFrames(delay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern3()
    {
        BulletAccel accel = new BulletAccel(0f, 0);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                CreateBullet(0, m_FirePosition[0].position, 5f, CurrentAngle, accel);
                CreateBullet(0, m_FirePosition[0].position, 6.4f, CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(100);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                CreateBullet(0, m_FirePosition[0].position, 5f, CurrentAngle, accel);
                CreateBullet(0, m_FirePosition[0].position, 6.4f, CurrentAngle, accel);
                CreateBullet(2, m_FirePosition[1].position, 6f, CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(0, m_FirePosition[0].position, 5f, CurrentAngle, accel, 2, 8f);
                CreateBulletsSector(0, m_FirePosition[0].position, 6.4f, CurrentAngle, accel, 2, 8f);
                CreateBullet(2, m_FirePosition[1].position, 5f, CurrentAngle + 180f, accel);
                CreateBullet(2, m_FirePosition[1].position, 6.4f, CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
    }
}

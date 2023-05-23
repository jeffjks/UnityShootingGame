using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4MainTurret : EnemyUnit
{
    public EnemyBoss4MainTurretBarrel m_EnemyBoss4MainTurretBarrel;
    public Transform m_FirePosition;
    [HideInInspector] public byte m_RotatePattern = 10;
    
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        switch (m_RotatePattern) {
            case 10:
                if (m_PlayerManager.m_PlayerIsAlive)
                    RotateSlightly(m_PlayerPosition, 120f);
                else
                    RotateSlightly(m_PlayerPosition, 100f);
                break;
            case 21:
                RotateSlightly(-45f, 80f);
                break;
            case 22:
                RotateSlightly(45f, 80f);
                break;
        }
    }

    public void StartPattern(byte num) {
        if (num == 1) {
            m_CurrentPattern1 = Pattern1A();
            m_CurrentPattern2 = Pattern1B();
        }
        else if (num == 2) {
            m_CurrentPattern1 = Pattern2();
        }
        else if (num == 3) {
            m_CurrentPattern1 = Pattern3();
        }
        else if (num == 4) {
            m_CurrentPattern1 = Pattern4();
        }
        else
            return;
        if (m_CurrentPattern1 != null)
            StartCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StartCoroutine(m_CurrentPattern2);
    }

    public void StopPattern() {
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        m_CurrentPattern1 = null;
        m_CurrentPattern2 = null;
    }

    private IEnumerator Pattern1A()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.64f, rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            pos0 = GetScreenPosition(m_FirePosition.position);
            pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(0, pos0, 5.8f, m_CurrentAngle + rand, accel);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 30f, accel, 2, 18f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 30f, accel, 2, 18f);
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 12f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 26f, accel, 2, 12f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 26f, accel, 2, 12f);
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 10f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 25f, accel, 3, 10f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 25f, accel, 3, 10f);
                yield return new WaitForMillisecondFrames(450);
            }
        }
    }

    private IEnumerator Pattern1B()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            pos = GetScreenPosition(m_FirePosition.position);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 4, 14f);
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 10f);
                yield return new WaitForMillisecondFrames(850);
            }
            else {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 8f);
                yield return new WaitForMillisecondFrames(600);
            }
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float rand;

        pos = GetScreenPosition(m_FirePosition.position);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 6; i++) {
                rand = Random.Range(-5f, 5f);
                CreateBullet(0, pos, 6.3f, m_CurrentAngle + rand - 60f + i*20f, accel);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 11; i++) {
                rand = Random.Range(-3f, 3f);
                CreateBullet(0, pos, 5.8f, m_CurrentAngle + rand - 60f + i*12f, accel);
                CreateBullet(0, pos, 7.2f, m_CurrentAngle + rand - 60f + i*12f, accel);
            }
        }
        else {
            for (int i = 0; i < 11; i++) {
                rand = Random.Range(-3f, 3f);
                CreateBullet(0, pos, 6f, m_CurrentAngle + rand - 60f + i*12f, accel);
                CreateBullet(0, pos, 7.5f, m_CurrentAngle + rand - 60f + i*12f, accel);
            }
        }
        yield break;
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        
        while (true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2f*1.5f);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2f*1.5f);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2f*1.5f);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2f*1.5f);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2f*1.5f);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2f*1.5f + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2f*1.5f + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2f*1.5f + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2f*1.5f + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2f*1.5f + 5f);
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(5, pos, 6.79f * 1f, m_CurrentAngle, accel, 2, 8.13f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1f, m_CurrentAngle, accel, 2, 17.63f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1f, m_CurrentAngle, accel, 2, 28.53f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1f, m_CurrentAngle, accel, 2, 40.95f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1f, m_CurrentAngle, accel, 2, 56.77f*2f*1.2f + 5f);

                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2f*1.2f);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2f*1.2f);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2f*1.2f);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2f*1.2f);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2f*1.2f);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2f*1.2f + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2f*1.2f + 5f);
                yield return new WaitForMillisecondFrames(1200);
            }
            else {
                CreateBulletsSector(5, pos, 6.79f * 1f, m_CurrentAngle, accel, 2, 8.13f*2f + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1f, m_CurrentAngle, accel, 2, 17.63f*2f + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1f, m_CurrentAngle, accel, 2, 28.53f*2f + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1f, m_CurrentAngle, accel, 2, 40.95f*2f + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1f, m_CurrentAngle, accel, 2, 56.77f*2f + 5f);

                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2f);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2f);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2f);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2f);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2f);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2f + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2f + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2f + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2f + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2f + 5f);
                yield return new WaitForMillisecondFrames(900);
            }
        }
    }

    private IEnumerator Pattern4()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        
        pos = GetScreenPosition(m_FirePosition.position);
        StartCoroutine(m_EnemyBoss4MainTurretBarrel.ShootAnimation());
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.ERASE_AND_CREATE, 500,
            1, 4.5f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 30, 12f);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.CREATE, 200,
            1, 5f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 45, 8f, new Vector2Int(170, 170));
        }
        else {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.CREATE, 200,
            1, 5.4f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 50, 7.2f, new Vector2Int(125, 125));
        }
        yield break;
    }
}
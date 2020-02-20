using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss4MainTurret : EnemyUnit
{
    public GameObject m_Barrel;
    public Transform m_FirePosition;
    [HideInInspector] public byte m_RotatePattern = 10;
    
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
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
        
        base.Update();
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos0, pos1, pos2;
        float gap = 0.64f, rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            pos0 = GetScreenPosition(m_FirePosition.position);
            pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 15f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 30f, accel, 2, 15f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 30f, accel, 2, 15f);
                yield return new WaitForSeconds(0.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 12f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 26f, accel, 2, 12f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 26f, accel, 2, 12f);
                yield return new WaitForSeconds(0.6f);
            }
            else {
                CreateBulletsSector(0, pos0, 5.8f, m_CurrentAngle + rand, accel, 3, 10f);
                CreateBulletsSector(0, pos1, 5.8f, m_CurrentAngle + rand - 25f, accel, 3, 10f);
                CreateBulletsSector(0, pos2, 5.8f, m_CurrentAngle + rand + 25f, accel, 3, 10f);
                yield return new WaitForSeconds(0.45f);
            }
        }
    }

    private IEnumerator Pattern1B()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float rand;
        
        while (true) {
            rand = Random.Range(-5f, 5f);
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 4, 12f);
                yield return new WaitForSeconds(1.2f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 10f);
                yield return new WaitForSeconds(0.8f);
            }
            else {
                CreateBulletsSector(3, pos, 4.1f, m_CurrentAngle + rand, accel, 5, 8f);
                yield return new WaitForSeconds(0.6f);
            }
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float rand;

        pos = GetScreenPosition(m_FirePosition.position);
        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 9; i++) {
                rand = Random.Range(-4f, 4f);
                CreateBullet(0, pos, 6.3f, m_CurrentAngle + rand - 60f + i*15f, accel);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        
        while (true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2 + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2 + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2 + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2 + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2 + 5f);
                yield return new WaitForSeconds(1.2f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(5, pos, 6.79f * 1f, m_CurrentAngle, accel, 2, 8.13f*2 + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1f, m_CurrentAngle, accel, 2, 17.63f*2 + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1f, m_CurrentAngle, accel, 2, 28.53f*2 + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1f, m_CurrentAngle, accel, 2, 40.95f*2 + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1f, m_CurrentAngle, accel, 2, 56.77f*2 + 5f);

                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2 + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2 + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2 + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2 + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2 + 5f);
                yield return new WaitForSeconds(1f);
            }
            else {
                CreateBulletsSector(5, pos, 6.79f * 1f, m_CurrentAngle, accel, 2, 8.13f*2 + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1f, m_CurrentAngle, accel, 2, 17.63f*2 + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1f, m_CurrentAngle, accel, 2, 28.53f*2 + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1f, m_CurrentAngle, accel, 2, 40.95f*2 + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1f, m_CurrentAngle, accel, 2, 56.77f*2 + 5f);

                CreateBulletsSector(3, pos, 6.79f * 1.2f, m_CurrentAngle, accel, 2, 8.13f*2);
                CreateBulletsSector(3, pos, 6.16f * 1.2f, m_CurrentAngle, accel, 2, 17.63f*2);
                CreateBulletsSector(3, pos, 5.48f * 1.2f, m_CurrentAngle, accel, 2, 28.53f*2);
                CreateBulletsSector(3, pos, 4.77f * 1.2f, m_CurrentAngle, accel, 2, 40.95f*2);
                CreateBulletsSector(3, pos, 4.16f * 1.2f, m_CurrentAngle, accel, 2, 56.77f*2);

                CreateBulletsSector(5, pos, 6.79f * 1.4f, m_CurrentAngle, accel, 2, 8.13f*2 + 5f);
                CreateBulletsSector(5, pos, 6.16f * 1.4f, m_CurrentAngle, accel, 2, 17.63f*2 + 5f);
                CreateBulletsSector(5, pos, 5.48f * 1.4f, m_CurrentAngle, accel, 2, 28.53f*2 + 5f);
                CreateBulletsSector(5, pos, 4.77f * 1.4f, m_CurrentAngle, accel, 2, 40.95f*2 + 5f);
                CreateBulletsSector(5, pos, 4.16f * 1.4f, m_CurrentAngle, accel, 2, 56.77f*2 + 5f);
                yield return new WaitForSeconds(0.9f);
            }
        }
    }

    private IEnumerator Pattern4()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        
        pos = GetScreenPosition(m_FirePosition.position);
        StartCoroutine(ShootAnimation());
        if (m_SystemManager.m_Difficulty == 0) {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.CREATE, 0.2f,
            1, 4.5f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 30, 12f, new Vector2(0.32f, 0.32f));
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.CREATE, 0.2f,
            1, 4.5f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 45, 8f, new Vector2(0.2f, 0.2f));
        }
        else {
            CreateBullet(0, pos, 5.1f, m_CurrentAngle, accel, BulletType.CREATE, 0.2f,
            1, 4.5f, BulletDirection.FIXED, Random.Range(0f, 360f), accel, 50, 7.2f, new Vector2(0.15f, 0.15f));
        }
        yield break;
    }

    private IEnumerator ShootAnimation() {
        m_Barrel.transform.DOLocalMoveZ(-0.32f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        m_Barrel.transform.DOLocalMoveZ(0f, 0.3f);
        yield break;
    }
}
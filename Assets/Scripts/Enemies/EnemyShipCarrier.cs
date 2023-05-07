using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipCarrier : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[3];
    public EnemyUnit[] m_EnemyUnits;

    private float m_Direction1, m_Direction2;
    private int m_Pattern1Direction = 1;
    
    void Start()
    {
        RotateImmediately(m_MoveVector.direction);
        m_Direction1 = Random.Range(0f, 360f);
        if (m_CurrentAngle >= 180f) {
            m_Pattern1Direction = -1;
        }
        StartCoroutine(Pattern1());
        StartCoroutine(Pattern2());
    }
    
    protected override void Update()
    {
        base.Update();
        
        m_Direction1 += 120f / Application.targetFrameRate * Time.timeScale;
        m_Direction2 += 180f / Application.targetFrameRate * Time.timeScale;

        if (m_Direction1 > 360f) {
            m_Direction1 -= m_Pattern1Direction*360f;
        }
        if (m_Direction2 > 360f) {
            m_Direction2 -= 360f;
        }

        RotateImmediately(m_MoveVector.direction);
    }

    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        
        if (m_SystemManager.GetDifficulty() == 0) {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 2, 180f);
                yield return new WaitForMillisecondFrames(160);
            }

        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 3, 120f);
                yield return new WaitForMillisecondFrames(110);
            }

        }
        else {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 4, 90f);
                yield return new WaitForMillisecondFrames(70);
            }
        }
    }
    
    private IEnumerator Pattern2() {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        
        if (m_SystemManager.GetDifficulty() == 0) {
            while(true) {
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBullet(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle, accel);
                CreateBullet(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(200);
            }

        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            while(true) {
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBulletsSector(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle, accel, 2, 180f);
                CreateBulletsSector(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle, accel, 2, 180f);
                yield return new WaitForMillisecondFrames(120);
            }

        }
        else {
            while(true) {
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBulletsSector(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle, accel, 2, 30f);
                CreateBullet(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle + 180f, accel);
                CreateBulletsSector(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle, accel, 2, 30f);
                CreateBullet(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle + 180f, accel);
                yield return new WaitForMillisecondFrames(100);
            }
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_SystemManager.EraseBullets(2000);

        for (int i = 0; i < m_EnemyUnits.Length; i++) {
            if (m_EnemyUnits[i] != null)
                m_EnemyUnits[i].m_EnemyDeath.OnDying();
        }
        
        yield break;
    }

    private IEnumerator DeathExplosion1(float explosion_height) {
        int timer = 0, random_timer = 0;
        Vector3 random_pos;
        while (timer < 1500) {
            random_timer = Random.Range(100, 250);
            random_pos = Random.insideUnitCircle * 4f;
            CreateExplosionEffect(0, 0, new Vector3(random_pos.x, explosion_height, random_pos.z) + new Vector3(0f, 0f, 3.8f));
            yield return new WaitForMillisecondFrames(random_timer);
            timer += random_timer;
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float explosion_height) {
        int timer = 0, random_timer = 0;
        Vector3 random_pos;
        while (timer < 1500) {
            random_timer = Random.Range(100, 250);
            random_pos = Random.insideUnitCircle * 4f;
            CreateExplosionEffect(0, 1, new Vector3(random_pos.x, explosion_height, random_pos.z) + new Vector3(0f, 0f, -3.8f));
            yield return new WaitForMillisecondFrames(random_timer);
            timer += random_timer;
        }
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipCarrier : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[3];
    [SerializeField] private EnemyUnit[] m_EnemyUnits = null;

    private float m_Direction1, m_Direction2;
    private int m_Pattern1Direction = 1;
    
    void Start()
    {
        GetCoordinates();
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
        m_Direction1 += 120f * Time.deltaTime;
        m_Direction2 += 180f * Time.deltaTime;

        if (m_Direction1 > 360f) {
            m_Direction1 -= m_Pattern1Direction*360f;
        }
        if (m_Direction2 > 360f) {
            m_Direction2 -= 360f;
        }

        RotateImmediately(m_MoveVector.direction);
        
        base.Update();
    }

    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        
        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 2, 180f);
                yield return new WaitForSeconds(0.16f);
            }

        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 3, 120f);
                yield return new WaitForSeconds(0.11f);
            }

        }
        else {
            while(true) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                CreateBulletsSector(3, pos[0], 5.7f, m_Direction1 + m_CurrentAngle, accel, 4, 90f);
                yield return new WaitForSeconds(0.07f);
            }
        }
    }
    
    private IEnumerator Pattern2() {
        Vector3[] pos = new Vector3[3];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        
        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBullet(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle, accel);
                CreateBullet(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.2f);
            }

        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                pos[2] = GetScreenPosition(m_FirePosition[2].position);
                CreateBulletsSector(3, pos[1], 6.2f, m_Direction2 + m_CurrentAngle, accel, 2, 180f);
                CreateBulletsSector(3, pos[2], 6.2f, -m_Direction2 + m_CurrentAngle, accel, 2, 180f);
                yield return new WaitForSeconds(0.12f);
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
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        float timer = 0f, random_timer = 0f;
        float explosion_height = 3f;
        Vector3 random_pos1, random_pos2;
        m_SystemManager.EraseBullets(2f);

        for (int i = 0; i < m_EnemyUnits.Length; i++) {
            if (m_EnemyUnits[i] != null)
                m_EnemyUnits[i].OnDeath();
        }

        StartCoroutine(DeathExplosion1(2f));
        StartCoroutine(DeathExplosion2(2f));

        while (timer < 1.5f) {
            random_timer = Random.Range(0.1f, 0.25f);
            random_pos1 = Random.insideUnitCircle * 3f;
            random_pos2 = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, -1, new Vector3(random_pos1.x, explosion_height, random_pos1.z), new MoveVector(3f, Random.Range(0f, 360f)));
            ExplosionEffect(2, -1, new Vector3(random_pos2.x, explosion_height, random_pos2.z), new MoveVector(3f, Random.Range(0f, 360f)));
            yield return new WaitForSeconds(random_timer);
            timer += random_timer;
        }
        yield return new WaitForSeconds(0.2f);
        ExplosionEffect(0, -1, new Vector3(2f, 2f, 2f));
        ExplosionEffect(0, -1, new Vector3(-2f, 2f, 2f));
        ExplosionEffect(0, -1, new Vector3(2f, 2f, 4f));
        ExplosionEffect(0, -1, new Vector3(-2f, 2f, 4f));
        ExplosionEffect(1, 2, new Vector3(0f, 2f, 0f));
        ExplosionEffect(0, -1, new Vector3(2f, 2f, -2f));
        ExplosionEffect(0, -1, new Vector3(-2f, 2f, -2f));
        ExplosionEffect(0, -1, new Vector3(2f, 2f, -4f));
        ExplosionEffect(0, -1, new Vector3(-2f, 2f, -4f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float explosion_height) {
        float timer = 0f, random_timer = 0f;
        Vector3 random_pos;
        while (timer < 1.5f) {
            random_timer = Random.Range(0.1f, 0.25f);
            random_pos = Random.insideUnitCircle * 4f;
            ExplosionEffect(0, 0, new Vector3(random_pos.x, explosion_height, random_pos.z) + new Vector3(0f, 0f, 3.8f));
            yield return new WaitForSeconds(random_timer);
            timer += random_timer;
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float explosion_height) {
        float timer = 0f, random_timer = 0f;
        Vector3 random_pos;
        while (timer < 1.5f) {
            random_timer = Random.Range(0.1f, 0.25f);
            random_pos = Random.insideUnitCircle * 4f;
            ExplosionEffect(0, 1, new Vector3(random_pos.x, explosion_height, random_pos.z) + new Vector3(0f, 0f, -3.8f));
            yield return new WaitForSeconds(random_timer);
            timer += random_timer;
        }
        yield break;
    }
}

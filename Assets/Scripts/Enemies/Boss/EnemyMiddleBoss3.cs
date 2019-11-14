using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss3 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[3];
    public float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    
    private float m_Direction1, m_Direction2;
    private Vector2 m_TargetPosition;
    private sbyte m_Phase;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private float m_AppearanceTimer = 3.5f;

    void Start()
    {
        float delay = 2f;
        DisableAttackable(m_AppearanceTimer);
        m_TargetPosition = new Vector2(0f, -4.3f);

        m_Sequence = DOTween.Sequence()
        .AppendInterval(delay)
        .Append(transform.DOMoveY(2.4f, m_AppearanceTimer - delay));

        Invoke("OnAppearanceComplete", m_AppearanceTimer);
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 75f, 105f, -75f, -105f };
        m_MoveVector = new MoveVector(0.6f, random_direction[Random.Range(0, 4)]);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
    }


    protected override void Update()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 0.96f*Time.deltaTime);

        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.4f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
        
        if (m_IsAttackable) {
            if (m_Position2D.x >= m_TargetPosition.x + 1.6f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
            }
            else if (m_Position2D.x <= m_TargetPosition.x - 1.6f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
            }
            else if (m_Position2D.y >= m_TargetPosition.y + 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
            }
            else if (m_Position2D.y <= m_TargetPosition.y - 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
            }
        }

        m_Direction1 += 111f * Time.deltaTime;
        if (m_Direction1 >= 360f)
            m_Direction1 -= 360f;

        m_Direction2 += 79f * Time.deltaTime;
        if (m_Direction2 >= 360f)
            m_Direction2 -= 360f;

        base.Update();
    }

    public void ToNextPhase() {
        if (m_Phase == 2)
            return;
        m_Phase = 2;

        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);

        ExplosionEffect(1, 0, new Vector3(0f, 3f, 2.8f));
        ExplosionEffect(0, -1, new Vector3(1.4f, 3f, 0.8f));
        ExplosionEffect(0, -1, new Vector3(-1.4f, 3f, 0.8f));
        ExplosionEffect(1, -1, new Vector3(0f, 3f, -1.2f));

        m_CurrentPattern1 = Pattern2A1();
        m_CurrentPattern2 = Pattern2A2();
        StartCoroutine(m_CurrentPattern1);
        StartCoroutine(m_CurrentPattern2);
    }

    

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(1f);
        while (m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A1();
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(4f);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(2f);
            
            m_CurrentPattern1 = Pattern1B1();
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(3f);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(2f);
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        Vector2 pos;
        float timer = 1.25f;
        float target_angle, random_value1, random_value2;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, timer);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0f);

        while (true) {
            random_value1 = Random.Range(-3f, 3f);
            random_value2 = Random.Range(-70f, 70f);
            pos = GetScreenPosition(m_FirePosition[2].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(3, pos, 10f, target_angle + random_value2, accel1,
                BulletType.ERASE_AND_CREATE, timer, 4, 6f, BulletDirection.PLAYER, random_value1, accel2);
            }
            else if (m_SystemManager.m_Difficulty >= 1) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, pos, 10f, target_angle + random_value2, accel1,
                    BulletType.ERASE_AND_CREATE, timer, 4, 5.6f + i*0.4f, BulletDirection.PLAYER, random_value1, accel2);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern1A2() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[1].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);
            random_value = Random.Range(0f, 360f);

            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(0, pos, 7.2f, random_value + 32f*i, accel, 3, 3f);
                }
                yield return new WaitForSeconds(0.43f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 5; i++) {
                    CreateBulletsSector(0, pos, 7.2f, random_value + 25f*i, accel, 4, 3f);
                }
                yield return new WaitForSeconds(0.27f);
            }
            else {
                for (int i = 0; i < 5; i++) {
                    CreateBulletsSector(0, pos, 6.9f, random_value + 25f*i, accel, 4, 3f);
                    CreateBulletsSector(0, pos, 7.5f, random_value + 25f*i, accel, 4, 3f);
                }
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    private IEnumerator Pattern1B1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0f, 0f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(8.7f, 1.2f);

        for (int i = 0; i < 3; i++) {
            pos = GetScreenPosition(m_FirePosition[2].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 9, 15f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel1, 9, 15f);
                yield return new WaitForSeconds(1.2f);
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 9, 15f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel1, 9, 15f);
                break;
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 11, 12f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel1, 11, 12f);
                yield return new WaitForSeconds(0.8f);
            }
            else {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 13, 10f);
                CreateBulletsSector(0, pos, 5.2f, target_angle, accel2, 14, 10f);
                yield return new WaitForSeconds(0.8f);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1B2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[1].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(3, pos, 5.3f, target_angle + Random.Range(-40f, 40f), accel);
                yield return new WaitForSeconds(0.07f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBullet(3, pos, Random.Range(5f, 5.8f), target_angle + Random.Range(-40f, 40f), accel);
                yield return new WaitForSeconds(0.04f);
            }
            else {
                CreateBullet(3, pos, Random.Range(5f, 6f), target_angle + Random.Range(-40f, 40f), accel);
                yield return new WaitForSeconds(0.02f);
            }
        }
    }

    private IEnumerator Pattern2A1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.5f);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 4, 90f);
                yield return new WaitForSeconds(0.12f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 5, 120f);
                yield return new WaitForSeconds(0.08f);
            }
            else {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 6, 60f);
                yield return new WaitForSeconds(0.06f);
            }
        }
    }

    private IEnumerator Pattern2A2() {
        Vector2 pos;
        float target_angle, speed;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.5f);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (m_SystemManager.m_Difficulty == 0) {
                speed = 1f;
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2, accel, 18, 20f);
                yield return new WaitForSeconds(0.33f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                speed = 1.1f;
                CreateBulletsSector(0, pos, 5.4f*speed, -m_Direction2 , accel, 6, 60f);
                CreateBulletsSector(0, pos, 5.9f*speed, -m_Direction2 + 1.5f, accel, 6, 60f);
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2 + 3.5f, accel, 6, 60f);
                CreateBulletsSector(0, pos, 6.8f*speed, -m_Direction2 + 6f, accel, 6, 60f);
                yield return new WaitForSeconds(0.25f);
            }
            else {
                speed = 1.2f;
                CreateBulletsSector(0, pos, 5.4f*speed, -m_Direction2 , accel, 8, 45f);
                CreateBulletsSector(0, pos, 5.9f*speed, -m_Direction2 + 1.5f, accel, 8, 45f);
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2 + 3.5f, accel, 8, 45f);
                CreateBulletsSector(0, pos, 6.8f*speed, -m_Direction2 + 6f, accel, 8, 45f);
                yield return new WaitForSeconds(0.22f);
            }
        }
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector.speed = 0f;
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1.9f));
        StartCoroutine(DeathExplosion2(1.9f));

        yield return new WaitForSeconds(2f);
        ExplosionEffect(3, 3, new Vector3(0f, 3f, 5f)); // 최종 파괴
        ExplosionEffect(2, -1, new Vector3(1.5f, 3f, 2f));
        ExplosionEffect(2, -1, new Vector3(-1.5f, 3f, 2f));
        ExplosionEffect(3, -1, new Vector3(0f, 3f, -1f));
        ExplosionEffect(2, -1, new Vector3(1.5f, 3f, -4f));
        ExplosionEffect(2, -1, new Vector3(-1.5f, 3f, -4f));
        ExplosionEffect(3, -1, new Vector3(0f, 3f, -7f));
        m_SystemManager.ScreenEffect(0);
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector3 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.4f);
            random_pos = new Vector3(Random.Range(-2.5f, 2.5f), 3f, Random.Range(-8f, 7f));
            ExplosionEffect(0, 2, random_pos);
            random_pos = new Vector3(Random.Range(-2.5f, 2.5f), 3f, Random.Range(-8f, 7f));
            ExplosionEffect(0, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector3 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.4f, 0.7f);
            random_pos = new Vector3(Random.Range(-2.5f, 2.5f), 3f, Random.Range(-8f, 7f));
            ExplosionEffect(1, 1, random_pos);
            random_pos = new Vector3(Random.Range(-2.5f, 2.5f), 3f, Random.Range(-8f, 7f));
            ExplosionEffect(1, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}

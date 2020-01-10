using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss4 : EnemyUnit
{
    public EnemyMiddleBoss4Turret1 m_Turret1;
    public EnemyMiddleBoss4Turret2 m_Turret2;
    public EnemyMiddleBoss4Part[] m_Part = new EnemyMiddleBoss4Part[2];
    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.5f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_SubPattern;
    private bool m_Pattern1B;

    void Start()
    {
        float time_limit = 40f;
        DisableAttackable(m_AppearanceTime);
        m_ChildEnemies[0].DisableAttackable(m_AppearanceTime);
        m_ChildEnemies[1].DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition = new Vector3(0f, -5f, Depth.ENEMY);
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, m_AppearanceTime).SetEase(Ease.OutQuad));
        
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        if (!m_TimeLimitState) {
            if (m_IsAttackable) {
                if (transform.position.x >= m_PlayerPosition.x * 0.14f + 1.2f) {
                    m_MoveVector = new MoveVector(new Vector2(-Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
                }
                else if (transform.position.x <= m_PlayerPosition.x * 0.14f - 1.2f) {
                    m_MoveVector = new MoveVector(new Vector2(Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
                }

                if (transform.position.y >= m_TargetPosition.y + 0.5f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                }
                else if (transform.position.y <= m_TargetPosition.y - 0.5f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                }
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 80f, 100f, -80f, -100f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_UpdateTransform = true;
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        m_SubPattern = SubPattern();
        StartCoroutine(m_SubPattern);
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_UpdateTransform = false;
        transform.DOMoveY(12f, 4f).SetEase(Ease.InQuad);
    }

    private void ToNextPhase() {
        m_Phase++;
        m_SystemManager.EraseBullets(1f);
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);

        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);

        ExplosionEffect(2, 2, new Vector2(0f, -1.4f));
        ExplosionEffect(2, -1, new Vector2(0f, 1.6f));
        ExplosionEffect(1, -1, new Vector2(0.6f, 0.9f) + Random.insideUnitCircle * 0.15f, new MoveVector(4.5f, Random.Range(100f, 170f)));
        ExplosionEffect(1, -1, new Vector2(-0.6f, 0.9f) + Random.insideUnitCircle * 0.15f, new MoveVector(4.5f, Random.Range(190f, 260f)));
        ExplosionEffect(1, -1, new Vector2(0.6f, -0.9f) + Random.insideUnitCircle * 0.15f, new MoveVector(4.5f, Random.Range(10f, 80f)));
        ExplosionEffect(1, -1, new Vector2(-0.6f, -0.9f) + Random.insideUnitCircle * 0.15f, new MoveVector(4.5f, Random.Range(280f, 350f)));
    }

    private IEnumerator SubPattern() { // 서브 패턴 ============================
        yield return new WaitForSeconds(1f);
        while (m_Phase == 1) {
            if (m_Part[0] != null) {
                m_Part[0].StartPattern(1);
            }
            else if (m_Part[1] != null) {
                m_Part[1].StartPattern(1);
            }
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(1.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(1.3f);
            }
            else {
                yield return new WaitForSeconds(0.7f);
            }
            if (m_Part[1] != null) {
                m_Part[1].StartPattern(1);
            }
            else if (m_Part[0] != null) {
                m_Part[0].StartPattern(1);
            }
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(1.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(1.3f);
            }
            else {
                yield return new WaitForSeconds(0.7f);
            }
        }
        if (m_Part[0] != null) {
            m_Part[0].StopPattern();
        }
        else if (m_Part[1] != null) {
            m_Part[1].StopPattern();
        }
        yield return new WaitForSeconds(3f);
        if (m_Part[0] != null) {
            m_Part[0].StartPattern(2);
        }
        if (m_Part[1] != null) {
            m_Part[1].StartPattern(2);
        }
        yield break;
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(1f);
        while (m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A1();
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(6f);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(1f);

            m_Pattern1B = true;
            m_CurrentPattern1 = Pattern1B1();
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(6f);
            m_Pattern1B = false;
            yield return new WaitForSeconds(1.8f);
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            pos = GetScreenPosition(m_Turret1.m_FirePosition.position);
            target_angle = m_Turret1.m_CurrentAngle + Random.Range(-1.5f, 1.5f);
            random_value = Random.Range(0.5f,14f);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 5, random_value);
                yield return new WaitForSeconds(1f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 7, random_value);
                yield return new WaitForSeconds(0.6f);
            }
            else {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 7, random_value);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator Pattern1A2() {
        Vector2 pos;
        float target_angle;
        int random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            random_value = Random.Range(0, 2);

            if (random_value == 0) {
                if (m_SystemManager.m_Difficulty == 0) {
                    for (int i = 0; i < 5; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 7, 14f);
                        yield return new WaitForSeconds(0.07f);
                    }
                    yield return new WaitForSeconds(1.2f);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    for (int i = 0; i < 5; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 9, 11f);
                        yield return new WaitForSeconds(0.07f);
                    }
                    yield return new WaitForSeconds(1f);
                }
                else {
                    for (int i = 0; i < 5; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 11, 8f);
                        yield return new WaitForSeconds(0.07f);
                    }
                    yield return new WaitForSeconds(0.8f);
                }
            }
            else {
                if (m_SystemManager.m_Difficulty == 0) {
                    for (int i = 0; i < 3; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 2f, accel, 5, 17f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 5, 17f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 2f, accel, 5, 17f);
                        yield return new WaitForSeconds(0.08f);
                    }
                    yield return new WaitForSeconds(1.3f);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    for (int i = 0; i < 3; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 1.5f, accel, 7, 13f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 7, 15f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 1.5f, accel, 7, 13f);
                        yield return new WaitForSeconds(0.08f);
                    }
                    yield return new WaitForSeconds(1.1f);
                }
                else {
                    for (int i = 0; i < 3; i++) {
                        pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 1.5f, accel, 7, 13f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 7, 15f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 1.5f, accel, 7, 13f);
                        yield return new WaitForSeconds(0.08f);
                    }
                    yield return new WaitForSeconds(0.8f);
                }
            }
        }
    }

    private IEnumerator Pattern1B1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (m_Pattern1B) {
            pos = GetScreenPosition(m_Turret1.m_FirePosition.position);

            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 6; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBullet(4, pos, 3.5f + i*1.2f, target_angle, accel);
                    yield return new WaitForSeconds(0.07f);
                }
                yield return new WaitForSeconds(2.3f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 8; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBullet(4, pos, 3.5f + i*1.2f, target_angle, accel);
                    yield return new WaitForSeconds(0.07f);
                }
                yield return new WaitForSeconds(1.8f);
            }
            else {
                for (int i = 0; i < 10; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBulletsSector(4, pos, 3.5f + i*1.2f, target_angle, accel, 3, 3f);
                    yield return new WaitForSeconds(0.07f);
                }
                yield return new WaitForSeconds(1.3f);
            }
        }
    }

    private IEnumerator Pattern1B2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (m_Pattern1B) {
            pos = GetScreenPosition(m_Turret2.m_FirePosition.position);
            target_angle = m_Turret2.m_CurrentAngle;

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 80f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(0, pos, 6.6f, target_angle, accel, 2, 62f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 44f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(2, pos, 5.2f, target_angle, accel, 2, 26f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(2, pos, 4.5f, target_angle, accel, 2, 8f);
                yield return new WaitForSeconds(1f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 80f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(0, pos, 6.6f, target_angle, accel, 2, 62f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 44f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(2, pos, 5.2f, target_angle, accel, 2, 26f);
                yield return new WaitForSeconds(0.02f);
                CreateBulletsSector(2, pos, 4.5f, target_angle, accel, 2, 8f);
                yield return new WaitForSeconds(0.4f);
            }
            else {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 90f);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(0, pos, 6.8f, target_angle, accel, 2, 78f);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(0, pos, 6.3f, target_angle, accel, 2, 66f);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(0, pos, 5.8f, target_angle, accel, 2, 54f);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(0, pos, 5.3f, target_angle, accel, 2, 42);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(2, pos, 4.8f, target_angle, accel, 2, 30);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(2, pos, 4.3f, target_angle, accel, 2, 18);
                yield return new WaitForSeconds(0.01f);
                CreateBulletsSector(2, pos, 3.8f, target_angle, accel, 2, 6);
                yield return new WaitForSeconds(0.32f);
            }
        }
    }


    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForSeconds(1f);
        while(m_Phase == 2) {
            m_CurrentPattern1 = Pattern2A1();
            m_CurrentPattern2 = Pattern2A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);

            yield return new WaitForSeconds(4.4f);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator Pattern2A1() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            pos = GetScreenPosition(m_Turret1.m_FirePosition.position);
            target_angle = m_Turret1.m_CurrentAngle;
            random_value = Random.Range(-12f, 12f);

            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 5, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1.5f, accel, 4, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value, accel, 4, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1.5f, accel, 4, 24f);
                yield return new WaitForSeconds(0.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 7, 18f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1.5f, accel, 6, 18f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 0.5f, accel, 6, 18f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 0.5f, accel, 6, 18f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1.5f, accel, 6, 18f);
                yield return new WaitForSeconds(0.35f);
            }
            else {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 7, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 0.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 0.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1.5f, accel, 6, 16f);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    private IEnumerator Pattern2A2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            pos = GetScreenPosition(m_Turret1.m_FirePosition.position);
            target_angle = m_Turret1.m_CurrentAngle;

            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle, accel, 8, 45f);
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle + 22.5f, accel, 8, 45f);
                }
                yield return new WaitForSeconds(1.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle, accel, 10, 36f);
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle + 18f, accel, 10, 36f);
                }
                yield return new WaitForSeconds(1f);
            }
            else {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle, accel, 10, 36f);
                    CreateBulletsSector(1, pos, 5.8f + i*0.4f, target_angle + 18f, accel, 10, 36f);
                }
                yield return new WaitForSeconds(0.8f);
            }
        }
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        for (int i = 0; i < m_ChildEnemies.Length; i++) {
            if (m_ChildEnemies[i] != null) {
                m_ChildEnemies[i].OnDeath();
            }
        }

        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(1.4f, 0f);
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1.5f));
        StartCoroutine(DeathExplosion2(1.5f));

        yield return new WaitForSeconds(2f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(3f, 0f));
        ExplosionEffect(0, -1, new Vector2(-3f, 0f));
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        ExplosionEffect(0, -1, new Vector2(0f, -1.5f));
        m_SystemManager.ScreenEffect(0);
        
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.4f);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(0, 0, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.5f, 0.8f);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(1, 1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}

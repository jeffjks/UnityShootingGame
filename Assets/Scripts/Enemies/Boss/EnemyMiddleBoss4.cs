using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss4 : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss4Turret1 m_Turret1;
    public EnemyMiddleBoss4Turret2 m_Turret2;
    public EnemyMiddleBoss4Part[] m_Part = new EnemyMiddleBoss4Part[2];
    private int m_Phase;
    
    private Vector3 m_TargetPosition;
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 40000;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_SubPattern;
    private bool m_Pattern1B;

    void Start()
    {
        m_UpdateTransform = false;
        m_TargetPosition = new Vector3(0f, -5f, Depth.ENEMY);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, APPEARANCE_TIME).SetEase(Ease.OutQuad));*/
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        
        if (!m_TimeLimitState && m_Phase > 0) {
            if (transform.position.x >= m_PlayerPosition.x * 0.14f + 1.2f) {
                m_MoveVector = new MoveVector(new Vector2(-Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
            }
            if (transform.position.x <= m_PlayerPosition.x * 0.14f - 1.2f) {
                m_MoveVector = new MoveVector(new Vector2(Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
            }
            
            if (transform.position.y > m_TargetPosition.y + 0.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                transform.position = new Vector3(transform.position.x, m_TargetPosition.y + 0.5f, transform.position.z);
            }
            if (transform.position.y < m_TargetPosition.y - 0.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                transform.position = new Vector3(transform.position.x, m_TargetPosition.y - 0.5f, transform.position.z);
            }
        }
    }

    public IEnumerator AppearanceSequence() {
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, m_TargetPosition.y, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
        yield break;
    }

    public void OnAppearanceComplete() {
        float[] random_direction = { 80f, 100f, -80f, -100f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_UpdateTransform = true;
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        m_SubPattern = SubPattern();
        StartCoroutine(m_SubPattern);

        EnableInteractableAll();

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        m_UpdateTransform = false;
        m_MoveVector = new MoveVector(0f, 0f);

        int frame = 4000 * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, 12f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private void ToNextPhase() {
        m_Phase++;
        m_SystemManager.EraseBullets(1000);
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
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            if (m_Part[0] != null) {
                m_Part[0].StartPattern(1);
            }
            else if (m_Part[1] != null) {
                m_Part[1].StartPattern(1);
            }
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                yield return new WaitForMillisecondFrames(1300);
            }
            else {
                yield return new WaitForMillisecondFrames(700);
            }
            if (m_Part[1] != null) {
                m_Part[1].StartPattern(1);
            }
            else if (m_Part[0] != null) {
                m_Part[0].StartPattern(1);
            }
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                yield return new WaitForMillisecondFrames(1300);
            }
            else {
                yield return new WaitForMillisecondFrames(700);
            }
        }
        if (m_Part[0] != null) {
            m_Part[0].StopPattern();
        }
        else if (m_Part[1] != null) {
            m_Part[1].StopPattern();
        }
        yield return new WaitForMillisecondFrames(3000);
        if (m_Part[0] != null) {
            m_Part[0].StartPattern(2);
        }
        if (m_Part[1] != null) {
            m_Part[1].StartPattern(2);
        }
        yield break;
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A1();
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(6000);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(1000);

            m_Pattern1B = true;
            m_CurrentPattern1 = Pattern1B1();
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(6000);
            m_Pattern1B = false;
            yield return new WaitForMillisecondFrames(1800);
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            pos = m_Turret1.m_FirePosition.position;
            target_angle = m_Turret1.m_CurrentAngle + Random.Range(-1.5f, 1.5f);
            random_value = Random.Range(0.5f,14f);

            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 5, random_value);
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 7, random_value);
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                CreateBulletsSector(4, pos, 6f, target_angle, accel, 7, random_value);
                yield return new WaitForMillisecondFrames(500);
            }
        }
    }

    private IEnumerator Pattern1A2() {
        Vector2 pos;
        float target_angle;
        int random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            random_value = Random.Range(0, 2);

            if (random_value == 0) {
                if (m_SystemManager.GetDifficulty() == 0) {
                    for (int i = 0; i < 5; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 7, 14f);
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(1200);
                }
                else if (m_SystemManager.GetDifficulty() == 1) {
                    for (int i = 0; i < 5; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 9, 11f);
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(1000);
                }
                else {
                    for (int i = 0; i < 5; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 5.8f + i*0.82f, target_angle, accel, 11, 8f);
                        yield return new WaitForMillisecondFrames(70);
                    }
                    yield return new WaitForMillisecondFrames(800);
                }
            }
            else {
                if (m_SystemManager.GetDifficulty() == 0) {
                    for (int i = 0; i < 3; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 2f, accel, 5, 17f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 5, 17f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 2f, accel, 5, 17f);
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(1300);
                }
                else if (m_SystemManager.GetDifficulty() == 1) {
                    for (int i = 0; i < 3; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 1.5f, accel, 7, 13f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 7, 15f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 1.5f, accel, 7, 13f);
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(1100);
                }
                else {
                    for (int i = 0; i < 3; i++) {
                        pos = m_Turret2.m_FirePosition.position;
                        target_angle = m_Turret2.m_CurrentAngle;
                        CreateBulletsSector(2, pos, 8.4f, target_angle - 1.5f, accel, 7, 13f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle, accel, 7, 15f);
                        CreateBulletsSector(2, pos, 8.4f, target_angle + 1.5f, accel, 7, 13f);
                        yield return new WaitForMillisecondFrames(80);
                    }
                    yield return new WaitForMillisecondFrames(800);
                }
            }
        }
    }

    private IEnumerator Pattern1B1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (m_Pattern1B) {
            pos = m_Turret1.m_FirePosition.position;

            if (m_SystemManager.GetDifficulty() == 0) {
                for (int i = 0; i < 6; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBullet(4, pos, 3.5f + i*1.2f, target_angle, accel);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(2300);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 8; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBullet(4, pos, 3.5f + i*1.2f, target_angle, accel);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(1800);
            }
            else {
                for (int i = 0; i < 10; i++) {
                    target_angle = m_Turret1.m_CurrentAngle;
                    CreateBulletsSector(4, pos, 3.5f + i*1.2f, target_angle, accel, 3, 3f);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(1300);
            }
        }
    }

    private IEnumerator Pattern1B2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (m_Pattern1B) {
            pos = m_Turret2.m_FirePosition.position;
            target_angle = m_Turret2.m_CurrentAngle;

            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 80f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(0, pos, 6.6f, target_angle, accel, 2, 62f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 44f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(2, pos, 5.2f, target_angle, accel, 2, 26f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(2, pos, 4.5f, target_angle, accel, 2, 8f);
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 80f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(0, pos, 6.6f, target_angle, accel, 2, 62f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel, 2, 44f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(2, pos, 5.2f, target_angle, accel, 2, 26f);
                yield return new WaitForFrames(2);
                CreateBulletsSector(2, pos, 4.5f, target_angle, accel, 2, 8f);
                yield return new WaitForMillisecondFrames(400);
            }
            else {
                CreateBulletsSector(0, pos, 7.3f, target_angle, accel, 2, 90f);
                yield return new WaitForFrames(1);
                CreateBulletsSector(0, pos, 6.8f, target_angle, accel, 2, 78f);
                yield return new WaitForFrames(1);
                CreateBulletsSector(0, pos, 6.3f, target_angle, accel, 2, 66f);
                yield return new WaitForFrames(1);
                CreateBulletsSector(0, pos, 5.8f, target_angle, accel, 2, 54f);
                yield return new WaitForFrames(1);
                CreateBulletsSector(0, pos, 5.3f, target_angle, accel, 2, 42);
                yield return new WaitForFrames(1);
                CreateBulletsSector(2, pos, 4.8f, target_angle, accel, 2, 30);
                yield return new WaitForFrames(1);
                CreateBulletsSector(2, pos, 4.3f, target_angle, accel, 2, 18);
                yield return new WaitForFrames(1);
                CreateBulletsSector(2, pos, 3.8f, target_angle, accel, 2, 6);
                yield return new WaitForMillisecondFrames(320);
            }
        }
    }


    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while(m_Phase == 2) {
            m_CurrentPattern1 = Pattern2A1();
            m_CurrentPattern2 = Pattern2A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);

            yield return new WaitForMillisecondFrames(4400);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(600);
        }
    }

    private IEnumerator Pattern2A1() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            pos = m_Turret1.m_FirePosition.position;
            target_angle = m_Turret1.m_CurrentAngle;
            random_value = Random.Range(-12f, 12f);

            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 5, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1f, accel, 4, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value, accel, 4, 24f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1f, accel, 4, 24f);
                yield return new WaitForMillisecondFrames(600);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 6, 19.2f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1.5f, accel, 5, 19.2f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 0.5f, accel, 5, 19.2f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 0.5f, accel, 5, 19.2f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1.5f, accel, 5, 19.2f);
                yield return new WaitForMillisecondFrames(350);
            }
            else {
                CreateBulletsSector(2, pos, 3.6f, target_angle + random_value, accel, 7, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 1.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value - 0.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 0.5f, accel, 6, 16f);
                CreateBulletsSector(5, pos, 3.6f, target_angle + random_value + 1.5f, accel, 6, 16f);
                yield return new WaitForMillisecondFrames(250);
            }
        }
    }

    private IEnumerator Pattern2A2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            pos = m_Turret1.m_FirePosition.position;
            target_angle = m_Turret1.m_CurrentAngle;

            if (m_SystemManager.GetDifficulty() == 0) {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.6f + i*0.4f, target_angle, accel, 6, 60f);
                    CreateBulletsSector(1, pos, 6f + i*0.4f, target_angle + 30f, accel, 6, 60f);
                }
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.6f + i*0.4f, target_angle, accel, 10, 36f);
                    CreateBulletsSector(1, pos, 6f + i*0.4f, target_angle + 18f, accel, 10, 36f);
                }
                yield return new WaitForMillisecondFrames(1000);
            }
            else {
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(1, pos, 5.6f + i*0.4f, target_angle, accel, 10, 36f);
                    CreateBulletsSector(1, pos, 6f + i*0.4f, target_angle + 18f, accel, 10, 36f);
                }
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        for (int i = 0; i < m_Part.Length; i++) {
            if (m_Part[i] != null) {
                m_Part[i].m_EnemyDeath.OnDying();
            }
        }

        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1.4f, 0f);
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1500));
        StartCoroutine(DeathExplosion2(1500));

        yield return new WaitForMillisecondFrames(2000);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(3f, 0f));
        ExplosionEffect(0, -1, new Vector2(-3f, 0f));
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        ExplosionEffect(0, -1, new Vector2(0f, -1.5f));
        
        m_EnemyDeath.OnDeath();
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.MiddleBossClear();
    }

    public void OnBossDeath() {
        m_SystemManager.ScreenEffect(0);
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(200, 400);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(0, 0, random_pos);
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(500, 800);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(1, 1, random_pos);
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }
}

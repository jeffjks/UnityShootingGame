using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss3 : EnemyUnit, IEnemyBossMain
{
    public Transform[] m_FirePosition = new Transform[3];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int[] m_FireDelay = { 1150, 500, 300 };
    private float m_Direction1, m_Direction2;
    private Vector2 m_TargetPosition;
    private int m_Phase;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private const int APPEARANCE_TIME = 3500;

    void Start()
    {
        m_TargetPosition = new Vector2(0f, -4.3f);

        DisableInteractableAll();

        int delay = 2000;
        StartCoroutine(AppearanceSequence(delay));

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;

        /*
        m_Sequence = DOTween.Sequence()
        .AppendInterval(delay)
        .Append(transform.DOMoveY(2.4f, APPEARANCE_TIME - delay));*/
    }

    private IEnumerator AppearanceSequence(int delay) {
        yield return new WaitForMillisecondFrames(delay);

        float init_position_y = transform.position.y;
        int frame = (APPEARANCE_TIME - delay) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, 2.4f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
        yield break;
    }

    public void OnAppearanceComplete() {
        float[] random_direction = { 90f, -90f };
        m_MoveVector = new MoveVector(0.6f, random_direction[Random.Range(0, 2)]);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        EnableInteractableAll();
    }

    protected override void Update()
    {
        base.Update();
        
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 0.96f / Application.targetFrameRate * Time.timeScale); // 배경 카메라 속도에 맞춰서 이동

        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
        
        if (!m_TimeLimitState && m_Phase > 0) {
            if (transform.position.x > m_TargetPosition.x + 1.6f) {
                m_MoveVector.direction = -90f;
            }
            if (transform.position.x < m_TargetPosition.x - 1.6f) {
                m_MoveVector.direction = 90f;
            }
        }

        m_Direction1 += 111f / Application.targetFrameRate * Time.timeScale;
        if (m_Direction1 >= 360f)
            m_Direction1 -= 360f;

        m_Direction2 += 79f / Application.targetFrameRate * Time.timeScale;
        if (m_Direction2 >= 360f)
            m_Direction2 -= 360f;
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

        m_CurrentPattern1 = Pattern2A1();
        m_CurrentPattern2 = Pattern2A2();
        StartCoroutine(m_CurrentPattern1);
        StartCoroutine(m_CurrentPattern2);

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A1();
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(4000);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(2000);
            
            m_CurrentPattern1 = Pattern1B1();
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(3000);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(2000);
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        Vector2 pos;
        int timer = 1250;
        float target_angle, random_value1, random_value2;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, timer);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0);

        while (true) {
            random_value1 = Random.Range(-3f, 3f);
            random_value2 = Random.Range(-70f, 70f);
            pos = GetScreenPosition(m_FirePosition[2].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(3, pos, 10f, target_angle + random_value2, accel1,
                BulletType.ERASE_AND_CREATE, timer, 4, 6f, BulletDirection.PLAYER, random_value1, accel2);
            }
            else if (SystemManager.Difficulty >= GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, pos, 10f, target_angle + random_value2, accel1,
                    BulletType.ERASE_AND_CREATE, timer, 4, 5.6f + i*0.4f, BulletDirection.PLAYER, random_value1, accel2);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern1A2() {
        Vector2 pos;
        float target_angle, random_value;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[1].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);
            random_value = Random.Range(0f, 360f);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(0, pos, 7.2f, random_value + 32f*i, accel, 3, 3f);
                }
                yield return new WaitForMillisecondFrames(430);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 5; i++) {
                    CreateBulletsSector(0, pos, 7.2f, random_value + 25f*i, accel, 4, 3f);
                }
                yield return new WaitForMillisecondFrames(270);
            }
            else {
                for (int i = 0; i < 5; i++) {
                    CreateBulletsSector(0, pos, 6.9f, random_value + 25f*i, accel, 4, 3f);
                    CreateBulletsSector(0, pos, 7.5f, random_value + 25f*i, accel, 4, 3f);
                }
                yield return new WaitForMillisecondFrames(250);
            }
        }
    }

    private IEnumerator Pattern1B1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0f, 0);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(8.7f, 1200);

        for (int i = 0; i < 3; i++) {
            pos = GetScreenPosition(m_FirePosition[2].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 7, 18f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel1, 7, 18f);
                yield return new WaitForMillisecondFrames(1200);
                break;
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 11, 12f);
                CreateBulletsSector(0, pos, 5.9f, target_angle, accel1, 11, 12f);
                yield return new WaitForMillisecondFrames(800);
            }
            else {
                CreateBulletsSector(0, pos, 7.1f, target_angle, accel1, 13, 10f);
                CreateBulletsSector(0, pos, 5.2f, target_angle, accel2, 14, 10f);
                yield return new WaitForMillisecondFrames(800);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1B2() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[1].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(3, pos, 5.3f, target_angle + Random.Range(-45f, 45f), accel);
                yield return new WaitForMillisecondFrames(80);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(3, pos, Random.Range(5f, 5.8f), target_angle + Random.Range(-40f, 40f), accel);
                yield return new WaitForMillisecondFrames(40);
            }
            else {
                CreateBullet(3, pos, Random.Range(5f, 6f), target_angle + Random.Range(-40f, 40f), accel);
                yield return new WaitForMillisecondFrames(20);
            }
        }
    }

    private IEnumerator Pattern2A1() {
        Vector2 pos;
        float target_angle;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1500);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 4, 90f);
                yield return new WaitForMillisecondFrames(120);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 5, 120f);
                yield return new WaitForMillisecondFrames(80);
            }
            else {
                CreateBulletsSector(5, pos, 6.2f, -m_Direction1, accel, 6, 60f);
                yield return new WaitForMillisecondFrames(60);
            }
        }
    }

    private IEnumerator Pattern2A2() {
        Vector2 pos;
        float target_angle, speed;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1500);

        while (true) {
            pos = GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, m_PlayerPosition);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                speed = 1f;
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2, accel, 6, 60f);
                yield return new WaitForMillisecondFrames(330);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                speed = 1.1f;
                CreateBulletsSector(0, pos, 5.4f*speed, -m_Direction2 , accel, 6, 60f);
                CreateBulletsSector(0, pos, 5.9f*speed, -m_Direction2 + 1.5f, accel, 6, 60f);
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2 + 3.5f, accel, 6, 60f);
                CreateBulletsSector(0, pos, 6.8f*speed, -m_Direction2 + 6f, accel, 6, 60f);
                yield return new WaitForMillisecondFrames(250);
            }
            else {
                speed = 1.2f;
                CreateBulletsSector(0, pos, 5.4f*speed, -m_Direction2 , accel, 8, 45f);
                CreateBulletsSector(0, pos, 5.9f*speed, -m_Direction2 + 1.5f, accel, 8, 45f);
                CreateBulletsSector(0, pos, 6.4f*speed, -m_Direction2 + 3.5f, accel, 8, 45f);
                CreateBulletsSector(0, pos, 6.8f*speed, -m_Direction2 + 6f, accel, 8, 45f);
                yield return new WaitForMillisecondFrames(220);
            }
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector.speed = 0f;
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.MiddleBossClear();
    }

    public void OnBossDeath() {
        ScreenEffectService.ScreenWhiteEffect(false);
    }
}

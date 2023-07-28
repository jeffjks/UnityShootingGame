using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss3 : EnemyUnit, IEnemyBossMain
{
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

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
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
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

        SystemManager.OnMiddleBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.z - 0.96f / Application.targetFrameRate * Time.timeScale); // 배경 카메라 속도에 맞춰서 이동

        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
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

    private void ToNextPhase() {
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
            StartPattern("1A1", new BulletPattern_EnemyMiddleBoss3_1A1(this));
            StartPattern("1A2", new BulletPattern_EnemyMiddleBoss3_1A2(this));
            yield return new WaitForMillisecondFrames(4000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2000);
            
            StartPattern("1B1", new BulletPattern_EnemyMiddleBoss3_1B1(this));
            StartPattern("1B2", new BulletPattern_EnemyMiddleBoss3_1B2(this));
            yield return new WaitForMillisecondFrames(3000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    private IEnumerator Pattern2A1() {
        Vector2 pos;
        float target_angle;
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1500);

        while (true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());

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
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1500);

        while (true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
            target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());

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
        BulletManager.BulletsToGems(2000);
        m_MoveVector.speed = 0f;
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnBossDeath() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}

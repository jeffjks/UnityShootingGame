﻿using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss1 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyUnit[] m_Turret = new EnemyUnit[2];
    public Transform m_Rotator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int _phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -5f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2000;
    private const int TIME_LIMIT = 17000;
    private const float ROLLING_ANGLE_MAX = 30f;

    private IEnumerator m_CurrentPhase;
    private IEnumerator _timeLimitCoroutine;

    private void Start()
    {
        // IsColliderInit = false;
        m_Rotator.rotation = Quaternion.Euler(0f, 36f, 20f);
        
        //DisableInteractableAll();
        m_EnemyHealth.SetInvincibility();
        
        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;
        
        // SystemManager.OnMiddleBossStart();
        
        m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;

        MovePattern();
    }

    private void MovePattern() {
        if (TimeLimitState) {
            return;
        }
        if (_phase < 1) {
            return;
        }
        if (transform.position.x > TARGET_POSITION.x + 2f) {
            m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
            transform.position = new Vector3(TARGET_POSITION.x + 2f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < TARGET_POSITION.x - 2f) {
            m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
            transform.position = new Vector3(TARGET_POSITION.x - 2f, transform.position.y, transform.position.z);
        }
        if (transform.position.y > TARGET_POSITION.y + 0.6f) {
            m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
            transform.position = new Vector3(transform.position.x, TARGET_POSITION.y + 0.6f, transform.position.z);
        }
        if (transform.position.y < TARGET_POSITION.y - 0.6f) {
            m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
            transform.position = new Vector3(transform.position.x, TARGET_POSITION.y - 0.6f, transform.position.z);
        }
    }

    private IEnumerator AppearanceSequence() {
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        Vector3 init_vector = transform.position;
        Quaternion init_quaternion = m_Rotator.rotation;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_vector, TARGET_POSITION, t_pos);
            m_Rotator.rotation = Quaternion.Lerp(init_quaternion, Quaternion.identity, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
        
        // IsColliderInit = true;
        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float[] randomDirection = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(0.8f, randomDirection[Random.Range(0, 4)]);
        _phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        
        if (TryGetComponent(out EnemyColorBlender enemyColorBlender)) {
            enemyColorBlender.StartInteractableEffect();
        }
        m_EnemyHealth.DisableInvincibility();
        
        _timeLimitCoroutine = TimeLimit(TIME_LIMIT);
        StartCoroutine(_timeLimitCoroutine);
    }

    private IEnumerator TimeLimit(int timeLimit = 0)
    {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME + timeLimit);
        TimeLimitState = true;

        int frame = 3500 * Application.targetFrameRate / 1000;

        Quaternion initQuaternion = m_Rotator.rotation;
        const float targetHorizontalSpeed = 18f;
        m_MoveVector.direction = (Mathf.DeltaAngle(0f, m_MoveVector.direction) < 180f) ? 90f : -90f;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            float t_rot = (float) (i+1) / frame;

            m_MoveVector.speed = Mathf.Lerp(0f, targetHorizontalSpeed / Application.targetFrameRate, t_pos);
            m_Rotator.rotation = Quaternion.Lerp(initQuaternion, Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f), t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
    }
    
    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForFrames(5);
        m_Turret[0].StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_Turret_1A(m_Turret[0]));
        m_Turret[1].StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_Turret_1A(m_Turret[1]));
        while (_phase == 1)
        {
            yield return StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_1A(this));
        }
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForFrames(5);
        m_Turret[0].StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_Turret_2A(m_Turret[0], -1));
        m_Turret[1].StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_Turret_2A(m_Turret[1], 1));
        while (_phase == 2)
        {
            if (SystemManager.Difficulty == GameDifficulty.Hell)
                yield return StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_2A(this));
            else
                yield return null;
        }
    }

    public void ToNextPhase()
    {
        switch (_phase)
        {
            case 1:
                if (m_EnemyHealth.HealthRatioScaled > 400) // 체력 40% 이하
                    return;
                break;
            default:
                return;
        }
        
        _phase++;
        BulletManager.SetBulletFreeState(1000);
        
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);

        StopAllPatterns();
        m_Turret[0].StopAllPatterns();
        m_Turret[1].StopAllPatterns();
        
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2000);
        if (_timeLimitCoroutine != null)
            StopCoroutine(_timeLimitCoroutine);
        m_MoveVector = new MoveVector(1f, 0f);
        _phase = -1;
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnEndBossDeathAnimation() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}

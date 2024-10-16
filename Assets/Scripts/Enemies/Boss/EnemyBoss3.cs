﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyUnit[] m_Turret = new EnemyUnit[2];
    public EnemyBoss3_Part m_Part;
    public GameObject[] m_PartOnDeath = new GameObject[2];
    public Animator m_BarrelAnimator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    public GameObject m_Engine;

    private int _phase;
    
    private Vector3 _targetPosition;
    private Vector3 _defaultScale;
    private const int APPEARANCE_TIME = 1200;
    private const float MAX_ROTATION = 10f;
    private readonly int _barrelAnimationTrigger = Animator.StringToHash("BarrelShoot");
    private float _directionDelta;

    private IEnumerator _currentPhase;

    private void Start()
    {
        _defaultScale = transform.localScale;
        transform.localScale = new Vector3(2f, 2f, 2f);
        m_MoveVector = new MoveVector(1f, -125f);
        m_CustomDirection = new CustomDirection();
        RotateUnit(m_MoveVector.direction);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnEndBossDeathAnimation;

        m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        m_CustomDirection[0] += _directionDelta / Application.targetFrameRate * Time.timeScale;

        if (_phase > 0) {
            if (transform.position.x > _targetPosition.x + 0.7f) {
                m_MoveVector.direction = Random.Range(-105f, -75f);
            }
            if (transform.position.x < _targetPosition.x - 0.7f) {
                m_MoveVector.direction = Random.Range(75f, 105f);
            }
            if (transform.position.y > _targetPosition.y + 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            if (transform.position.y < _targetPosition.y - 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }
    }

    private IEnumerator AppearanceSequence() {
        float init_speed = m_MoveVector.speed;
        var frame = 2000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 15f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }

        yield return new WaitForMillisecondFrames(1000);

        transform.localScale = _defaultScale;
        transform.position = new Vector3(0f, 4.3f, Depth.ENEMY);
        m_MoveVector = new MoveVector(0f, 0f);
        RotateUnit(m_MoveVector.direction);

        float init_position_y = transform.position.y;
        frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_posy = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            float position_y = Mathf.Lerp(init_position_y, -4.5f, t_posy);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(75f, 105f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        _targetPosition = transform.position;
        _phase = 1;
        _currentPhase = Phase1();
        StartCoroutine(_currentPhase);

        EnableInteractableAll();

        m_Turret[0].SetRotatePattern(new RotatePattern_TargetPlayer(100f));
        m_Turret[1].SetRotatePattern(new RotatePattern_TargetPlayer(100f));

        SystemManager.OnBossInteractable();
    }

    private int RandomValue() {
        int random_value = Random.Range(0, 2);
        if (random_value == 0)
            random_value = -1;
        return random_value;
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
            
        StopAllPatterns();
        if (_currentPhase != null)
            StopCoroutine(_currentPhase);
        m_Turret[0].StopAllPatterns();
        m_Turret[1].StopAllPatterns();
        
        _currentPhase = Phase2();
        StartCoroutine(_currentPhase);
        m_Part.m_EnemyDeath.KillEnemy();
        m_EnemyHealth.SetInvincibility(2000);
        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Phase1() { // 페이즈 1 패턴 =================
        yield return new WaitForMillisecondFrames(1000);
        var repeatedNum = 0;
        while (_phase == 1) {
            yield return Pattern1A();
            yield return new WaitForMillisecondFrames(repeatedNum == 0 ? 1500 : 300);
            
            var side = RandomValue();
            m_Turret[0].StartPattern("1B", new BulletPattern_EnemyBoss3_Turret_1B(m_Turret[0], side));
            m_Turret[1].StartPattern("1B", new BulletPattern_EnemyBoss3_Turret_1B(m_Turret[1], side));
            yield return StartPattern("1B", new BulletPattern_EnemyBoss3_1B(this));
            
            m_Turret[0].StopPattern("1B");
            m_Turret[1].StopPattern("1B");
            yield return new WaitForMillisecondFrames(2000);

            m_CustomDirection[0] = Random.Range(0f, 360f);
            _directionDelta = 71f * RandomValue();
            StartPattern("1C1", new BulletPattern_EnemyBoss3_1C1(this));
            StartPattern("1C2", new BulletPattern_EnemyBoss3_1C2(this));
            yield return new WaitForMillisecondFrames(5000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(700);
            
            m_BarrelAnimator.SetTrigger(_barrelAnimationTrigger);
            yield return StartPattern("1D", new BulletPattern_EnemyBoss3_1D(this));
            yield return new WaitForMillisecondFrames(2500);
            repeatedNum++;
        }
    }

    private IEnumerator Phase2() { // 페이즈 2 패턴 =================
        yield return new WaitForMillisecondFrames(4000);

        while (_phase == 2) {
            yield return StartPattern("2A", new BulletPattern_EnemyBoss3_2A(this));
            yield return new WaitForMillisecondFrames(4000);
            
            var rand = RandomValue();
            _directionDelta =  MAX_ROTATION * rand;
            m_CustomDirection[0] = - MAX_ROTATION * rand;
            
            StartPattern("2B1", new BulletPattern_EnemyBoss3_2B1(this, () => _directionDelta *= -1)); // Pink Wall
            
            for (int i = 0; i < 1; ++i) { // Repeat Once
                var random_value = Random.Range(0, 2);
                
                m_Turret[0].StartPattern("2B", new BulletPattern_EnemyBoss3_Turret_2B(m_Turret[0], random_value));
                m_Turret[1].StartPattern("2B", new BulletPattern_EnemyBoss3_Turret_2B(m_Turret[1], 1 - random_value));

                yield return new WaitForMillisecondFrames(4000);
                m_Turret[0].StopPattern("2B");
                m_Turret[1].StopPattern("2B");

                yield return StartPattern("2B2", new BulletPattern_EnemyBoss3_2B2(this));
                yield return new WaitForMillisecondFrames(1000);
            }
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(200);
        }
    }

    private IEnumerator Pattern1A() {
        var rand = RandomValue();
        _directionDelta = 90f * rand;
        m_CustomDirection[0] = GetAngleToTarget(m_FirePosition[0].position, PlayerManager.GetPlayerPosition()) - 45f * rand;
        
        StartPattern("1A1", new BulletPattern_EnemyBoss3_1A1(this));
        StartPattern("1A2", new BulletPattern_EnemyBoss3_1A2(this));

        const int repeatNum = 3;
        for (int i = 0; i < repeatNum; i++) {
            m_CustomDirection[0] = GetAngleToTarget(m_FirePosition[0].position, PlayerManager.GetPlayerPosition()) - 45f * Mathf.Sign(_directionDelta);

            if (i == repeatNum - 1)
                break;
            yield return new WaitForFrames(64);
            _directionDelta *= -1;
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        _phase = -1;
        StopAllPatterns();
        if (_currentPhase != null)
            StopCoroutine(_currentPhase);
        for (int i = 0; i < m_Turret.Length; i++) {
            if (m_Turret[i] != null)
                m_Turret[i].m_EnemyDeath.KillEnemy();
        }
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1f, 0f);
        m_Engine.SetActive(false);
        
        yield return new WaitForMillisecondFrames(1600);
        m_PartOnDeath[0].SetActive(false);
        yield return new WaitForMillisecondFrames(800);
        m_PartOnDeath[1].SetActive(false);
    }

    public void OnBossKilled() {
        SystemManager.OnBossClear();
    }

    public void OnEndBossDeathAnimation() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(1f);
    }
}

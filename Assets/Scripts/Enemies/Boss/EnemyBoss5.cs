using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss5 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public Transform m_BottomLine;
    public GameObject m_AllWings;
    public GameObject m_WingsForAppearance;
    public EnemyBoss5_Wing[] m_EnemyBoss5Wings = new EnemyBoss5_Wing[3];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    public Animator m_WingOpenAnimator;
    
    private int _pattern1A_fireDelay1;
    private int _pattern1A_fireDelay2;

    private int m_Phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -3.5f);
    private const int APPEARANCE_TIME = 10000;
    private const float DEFAULT_SPEED = 0.2f;
    private int m_MoveDirection;
    private float m_MoveSpeed;
    private float m_TrackPos;
    private readonly MeshRenderer[] _wingMeshRenderers = new MeshRenderer[3];
    private readonly int _openedBoolAnimation = Animator.StringToHash("Opened");

    private IEnumerator m_CurrentPhase;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i] = m_EnemyBoss5Wings[i].GetComponentInChildren<MeshRenderer>();
            _wingMeshRenderers[i].gameObject.SetActive(false);
        }
        m_CustomDirection = new CustomDirection(3);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnEndBossDeathAnimation;
    }

    private IEnumerator AppearanceSequence() {
        var init_position_y = transform.position.y;
        var appearanceTime = (DebugOption.SceneMode > 0) ? 2000 : APPEARANCE_TIME;
        var frame = appearanceTime * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_posy = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            float position_y = Mathf.Lerp(init_position_y, TARGET_POSITION.y, t_posy);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float random_direction = 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.05f, random_direction);
        m_MoveDirection = Random.Range(0, 2)*2 - 1;
        ToNextPhase();
        StartCoroutine(InitMaterial());

        EnableInteractableAll();
        
        SystemManager.OnBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase == -1) {
            m_MoveVector.speed += 0.72f / Application.targetFrameRate * Time.timeScale;
        }
        else if (m_Phase > 0) {
            if (transform.position.x >= TARGET_POSITION.x + 0.5f) {
                m_MoveDirection = -1;
            }
            else if (transform.position.x <= TARGET_POSITION.x - 0.5f) {
                m_MoveDirection = 1;
            }
            else if (transform.position.y >= TARGET_POSITION.y + 0.2f) {
                m_MoveVector.direction = 0f;
            }
            else if (transform.position.y <= TARGET_POSITION.y - 0.2f) {
                m_MoveVector.direction = 180f;
            }

            if (m_MoveSpeed < DEFAULT_SPEED && m_MoveDirection == 1) {
                m_MoveSpeed += 0.13f / Application.targetFrameRate * Time.timeScale;
            }
            else if (m_MoveSpeed > DEFAULT_SPEED && m_MoveDirection == -1) {
                m_MoveSpeed -= 0.13f / Application.targetFrameRate * Time.timeScale;
            }
            else {
                m_MoveSpeed = DEFAULT_SPEED*m_MoveDirection;
            }

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x + m_MoveSpeed / Application.targetFrameRate * Time.timeScale, pos.y, Depth.ENEMY);
        }

        m_CustomDirection[0] -= 111f / Application.targetFrameRate * Time.timeScale;
        m_CustomDirection[1] += 191f / Application.targetFrameRate * Time.timeScale;
        m_CustomDirection[2] += 23f / Application.targetFrameRate * Time.timeScale;
    }

    private void SetWingOpenState(bool state) {
        m_WingOpenAnimator.SetBool(_openedBoolAnimation, state);
    }

    private IEnumerator InitMaterial() {
        yield return new WaitForMillisecondFrames(400);
        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i].material.SetColor("_EmissionColor", Color.white);
        }
        m_WingsForAppearance.SetActive(false);
        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i].gameObject.SetActive(true);
        }
        yield break;
    }

    public void ToNextPhase() {
        m_Phase++;
        StopAllPatterns();

        if (m_Phase == 1) {
            m_Phase = 1;
            m_CurrentPhase = Phase1();
            StartCoroutine(m_CurrentPhase);
        }
        else if (m_Phase == 2) {
            NextPhaseExplosion();
            BulletManager.SetBulletFreeState(2000);
            SetWingOpenState(true);

            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1500);
        
        var difficulty = (int)SystemManager.Difficulty;
        int[,] fireDelay1 = {{ 600, 360, 250 }, { 2400, 2000, 2000 }};
        int[,] fireDelay2 = {{ 2400, 2000, 2000 }, { 600, 400, 250 }};
        var initDelay1 = fireDelay1[0, difficulty]; // start value
        var targetDelay1 = fireDelay1[1, difficulty]; // target value
        var initDelay2 = fireDelay2[0, difficulty]; // start value
        var targetDelay2 = fireDelay2[1, difficulty]; // target value
        
        StartCoroutine(Pattern1_A0(11000, initDelay1, targetDelay1, initDelay2, targetDelay2));
        StartPattern("1A1", new BulletPattern_EnemyBoss5_1A1(this, () => _pattern1A_fireDelay1, targetDelay1));
        StartPattern("1A2", new BulletPattern_EnemyBoss5_1A2(this, () => _pattern1A_fireDelay2, targetDelay2));
        yield return new WaitForMillisecondFrames(15000);

        while (m_Phase == 1)
        {
            StartPattern("1B1", new BulletPattern_EnemyBoss5_1B1(this));
            StartPattern("1B2", new BulletPattern_EnemyBoss5_1B2(this, GetAngleToTarget));
            yield return new WaitForMillisecondFrames(10000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2500);

            StartPattern("1C1", new BulletPattern_EnemyBoss5_1C1(this));
            yield return new WaitForMillisecondFrames(3000);
            StartPattern("1C2", new BulletPattern_EnemyBoss5_1C2(this));
            yield return new WaitForMillisecondFrames(7000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            var initDir1 = GetAngleToTarget(m_FirePosition[1].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1a", new BulletPattern_EnemyBoss5_1D1(this, 1, 1200, initDir1, 2f));
            var initDir2 = GetAngleToTarget(m_FirePosition[2].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1b", new BulletPattern_EnemyBoss5_1D1(this, 2, 800, initDir2, -1f));
            var initDir3 = GetAngleToTarget(m_FirePosition[3].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1c", new BulletPattern_EnemyBoss5_1D1(this, 3, 500, initDir3, -3f));
            yield return new WaitForMillisecondFrames(3000);
            StartPattern("1D2", new BulletPattern_EnemyBoss5_1D2(this));
            yield return new WaitForMillisecondFrames(11000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
        }
    }

    private IEnumerator Pattern1_A0(int duration, float initDelay1, float targetDelay1, float initDelay2, float targetDelay2)
    {
        var frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            var t_delay = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            var fire_delay_1 = Mathf.Lerp(initDelay1, targetDelay1, t_delay);
            var fire_delay_2 = Mathf.Lerp(initDelay2, targetDelay2, t_delay);
            _pattern1A_fireDelay1 = (int) fire_delay_1;
            _pattern1A_fireDelay2 = (int) fire_delay_2;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(3000);
        
        while (m_Phase == 2) {
            StartPattern("2A1a", new BulletPattern_EnemyBoss5_2A1a(this, m_BottomLine));
            StartPattern("2A1b", new BulletPattern_EnemyBoss5_2A1b(this, m_BottomLine));
            //StartPattern("2A2", new BulletPattern_EnemyBoss5_2A2(this));
            yield return new WaitForMillisecondFrames(10000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
            
            StartPattern("2B1", new BulletPattern_EnemyBoss5_2B1(this, m_BottomLine));
            StartPattern("2B2", new BulletPattern_EnemyBoss5_2B2(this));
            yield return new WaitForMillisecondFrames(12000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
            
            StartPattern("2C1", new BulletPattern_EnemyBoss5_2C1(this, m_BottomLine));
            yield return StartPattern("2C2", new BulletPattern_EnemyBoss5_2C2(this));
            StopPattern("2C1");
            StopPattern("2C2");
            yield return new WaitForMillisecondFrames(2500);
        }
    }


    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_EnemyBoss5Wings.Length; i++) {
            m_EnemyBoss5Wings[i].m_EnemyDeath.KillEnemy();
        }
        Destroy(m_AllWings);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);
        
        if (SystemManager.Difficulty < GameDifficulty.Hell) {
            InGameDataManager.Instance.SaveElapsedTime();
        }
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnBossClear();
    }

    public void OnEndBossDeathAnimation() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(1.5f);
        
        if (SystemManager.Difficulty == GameDifficulty.Hell)
        {
            Vector3 bossPos = transform.position;
            bossPos.z = Depth.ENEMY;
            StageManager.Instance.StartFinalBoss(bossPos);
        }
    }
}

using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss1 : EnemyUnit, IEnemyBossMain
{
    public EnemyUnit[] m_Turret = new EnemyUnit[2];
    public Transform m_Rotator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -5f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2000;
    private const int TIME_LIMIT = 17000;
    private const float ROLLING_ANGLE_MAX = 30f;

    private IEnumerator m_CurrentPhase;

    void Start()
    {
        m_Rotator.rotation = Quaternion.Euler(0f, 36f, 20f);

        DisableInteractableAll();
        
        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        MovePattern();
    }

    private void MovePattern() {
        if (TimeLimitState) {
            return;
        }
        if (m_Phase < 1) {
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
        
        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float[] randomDirection = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(0.8f, randomDirection[Random.Range(0, 4)]);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        
        EnableInteractableAll();

        SystemManager.OnMiddleBossStart();
        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int timeLimit = 0) {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME + timeLimit);
        TimeLimitState = true;

        int frame = 3500 * Application.targetFrameRate / 1000;

        Quaternion initQuaternion = m_Rotator.rotation;
        float targetHorizontalSpeed;

        if (Mathf.DeltaAngle(0f, m_MoveVector.direction) < 180f) {
            targetHorizontalSpeed = 18f;
        }
        else {
            targetHorizontalSpeed = -18f;
        }

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            float t_rot = (float) (i+1) / frame;
            
            transform.Translate(new Vector3(Mathf.Lerp(0f, targetHorizontalSpeed / Application.targetFrameRate, t_pos), 0f, 0f));
            m_Rotator.rotation = Quaternion.Lerp(initQuaternion, Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f), t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
    }
    
    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        m_Turret[0].StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_Turret_1A(m_Turret[0]));
        m_Turret[1].StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_Turret_1A(m_Turret[1]));
        while (m_Phase == 1)
        {
            yield return StartPattern("1A", new BulletPattern_EnemyMiddleBoss1_1A(this));
        }
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1800);
        m_Turret[0].StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_Turret_2A(m_Turret[0], -1));
        m_Turret[1].StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_Turret_2A(m_Turret[1], 1));
        while (m_Phase == 2)
        {
            if (SystemManager.Difficulty == GameDifficulty.Hell)
                yield return StartPattern("2A", new BulletPattern_EnemyMiddleBoss1_2A(this));
            else
                yield return null;
        }
    }

    private void ToNextPhase() {
        m_Phase++;
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
        m_MoveVector = new MoveVector(1f, 0f);
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

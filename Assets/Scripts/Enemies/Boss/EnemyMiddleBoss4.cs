using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss4 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyMiddleBoss4_FrontTurret m_FrontTurret;
    public EnemyMiddleBoss4_BackTurret m_BackTurret;
    public EnemyMiddleBoss4_Part[] m_Part = new EnemyMiddleBoss4_Part[2];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -5f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 40000;

    private IEnumerator m_CurrentPhase, m_SubPattern;
    private IEnumerator m_TimeLimit;

    private void Start()
    {
        IsColliderInit = false;
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        
        if (!TimeLimitState && m_Phase > 0) {
            if (transform.position.x >= PlayerManager.GetPlayerPosition().x * 0.14f + 1.2f) {
                m_MoveVector = new MoveVector(new Vector2(-Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
            }
            if (transform.position.x <= PlayerManager.GetPlayerPosition().x * 0.14f - 1.2f) {
                m_MoveVector = new MoveVector(new Vector2(Mathf.Abs(m_MoveVector.GetVector().x), m_MoveVector.GetVector().y));
            }
            
            if (transform.position.y > TARGET_POSITION.y + 0.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y + 0.5f, transform.position.z);
            }
            if (transform.position.y < TARGET_POSITION.y - 0.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y - 0.5f, transform.position.z);
            }
        }
    }

    private IEnumerator AppearanceSequence() {
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, TARGET_POSITION.y, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 80f, 100f, -80f, -100f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_Phase = 1;
        IsColliderInit = true;

        EnableInteractableAll();
        
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        m_SubPattern = SubPattern();
        StartCoroutine(m_SubPattern);

        SystemManager.OnMiddleBossStart();

        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0)
    {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        m_MoveVector = new MoveVector(0f, 0f);

        int frame = 4000 * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, 12f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    public void ToNextPhase() {
        m_Phase++;
        BulletManager.SetBulletFreeState(1000);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_FrontTurret.StopAllPatterns();
        m_BackTurret.StopAllPatterns();

        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator SubPattern() { // 서브 패턴 ============================
        int[] fireDelay = { 1800, 1300, 700 };
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1)
        {
            StopAllPartPattern();
            if (m_Part[0] != null)
                m_Part[0].StartPattern("SubA", new BulletPattern_EnemyMiddleBoss4_PartA(m_Part[0]));
            else if (m_Part[1] != null)
                m_Part[1].StartPattern("SubA", new BulletPattern_EnemyMiddleBoss4_PartA(m_Part[1]));

            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
            
            StopAllPartPattern();
            if (m_Part[1] != null)
                m_Part[1].StartPattern("SubA", new BulletPattern_EnemyMiddleBoss4_PartA(m_Part[1]));
            else if (m_Part[0] != null)
                m_Part[0].StartPattern("SubA", new BulletPattern_EnemyMiddleBoss4_PartA(m_Part[0]));
            
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }

        StopAllPartPattern();
        yield return new WaitForMillisecondFrames(3000);
        
        if (m_Part[0] != null)
            m_Part[0].StartPattern("SubB", new BulletPattern_EnemyMiddleBoss4_PartB(m_Part[0]));
        else if (m_Part[1] != null)
            m_Part[1].StartPattern("SubB", new BulletPattern_EnemyMiddleBoss4_PartB(m_Part[1]));
    }

    private void StopAllPartPattern()
    {
        if (m_Part[0] != null)
            m_Part[0].StopAllPatterns();
        if (m_Part[1] != null)
            m_Part[1].StopAllPatterns();
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_FrontTurret.StartPattern("1A1", new BulletPattern_EnemyMiddleBoss4_MainTurret_1A(m_FrontTurret));
            m_BackTurret.StartPattern("1A2", new BulletPattern_EnemyMiddleBoss4_BackTurret_1A(m_BackTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_FrontTurret.StopPattern("1A1");
            m_BackTurret.StopPattern("1A2");
            yield return new WaitForMillisecondFrames(1000);

            m_FrontTurret.StartPattern("1B1", new BulletPattern_EnemyMiddleBoss4_MainTurret_1B(m_FrontTurret));
            m_BackTurret.StartPattern("1B2", new BulletPattern_EnemyMiddleBoss4_BackTurret_1B(m_BackTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_FrontTurret.StopPattern("1B1");
            m_BackTurret.StopPattern("1B2");
            yield return new WaitForMillisecondFrames(1800);
        }
    }


    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 2) {
            m_FrontTurret.StartPattern("2A1", new BulletPattern_EnemyMiddleBoss4_MainTurret_2A1(m_FrontTurret));
            m_FrontTurret.StartPattern("2A2", new BulletPattern_EnemyMiddleBoss4_MainTurret_2A2(m_FrontTurret));
            yield return new WaitForMillisecondFrames(4400);
            m_FrontTurret.StopAllPatterns();
            yield return new WaitForMillisecondFrames(600);
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        for (int i = 0; i < m_Part.Length; i++) {
            if (m_Part[i] != null) {
                m_Part[i].m_EnemyDeath.KillEnemy();
            }
        }

        BulletManager.BulletsToGems(2000);
        if (m_TimeLimit != null)
            StopCoroutine(m_TimeLimit);
        m_MoveVector = new MoveVector(1.4f, 0f);
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnEndBossDeathAnimation() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}

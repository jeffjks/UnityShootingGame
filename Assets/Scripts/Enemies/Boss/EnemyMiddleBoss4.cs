using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss4 : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss4_Turret1 m_Turret1;
    public EnemyMiddleBoss4_Turret2 m_Turret2;
    public EnemyMiddleBoss4_Part[] m_Part = new EnemyMiddleBoss4_Part[2];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -5f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 40000;

    private IEnumerator m_CurrentPhase, m_SubPattern;

    void Start()
    {
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(TARGET_POSITION.y, APPEARANCE_TIME).SetEase(Ease.OutQuad));*/
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        
        if (!m_TimeLimitState && m_Phase > 0) {
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

    public IEnumerator AppearanceSequence() {
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

    public void OnAppearanceComplete() {
        float[] random_direction = { 80f, 100f, -80f, -100f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        m_SubPattern = SubPattern();
        StartCoroutine(m_SubPattern);

        EnableInteractableAll();

        SystemManager.OnMiddleBossStart();

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        m_MoveVector = new MoveVector(0f, 0f);

        int frame = 4000 * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, 12f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private void ToNextPhase() {
        m_Phase++;
        BulletManager.SetBulletFreeState(1000);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);

        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
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
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
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
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
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
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_Turret1.StartPattern("1A1", new BulletPattern_EnemyMiddleBoss4_Turret1_1A(m_Turret1));
            m_Turret2.StartPattern("1A2", new BulletPattern_EnemyMiddleBoss4_Turret2_1A(m_Turret2));
            yield return new WaitForMillisecondFrames(6000);
            m_Turret1.StopPattern("1A");
            m_Turret2.StopPattern("1A");
            yield return new WaitForMillisecondFrames(1000);

            m_Turret1.StartPattern("1B1", new BulletPattern_EnemyMiddleBoss4_Turret1_1B(m_Turret1));
            m_Turret2.StartPattern("1B2", new BulletPattern_EnemyMiddleBoss4_Turret2_1B(m_Turret2));
            yield return new WaitForMillisecondFrames(6000);
            m_Turret1.StopPattern("1B");
            m_Turret2.StopPattern("1B");
            yield return new WaitForMillisecondFrames(1800);
        }
    }


    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 2) {
            m_Turret1.StartPattern("2A1", new BulletPattern_EnemyMiddleBoss4_Turret1_2A1(m_Turret1));
            m_Turret1.StartPattern("2A2", new BulletPattern_EnemyMiddleBoss4_Turret1_2A2(m_Turret1));
            yield return new WaitForMillisecondFrames(4400);
            m_Turret1.StopAllPatterns();
            yield return new WaitForMillisecondFrames(600);
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        for (int i = 0; i < m_Part.Length; i++) {
            if (m_Part[i] != null) {
                m_Part[i].m_EnemyDeath.OnDying();
            }
        }

        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1.4f, 0f);
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

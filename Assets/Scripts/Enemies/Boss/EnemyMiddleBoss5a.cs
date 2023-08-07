using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss5a : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss5a_MainTurret m_MainTurret;
    public EnemyMiddleBoss5a_SubTurret[] m_SubTurret = new EnemyMiddleBoss5a_SubTurret[2];
    public EnemyMissile[] m_Missiles = new EnemyMissile[8];
    private int m_Phase;
    
    private readonly Vector3 TARGET_POSITION = new (0f, -4f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2500;
    private const int TIME_LIMIT = 38000;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private bool m_Pattern1B;

    void Start()
    {
        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;

        SystemManager.OnMiddleBossStart();
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(TARGET_POSITION.y, APPEARANCE_TIME).SetEase(Ease.OutQuad));*/
    }

    protected override void Update()
    {
        base.Update();
        
        if (!TimeLimitState && m_Phase > 0) {
            if (transform.position.x >= TARGET_POSITION.x + 0.6f) {
                m_MoveVector.direction = Random.Range(-100f, -80f);
            }
            else if (transform.position.x <= TARGET_POSITION.x - 0.6f) {
                m_MoveVector.direction = Random.Range(80f, 100f);
            }
            else if (transform.position.y >= TARGET_POSITION.y + 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            else if (transform.position.y <= TARGET_POSITION.y - 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }
    }

    public IEnumerator AppearanceSequence() {
        float init_position_y = transform.position.y;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, TARGET_POSITION.y, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
        OnAppearanceComplete();
    }

    public void OnAppearanceComplete() {
        float random_direction = Random.Range(80f, 100f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.4f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;

        int frame = 5000 * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, Size.GAME_BOUNDARY_BOTTOM - 8f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1)
        {
            m_MainTurret.StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_MainTurret_A(m_MainTurret));
            yield return new WaitForMillisecondFrames(2000);
            m_SubTurret[0].SetRotatePattern(new RotatePattern_RotateAround(240f));
            m_SubTurret[0].SetRotatePattern(new RotatePattern_RotateAround(-240f));
            m_SubTurret[0].StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_SubTurret_A(m_SubTurret[0]));
            m_SubTurret[1].StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_SubTurret_A(m_SubTurret[1]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StopPattern("A");
            yield return new WaitForMillisecondFrames(2000);
            m_SubTurret[0].StopPattern("A");
            m_SubTurret[1].StopPattern("A");
            m_SubTurret[0].SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
            m_SubTurret[0].SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
            yield return new WaitForMillisecondFrames(2200);
            m_MainTurret.StartPattern("D", new BulletPattern_EnemyMiddleBoss5a_MainTurret_D(m_MainTurret));
            m_MainTurret.PlayerSweepRotatePattern = -1;
            yield return new WaitForMillisecondFrames(500);
            m_MainTurret.StopPattern("D");

            yield return new WaitForMillisecondFrames(500);
            StartCoroutine(LaunchMissile());
            yield return new WaitForMillisecondFrames(1000);
            m_MainTurret.StartPattern("B", new BulletPattern_EnemyMiddleBoss5a_MainTurret_B(m_MainTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern("B");
            m_MainTurret.SetRotatePattern(new RotatePattern_TargetPlayer(150f, 100f));
            yield return new WaitForMillisecondFrames(4000);
            
            m_MainTurret.StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_MainTurret_A(m_MainTurret));
            yield return new WaitForMillisecondFrames(1000);
            var rand = Random.Range(0, 2);
            m_SubTurret[0].StartPattern("B", new BulletPattern_EnemyMiddleBoss5a_SubTurret_B(m_SubTurret[0], rand));
            m_SubTurret[1].StartPattern("B", new BulletPattern_EnemyMiddleBoss5a_SubTurret_B(m_SubTurret[1], 1 - rand));
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern("A");
            m_SubTurret[0].StopPattern("B");
            m_SubTurret[1].StopPattern("B");

            rand = Random.Range(0, 2);
            m_SubTurret[rand].SetRotatePattern(new RotatePattern_TargetAngle(0f, 120f));
            m_SubTurret[1 - rand].SetRotatePattern(new RotatePattern_TargetAngle(180f, 120f));
            yield return new WaitForMillisecondFrames(2000);
            m_SubTurret[0].SetRotatePattern(new RotatePattern_RotateAround(-240f));
            m_SubTurret[0].SetRotatePattern(new RotatePattern_RotateAround(240f));
            m_SubTurret[0].StartPattern("C", new BulletPattern_EnemyMiddleBoss5a_SubTurret_C(m_SubTurret[0]));
            m_SubTurret[1].StartPattern("C", new BulletPattern_EnemyMiddleBoss5a_SubTurret_C(m_SubTurret[1]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("C", new BulletPattern_EnemyMiddleBoss5a_MainTurret_C(m_MainTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_SubTurret[0].StopPattern("C");
            m_SubTurret[1].StopPattern("C");
            m_MainTurret.StopPattern("C");
            break;
        }
        yield break;
    }


    private IEnumerator LaunchMissile() {
        for (int i = 0; i < 4; i++) {
            yield return new WaitForMillisecondFrames(2000);
            if (m_Phase > 0) {
                m_Missiles[i*2].enabled = true;
                m_Missiles[i*2 + 1].enabled = true;
            }
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2500);
        m_MoveVector = new MoveVector(1.5f, 0f);
        
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

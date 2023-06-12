using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss5a : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss5aMainTurret m_MainTurret;
    public EnemyMiddleBoss5aTurret[] m_Turret = new EnemyMiddleBoss5aTurret[2];
    public EnemyMissile[] m_Missiles = new EnemyMissile[8];
    private int m_Phase;
    
    private Vector3 m_TargetPosition;
    private const int APPEARANCE_TIME = 2500;
    private const int TIME_LIMIT = 38000;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private bool m_Pattern1B;

    void Start()
    {
        m_TargetPosition = new Vector3(0f, -4f, Depth.ENEMY);

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
        
        if (!m_TimeLimitState && m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 0.6f) {
                m_MoveVector.direction = Random.Range(-100f, -80f);
            }
            else if (transform.position.x <= m_TargetPosition.x - 0.6f) {
                m_MoveVector.direction = Random.Range(80f, 100f);
            }
            else if (transform.position.y >= m_TargetPosition.y + 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            else if (transform.position.y <= m_TargetPosition.y - 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }
    }

    public IEnumerator AppearanceSequence() {
        float init_position_y = transform.position.y;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

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
        float random_direction = Random.Range(80f, 100f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.4f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;

        int frame = 5000 * Application.targetFrameRate / 1000;
        float init_position_y = transform.position.y;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, Size.GAME_BOUNDARY_BOTTOM - 8f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        int rand;
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_MainTurret.StartPattern(1);
            yield return new WaitForMillisecondFrames(2000);
            m_Turret[0].m_RotatePattern = 21;
            m_Turret[1].m_RotatePattern = 22;
            m_Turret[0].StartPattern(1);
            m_Turret[1].StartPattern(1);
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StopPattern();
            yield return new WaitForMillisecondFrames(2000);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            m_Turret[0].m_RotatePattern = 10;
            m_Turret[1].m_RotatePattern = 10;
            yield return new WaitForMillisecondFrames(2200);
            m_MainTurret.StartPattern(4);
            m_MainTurret.m_RotatePattern = 21;
            yield return new WaitForMillisecondFrames(500);
            m_MainTurret.StopPattern();

            yield return new WaitForMillisecondFrames(500);
            StartCoroutine(LaunchMissile());
            yield return new WaitForMillisecondFrames(1000);
            m_MainTurret.StartPattern(2);
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern();
            m_MainTurret.m_RotatePattern = 10;
            yield return new WaitForMillisecondFrames(4000);
            
            m_MainTurret.StartPattern(1);
            yield return new WaitForMillisecondFrames(1000);
            rand = Random.Range(0, 2);
            m_Turret[0].StartPattern(2, rand);
            m_Turret[1].StartPattern(2, 1-rand);
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern();
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();

            rand = Random.Range(0, 2);
            m_Turret[0].m_RotatePattern = (byte) (31 + rand);
            m_Turret[1].m_RotatePattern = (byte) (32 - rand);
            yield return new WaitForMillisecondFrames(2000);
            m_Turret[0].m_RotatePattern = 22;
            m_Turret[1].m_RotatePattern = 21;
            m_Turret[0].StartPattern(3);
            m_Turret[1].StartPattern(3);
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern(3);
            yield return new WaitForMillisecondFrames(6000);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            m_MainTurret.StopPattern();
            break;
        }
        yield break;
    }


    private IEnumerator LaunchMissile() {
        for (int i = 0; i < 4; i++) {
            yield return new WaitForMillisecondFrames(2000);
            if (m_Phase > 0) {
                try {
                    m_Missiles[i*2].enabled = true;
                    m_Missiles[i*2 + 1].enabled = true;
                }
                catch {
                }
            }
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_SystemManager.BulletsToGems(2500);
        m_MoveVector = new MoveVector(1.5f, 0f);
        
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.MiddleBossClear();
    }

    public void OnBossDeath() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}

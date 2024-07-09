using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss5a : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss5a_MainTurret m_MainTurret;
    public EnemyMiddleBoss5a_SubTurret[] m_SubTurret = new EnemyMiddleBoss5a_SubTurret[2];
    
    private int _phase;
    private EnemyMissile[] _enemyMissiles;
    private EnemyItemCreater[] _enemyMissileItemCreaters;
    private Collider2D[] _enemyMissileCollider2Ds;
    
    private readonly Vector3 TARGET_POSITION = new (0f, -4f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2500;
    private const int TIME_LIMIT = 38000;

    private IEnumerator _currentPhase;
    private IEnumerator _timeLimit;
    private IEnumerator _missileLaunch;

    private void Start()
    {
        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;

        _enemyMissiles = GetComponentsInChildren<EnemyMissile>();
        _enemyMissileItemCreaters = new EnemyItemCreater[_enemyMissiles.Length];
        _enemyMissileCollider2Ds = new Collider2D[_enemyMissiles.Length];
        for (var i = 0; i < _enemyMissileItemCreaters.Length; ++i)
        {
            _enemyMissileItemCreaters[i] = _enemyMissiles[i].GetComponent<EnemyItemCreater>();
            _enemyMissileCollider2Ds[i] = _enemyMissiles[i].GetComponentInChildren<Collider2D>();
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        if (!TimeLimitState && _phase > 0) {
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

    private IEnumerator AppearanceSequence() {
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

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(80f, 100f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.4f, random_direction);
        _phase = 1;
        _currentPhase = Phase1();
        StartCoroutine(_currentPhase);

        _timeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(_timeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0)
    {
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
        while (_phase == 1)
        {
            m_MainTurret.StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_MainTurret_A(m_MainTurret));
            yield return new WaitForMillisecondFrames(2000);
            m_SubTurret[0].SetRotatePattern(new RotatePattern_RotateAround(240f));
            m_SubTurret[1].SetRotatePattern(new RotatePattern_RotateAround(-240f));
            m_SubTurret[0].StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_SubTurret_A(m_SubTurret[0]));
            m_SubTurret[1].StartPattern("A", new BulletPattern_EnemyMiddleBoss5a_SubTurret_A(m_SubTurret[1]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StopPattern("A");
            yield return new WaitForMillisecondFrames(2000);
            m_SubTurret[0].StopPattern("A");
            m_SubTurret[1].StopPattern("A");
            m_SubTurret[0].SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
            m_SubTurret[1].SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
            yield return new WaitForMillisecondFrames(2200);
            m_MainTurret.StartPattern("D", new BulletPattern_EnemyMiddleBoss5a_MainTurret_D(m_MainTurret));
            m_MainTurret.PlayerSweepRotatePattern = -1;
            yield return new WaitForMillisecondFrames(500);
            m_MainTurret.StopPattern("D");

            yield return new WaitForMillisecondFrames(500);
            _missileLaunch = LaunchMissile();
            StartCoroutine(_missileLaunch);
            yield return new WaitForMillisecondFrames(1000);
            m_MainTurret.StartPattern("B", new BulletPattern_EnemyMiddleBoss5a_MainTurret_B(m_MainTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern("B");
            m_MainTurret.PlayerSweepRotatePattern = 0;
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
            m_SubTurret[1].SetRotatePattern(new RotatePattern_RotateAround(240f));
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
    }


    private IEnumerator LaunchMissile()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForMillisecondFrames(2000);
            if (m_EnemyDeath.IsDead)
            {
                break;
            }
            if (_phase > 0)
            {
                _enemyMissiles[i * 2].Launch();
                _enemyMissiles[i * 2 + 1].Launch();
                _enemyMissileItemCreaters[i * 2].enabled = true;
                _enemyMissileItemCreaters[i * 2 + 1].enabled = true;
                _enemyMissileCollider2Ds[i * 2].enabled = true;
                _enemyMissileCollider2Ds[i * 2 + 1].enabled = true;
            }
        }
    }

    private void DestroyLaunchingMissile()
    {
        foreach (var enemyMissile in _enemyMissiles)
        {
            if (enemyMissile == null)
            {
                continue;
            }
            enemyMissile.DestroyIfNotInteractable();
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2500);
        if (_currentPhase != null)
            StopCoroutine(_currentPhase);
        if (_timeLimit != null)
            StopCoroutine(_timeLimit);
        
        DestroyLaunchingMissile();
        m_MoveVector = new MoveVector(1.5f, 0f);
        
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

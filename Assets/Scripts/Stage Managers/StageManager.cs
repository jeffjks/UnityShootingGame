using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public abstract class StageManager : MonoBehaviour
{
    [SerializeField] protected GameObject[] m_BossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_MiddleBossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_EnemyPreloaded = new GameObject[3];
    [Space(10)]
    [SerializeField] protected GameObject m_EnemySpawners = null;
    
    [SerializeField] private EnemyUnitPrefabDatas m_EnemyUnitPrefabDatas;
    public static event Action Action_BossWarningSign;
    public static event Action<EnemyUnit> Action_BossHealthBar;

    private BossHealthBarHandler m_BossHealthBar;
    private readonly Vector3[] m_BossOnlyBackgroundLocalPositions = {
        new (0.00000000f, 50.00000000f, 284.35050000f + 64f),
        new (15.98016000f, 50.00000000f, 76.13345000f + 64f),
        new (-24.95980000f, 50.00000000f, 29.27469000f + 64f),
        new (14.00522000f, 50.00000000f, 83.86731000f + 64f),
        new (0.00000000f, 50.00000000f, 387.40050000f + 64f)
    };
    private readonly Vector3[] m_BossOnlyBackgroundMoveVectors = {
        new (0f, 0f, 2.7f),
        new (0f, 0f, 0.96f),
        new (0f, 0f, 0.96f),
        new (0f, 0f, 1f),
        new (0f, 0f, 1f)
    };

    public static bool IsTrueBossEnabled { get; set; } // 일반 스테이지는 시작시 false, Hell 난이도 최종 스테이지는 시작시 true

    private Dictionary<string, EnemyBuilder> m_EnemyBuilders = default;

    protected abstract void Init();
    protected abstract IEnumerator MainTimeline();
    protected abstract IEnumerator EnemyTimeline();
    protected abstract IEnumerator BossTimeline();
    protected abstract IEnumerator TestTimeline();
    
    public static StageManager Instance { get; private set; }

    void Start ()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        SystemManager.PlayState = PlayState.None;
        Init();
        
        //UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 0f;
        PoolingManager.ResetPool();
        FadeScreenService.ScreenFadeIn(0f);
        
        InGameScreenEffectService.TransitionIn();

        BackgroundCamera.SetBackgroundSpeed(0f);
        if (SystemManager.GameMode == GameMode.Training && SystemManager.TrainingInfo.bossOnly) {
            StartBossTimeline();
        }
        else {
            StartCoroutine(MainTimeline());
            StartCoroutine(EnemyTimeline());
        }
    }

    private void Awake()
    {
        SystemManager.Action_OnStageClear += StopAllCoroutines;
    }

    private void OnDestroy()
    {
        SystemManager.Action_OnStageClear -= StopAllCoroutines;
    }

    protected void ShowBossWarningSign()
    {
        Action_BossWarningSign?.Invoke();
    }

    protected void StartBossTimeline() {
        if (SystemManager.GameMode == GameMode.Training && SystemManager.TrainingInfo.bossOnly) {
            int stage = SystemManager.Stage;
            BackgroundCamera.Instance.transform.localPosition = m_BossOnlyBackgroundLocalPositions[stage];
            BackgroundCamera.SetBackgroundSpeed(m_BossOnlyBackgroundMoveVectors[stage]);
        }
        StartCoroutine(BossTimeline());
    }

    protected void BackgroundLoop(float z, float subtract) {
        Vector3 pos = BackgroundCamera.Instance.transform.position;
        if (pos.z > z - 24f) {
            BackgroundCamera.Instance.transform.position = new Vector3(pos.x, pos.y, pos.z - subtract);
        }
    }

    protected GameObject CreateEnemy(GameObject obj, Vector3 pos) {
        if (Utility.CheckLayer(obj, Layer.AIR)) {
            pos = new Vector3(pos.x, pos.y, Depth.ENEMY);
        }
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        return ins;
    }
    
    protected GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector2 target_pos, int duration) { // Only Air Unit (where T: HasTargetPosition)
        GameObject ins = CreateEnemy(obj, pos);
        ITargetPosition enemy_unit = ins.GetComponent<ITargetPosition>();
        try {
            enemy_unit.MoveTowardsToTarget(target_pos, duration);
        }
        catch (System.NullReferenceException e) {
            Debug.LogError(e);
        }
        return ins;
    }
    
    protected GameObject CreateEnemyWithMoveVector(GameObject obj, Vector3 pos, MoveVector moveVector, MovePattern[] movePattern = null) { // delay 후 time에 걸쳐 속도 0으로
        GameObject ins = CreateEnemy(obj, pos);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = moveVector;
        
        if (movePattern == null) {
            return ins;
        }
        for (int i = 0; i < movePattern.Length; i++) {
            enemy_unit.m_TweenDataQueue.Enqueue(new TweenData(movePattern[i]));
        }
        enemy_unit.StartPlayTweenData();

        return ins;
    }

    protected IEnumerator MiddleBossStart(Vector3 pos, int millisecond, byte number = 0) { // millisecond 후 체력바 활성화, number = 중간보스 번호
        GameObject middle_boss;
        middle_boss = CreateEnemy(m_MiddleBossUnit[number], pos);
        EnemyUnit enemy_unit = middle_boss.GetComponent<EnemyUnit>();

        yield return new WaitForMillisecondFrames(millisecond);
        Action_BossHealthBar?.Invoke(enemy_unit);
        //SystemManager.PlayState = PlayState.OnMiddleBoss;
    }

    protected IEnumerator BossStart(Vector3 pos, int millisecond, byte number = 0) { // millisecond 후 체력바 활성화
        GameObject boss;
        boss = CreateEnemy(m_BossUnit[number], pos);
        EnemyUnit enemy_unit = boss.GetComponent<EnemyUnit>();

        yield return new WaitForMillisecondFrames(millisecond);
        Action_BossHealthBar?.Invoke(enemy_unit);
        //SystemManager.PlayState = PlayState.OnMiddleBoss;
    }

    protected void InitEnemies() {
        m_EnemySpawners.SetActive(true);
        for (int i = 0; i < 3; i++)
            m_EnemyPreloaded[i].SetActive(true);
    }

    public void StartFinalBoss(Vector3 pos) {
        BackgroundCamera.SetBackgroundSpeed(new Vector3(0f, 0f, 8f));
        AudioService.PlayMusic("FinalBoss");
        StartCoroutine(BossStart(pos, 1700, 1)); // True Last Boss
    }

    protected EnemyBuilder GetEnemyBuilder(string key)
    {
        return m_EnemyBuilders[key];
    }
}
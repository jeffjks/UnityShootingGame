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
    [SerializeField] protected GameObject[] m_EnemyPreloaded = new GameObject[Difficulty.DIFFICULTY_SIZE];
    [Space(10)]
    [SerializeField] protected GameObject m_EnemySpawners = null;
    
    [SerializeField] private EnemyUnitPrefabDatas m_EnemyUnitPrefabDatas;

    [HideInInspector] public Vector3 m_BackgroundVector;

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;

    private BossHealthBarHandler m_BossHealthBar;
    private readonly Vector3[] m_BossOnlyBackgroundLocalPositions = {
        new (0.00000000f, 50.00000000f, 284.35050000f + 64f),
        new (15.98016000f, 50.00000000f, 76.13345000f + 64f),
        new (-24.95980000f, 50.00000000f, 29.27469000f + 64f),
        new (14.00522000f, 50.00000000f, 83.86731000f + 64f),
        new (0.00000000f, 50.00000000f, 387.63410000f + 64f)
    };
    private readonly Vector3[] m_BossOnlyBackgroundMoveVectors = {
        new (0f, 0f, 2.7f),
        new (0f, 0f, 0.96f),
        new (0f, 0f, 0.96f),
        new (0f, 0f, 1f),
        new (0f, 0f, 1f)
    };

    private bool m_FinalBossAvailable = false; // 일반 스테이지는 시작시 false, Hell 난이도 최종 스테이지는 시작시 true

    private Dictionary<string, EnemyBuilder> m_EnemyBuilders = default;

    protected abstract void Init();
    protected abstract IEnumerator MainTimeline();
    protected abstract IEnumerator EnemyTimeline();
    protected abstract IEnumerator BossTimeline();
    protected abstract IEnumerator TestTimeline();

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;

        foreach (var value in m_EnemyUnitPrefabDatas.EnemyUnitPrefabs)
        {
            //m_EnemyBuilders[value.name] = new EnemyBuilder(value.prefab);
        }

        Init();

        m_PlayerManager.gameObject.SetActive(true);
        m_PoolingManager.PushToPoolAll();
    }

    void Start ()
    {
        m_BossHealthBar = m_SystemManager.m_BossHealthBar;
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 0f;
        m_PoolingManager.transform.GetChild(PoolingParent.DEBRIS).position = new Vector3(0f, 0f, 0f);
        m_PoolingManager.transform.GetChild(PoolingParent.ITEM_GEM_GROUND).position = new Vector3(0f, 0f, 0f);
        m_SystemManager.m_StageManager = this;
        m_SystemManager.ScreenEffect(2);

        SetBackgroundSpeed(0f);
        if (m_SystemManager.m_GameMode == GameMode.GAMEMODE_TRAINING && m_SystemManager.m_TrainingInfo.m_BossOnly) {
            StartBossTimeline();
        }
        else {
            StartCoroutine(MainTimeline());
            StartCoroutine(EnemyTimeline());
        }
    }

    protected void StartBossTimeline() {
        if (m_SystemManager.m_GameMode == GameMode.GAMEMODE_TRAINING && m_SystemManager.m_TrainingInfo.m_BossOnly) {
            int stage = m_SystemManager.GetCurrentStage();
            m_SystemManager.m_BackgroundCamera.transform.localPosition = m_BossOnlyBackgroundLocalPositions[stage];
            SetBackgroundSpeed(m_BossOnlyBackgroundMoveVectors[stage]);
        }
        StartCoroutine(BossTimeline());
    }

    protected void BackgroundLoop(float z, float subtract) {
        Vector3 pos = m_SystemManager.m_BackgroundCamera.transform.position;
        if (pos.z > z - 24f) {
            m_SystemManager.m_BackgroundCamera.transform.position = new Vector3(pos.x, pos.y, pos.z - subtract);
        }
    }

    protected GameObject CreateEnemy(GameObject obj, Vector3 pos) {
        if ((1 << obj.layer & Layer.AIR) != 0) {
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
        m_BossHealthBar.StartHealthListener(enemy_unit);
        m_SystemManager.m_PlayState = 1;
    }

    protected IEnumerator BossStart(Vector3 pos, int millisecond, byte number = 0) { // millisecond 후 체력바 활성화
        GameObject boss;
        boss = CreateEnemy(m_BossUnit[number], pos);
        EnemyUnit enemy_unit = boss.GetComponent<EnemyUnit>();

        yield return new WaitForMillisecondFrames(millisecond);
        m_BossHealthBar.StartHealthListener(enemy_unit);
        m_SystemManager.m_PlayState = 1;
    }

    public void SetBackgroundSpeed(float target, int millisecond = 0) {
        StartCoroutine(BackgroundSpeedCoroutine(target, millisecond));
    }

    public void SetBackgroundSpeed(Vector3 target, int millisecond = 0) { // Overloading
        StartCoroutine(BackgroundSpeedCoroutine(target, millisecond));
    }

    private IEnumerator BackgroundSpeedCoroutine(float target, int millisecond = 0) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            m_BackgroundVector.z = target;
            yield break;
        }

        float init_vector_z = m_BackgroundVector.z;
        
        for (int i = 0; i < frame; ++i) {
            m_BackgroundVector.z = init_vector_z + (target - init_vector_z)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    private IEnumerator BackgroundSpeedCoroutine(Vector3 target_vector, int millisecond = 0) { // Overloading
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame == 0) { // 즉시 종료
            m_BackgroundVector = target_vector;
            yield break;
        }

        Vector3 init_vector = m_BackgroundVector;

        for (int i = 0; i < frame; ++i) {
            m_BackgroundVector = init_vector + (target_vector - init_vector)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
    }

    protected void InitEnemies() {
        m_EnemySpawners.SetActive(true);
        for (int i = 0; i < 3; i++)
            m_EnemyPreloaded[i].SetActive(true);
    }

    void OnDestroy() {
        StopAllCoroutines();
    }

    public bool GetTrueLastBossState() {
        return m_FinalBossAvailable;
    }

    public void SetTrueLastBossState(bool state) {
        m_FinalBossAvailable = state;
    }

    protected EnemyBuilder GetEnemyBuilder(string key)
    {
        return m_EnemyBuilders[key];
    }
}
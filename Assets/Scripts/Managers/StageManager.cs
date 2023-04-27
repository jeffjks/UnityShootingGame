using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class StageManager : MonoBehaviour
{
    [SerializeField] protected AudioSource m_AudioStage = null;
    [SerializeField] protected AudioSource m_AudioBoss = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_BossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_MiddleBossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_EnemyPreloaded = new GameObject[Difficulty.DIFFICULTY_SIZE];
    [Space(10)]
    [SerializeField] protected GameObject m_EnemySpawners = null;

    [HideInInspector] public Vector3 m_BackgroundVector;

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;

    private BossHealthHandler m_BossHealthBar;
    private Vector3[] m_BossOnlyBackgroundLocalPositions = {
        new Vector3(0.00000000f, 50.00000000f, 284.35050000f + 64f),
        new Vector3(15.98016000f, 50.00000000f, 76.13345000f + 64f),
        new Vector3(-24.95980000f, 50.00000000f, 29.27469000f + 64f),
        new Vector3(14.00522000f, 50.00000000f, 83.86731000f + 64f),
        new Vector3(0.00000000f, 50.00000000f, 387.63410000f + 64f)
    };
    private Vector3[] m_BossOnlyBackgroundMoveVectors = {
        new Vector3(0f, 0f, 2.7f),
        new Vector3(0f, 0f, 0.96f),
        new Vector3(0f, 0f, 0.96f),
        new Vector3(0f, 0f, 1f),
        new Vector3(0f, 0f, 1f)
    };

    private bool m_TrueLastBoss = false; // 일반 스테이지는 시작시 false, Hell 난이도 최종 스테이지는 시작시 true
    
    protected class MovePattern
    {
        public int delay;
        public float speed;
        public float direction;
        public int duration;

        public MovePattern(int delay, float direction, float speed, int duration) {
            this.delay = delay;
            this.direction = direction;
            this.speed = speed;
            this.duration = duration;
        }
    }

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
        if (m_SystemManager.m_GameType == GameType.GAMETYPE_TRAINING && m_SystemManager.m_TrainingInfo.m_BossOnly) {
            StartBossTimeline();
        }
        else {
            StartCoroutine(MainTimeline());
            StartCoroutine(EnemyTimeline());
        }
    }

    protected virtual void Update()
    {
        MusicLoop();
    }

    protected void StartBossTimeline() {
        if (m_SystemManager.m_GameType == GameType.GAMETYPE_TRAINING && m_SystemManager.m_TrainingInfo.m_BossOnly) {
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

    private void MusicLoop() {
        if (m_SystemManager.m_PlayState == 0) {
            switch(m_SystemManager.GetCurrentStage()) {
                case 2: // Stage 3
                    if (m_AudioStage.time > 215.248f) {
                        m_AudioStage.time = 14.496f;
                    }
                    break;
                case 3: // Stage 4
                    if (m_AudioStage.time > 168.228f) {
                        m_AudioStage.time = 4.518f;
                    }
                    break;
                case 4: // Stage 5
                    if (m_AudioStage.time > 182.654f) {
                        m_AudioStage.time = 94.74f;
                    }
                    break;
                default:
                    break;
            }
        }
        else if (m_SystemManager.m_PlayState == 1) {
            switch(m_SystemManager.GetCurrentStage()) {
                case 4:
                    if (m_TrueLastBoss || m_SystemManager.GetDifficulty() < 2) { // Last Boss
                        if (m_AudioBoss.time > 101.168f) {
                            m_AudioBoss.time = 12.77f;
                        }
                    }
                    else { // True Last Boss
                        if (m_AudioBoss.time > 117.694f) {
                            m_AudioBoss.time = 9.697f;
                        }
                    }
                    break;
                default: // Boss
                    if (m_AudioBoss.time > 128.077f) {
                        m_AudioBoss.time = 6.49f;
                    }
                    break;
            }
        }
    }

    private GameObject InstantiateEnemyObject(GameObject obj, Vector3 pos) {
        if ((1 << obj.layer & Layer.AIR) != 0) {
            pos = new Vector3(pos.x, pos.y, Depth.ENEMY);
        }
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        return ins;
    }

    protected GameObject CreateEnemy(GameObject obj, Vector3 pos) { // attackable 후 attackable 활성화
        GameObject ins = InstantiateEnemyObject(obj, pos);
        //EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        //enemy_unit.DisableInteractable(attackable_millisecond);
        return ins;
    }
    
    protected GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector2 target_pos, int duration) { // Only Air Unit (where T: HasTargetPosition)
        GameObject ins = InstantiateEnemyObject(obj, pos);
        ITargetPosition enemy_unit = ins.GetComponent<ITargetPosition>();
        enemy_unit.MoveTowardsToTarget(target_pos, duration);
        return ins;
    }
    
    protected GameObject CreateEnemyWithMoveVector(GameObject obj, Vector3 pos, MoveVector moveVector, MovePattern[] movePattern = null) { // delay 후 time에 걸쳐 속도 0으로
        GameObject ins = InstantiateEnemyObject(obj, pos);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = moveVector;
        
        if (movePattern == null) {
            return ins;
        }
        for (int i = 0; i < movePattern.Length; i++) {
            int pattern_delay = movePattern[i].delay;
            MoveVector pattern_moveVector = new MoveVector(movePattern[i].speed, movePattern[i].direction);
            int pattern_duration = movePattern[i].duration;

            enemy_unit.m_TweenDataQueue.Enqueue(new TweenData(pattern_delay));
            enemy_unit.m_TweenDataQueue.Enqueue(new TweenDataMoveVector(pattern_moveVector, pattern_duration));

            //sequence.AppendInterval(pattern_delay)
            //.Append(DOTween.To(()=>enemy_unit.m_MoveVector.direction, x=>enemy_unit.m_MoveVector.direction = x, pattern_direction, pattern_duration).SetEase(Ease.Linear))
            //.Join(DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, pattern_speed, pattern_duration).SetEase(Ease.Linear));
        }
        return ins;
    }

    protected IEnumerator MiddleBossStart(Vector3 pos, int millisecond, byte number = 0) { // millisecond 후 체력바 활성화, number = 중간보스 번호
        int frame = millisecond * Application.targetFrameRate / 1000;
        GameObject middle_boss;
        middle_boss = CreateEnemy(m_MiddleBossUnit[number], pos);
        EnemyUnit enemy_unit = middle_boss.GetComponent<EnemyUnit>();
        m_BossHealthBar.m_EnemyUnitBoss = enemy_unit;

        yield return new WaitForFrames(frame);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }

    protected IEnumerator BossStart(Vector3 pos, int millisecond, byte number = 0) { // millisecond 후 체력바 활성화
        int frame = millisecond * Application.targetFrameRate / 1000;
        GameObject boss;
        boss = CreateEnemy(m_BossUnit[number], pos);
        m_BossHealthBar.m_EnemyUnitBoss = boss.GetComponent<EnemyUnit>();

        yield return new WaitForFrames(frame);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }

    protected IEnumerator FadeOutMusic(float duration = 3.3f) {
        m_AudioStage.DOFade(0f, duration);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_AudioStage);
        m_AudioStage.Stop();
        yield break;
    }

    protected void PlayBossMusic() {
        m_AudioBoss.Play();
    }

    public void StopMusic() {
        m_AudioBoss.Stop();
        m_AudioStage.Stop();
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
        yield break;
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
        yield break;
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
        return m_TrueLastBoss;
    }

    public void SetTrueLastBossState(bool state) {
        m_TrueLastBoss = state;
    }
}
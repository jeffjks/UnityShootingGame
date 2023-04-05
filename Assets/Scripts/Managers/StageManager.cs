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
    protected BossHealthHandler m_BossHealthBar;
    protected bool m_TrueLastBoss = false; // 일반 스테이지는 시작시 false, Hell 난이도 최종 스테이지는 시작시 true
    
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
    protected abstract IEnumerator MainTimeLine();
    protected abstract IEnumerator EnemyTimeLine();
    protected abstract IEnumerator BossOnlyTimeLine();
    protected abstract IEnumerator TestTimeLine();

    void Start ()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        
        m_BossHealthBar = m_SystemManager.m_BossHealthBar;
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 0f;
        m_PoolingManager.transform.GetChild(PoolingParent.DEBRIS).position = new Vector3(0f, 0f, 0f);
        m_PoolingManager.transform.GetChild(PoolingParent.ITEM_GEM_GROUND).position = new Vector3(0f, 0f, 0f);
        m_SystemManager.m_StageManager = this;

        SetBackgroundSpeed(0f);
        if (m_SystemManager.m_BossOnlyState) {
            StartCoroutine(BossOnlyTimeLine());
        }
        else if (m_SystemManager.m_DebugMod && !m_SystemManager.m_TrainingState) {
            StartCoroutine(TestTimeLine());
        }
        else {
            StartCoroutine(MainTimeLine());
            StartCoroutine(EnemyTimeLine());
        }
        Init();
    }

    protected virtual void Update()
    {
        MusicLoop();
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
                    if (m_TrueLastBoss || m_SystemManager.m_Difficulty < 2) { // Last Boss
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
        //enemy_unit.DisableAttackable(attackable_millisecond);
        return ins;
    }
    
    protected GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector2 target_pos, int duration) { // Only Air Unit (where T: HasTargetPosition)
        GameObject ins = InstantiateEnemyObject(obj, pos);
        HasTargetPosition enemy_unit = ins.GetComponent<HasTargetPosition>();
        enemy_unit.StartCoroutine(enemy_unit.MoveTowardsToTarget(target_pos, duration));
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
        //DOTween.To(()=>m_BackgroundVector, x=>m_BackgroundVector = x, target_vector, frame).SetEase(Ease.Linear);

        //yield return new WaitForFrames(frame);
        //DOTween.Kill(m_BackgroundVector.z);
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
            //m_BackgroundVector.z = m_BackgroundVector.z + (target - m_BackgroundVector.z)*(i+1) / frame;
            yield return new WaitForFrames(0);
        }
        //DOTween.To(()=>m_BackgroundVector, x=>m_BackgroundVector = x, target, frame).SetEase(Ease.Linear);

        //yield return new WaitForFrames(frame);
        //DOTween.Kill(m_BackgroundVector);
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
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
        public float delay, direction, speed, time;

        public MovePattern(float delay, float direction, float speed, float time) {
            this.delay = delay;
            this.direction = direction;
            this.speed = speed;
            this.time = time;
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
            switch(m_SystemManager.GetStage()) {
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
            switch(m_SystemManager.GetStage()) {
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

    protected GameObject CreateEnemy(GameObject obj, Vector3 pos, float attackable = 0f) { // attackable 후 attackable 활성화
        GameObject ins = InstantiateEnemyObject(obj, pos);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.DisableAttackable(attackable);
        return ins;
    }
    
    protected GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector3 target_pos, float time) { // Only Air Unit
        GameObject ins = InstantiateEnemyObject(obj, pos);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        ins.transform.DOMove(new Vector3(target_pos.x, target_pos.y, Depth.ENEMY), time).SetEase(Ease.OutQuad);
        return ins;
    }
    
    protected GameObject CreateEnemyWithMoveVector(GameObject obj, Vector3 pos, MoveVector moveVector, MovePattern[] movePattern = null) { // delay 후 time에 걸쳐 속도 0으로
        GameObject ins = InstantiateEnemyObject(obj, pos);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = moveVector;
        Sequence sequence = DOTween.Sequence();

        if (movePattern == null) {
            return ins;
        }
        else {
            for (int i = 0; i < movePattern.Length; i++) {
                float pattern_delay = movePattern[i].delay;
                float pattern_direction = movePattern[i].direction;
                float pattern_speed = movePattern[i].speed;
                float pattern_time = movePattern[i].time;

                if (pattern_direction == 8739f) {
                    pattern_direction = enemy_unit.m_MoveVector.direction;
                }
                if (pattern_speed == 8739f) {
                    pattern_speed = enemy_unit.m_MoveVector.speed;
                }

                sequence.AppendInterval(pattern_delay)
                .Append(DOTween.To(()=>enemy_unit.m_MoveVector.direction, x=>enemy_unit.m_MoveVector.direction = x, pattern_direction, pattern_time).SetEase(Ease.Linear))
                .Join(DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, pattern_speed, pattern_time).SetEase(Ease.Linear));
            }
            return ins;
        }
    }


    protected IEnumerator MiddleBossStart(Vector3 pos, float delay, byte number = 0) { // delay 후 체력바 활성화, number = 중간보스 번호
        GameObject middle_boss;
        middle_boss = CreateEnemy(m_MiddleBossUnit[number], pos);
        EnemyUnit enemy_unit = middle_boss.GetComponent<EnemyUnit>();
        m_BossHealthBar.m_EnemyUnitBoss = enemy_unit;

        yield return new WaitForSeconds(delay);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }

    protected IEnumerator BossStart(Vector3 pos, float delay, byte number = 0) { // delay 후 체력바 활성화
        GameObject boss;
        boss = CreateEnemy(m_BossUnit[number], pos);
        m_BossHealthBar.m_EnemyUnitBoss = boss.GetComponent<EnemyUnit>();

        yield return new WaitForSeconds(delay);
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

    public void SetBackgroundSpeed(float target, float duration = 0f) {
        StartCoroutine(BackgroundSpeedCoroutine(target, duration));
    }

    public void SetBackgroundSpeed(Vector3 target, float duration = 0f) { // Overloading
        StartCoroutine(BackgroundSpeedCoroutine(target, duration));
    }

    private IEnumerator BackgroundSpeedCoroutine(float target, float duration = 0f) {
        if (duration == 0f) {
            m_BackgroundVector.z = target;
            yield break;
        }
        Vector3 target_vector = new Vector3(0f, 0f, target);
        DOTween.To(()=>m_BackgroundVector, x=>m_BackgroundVector = x, target_vector, duration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_BackgroundVector.z);
        yield break;
    }

    private IEnumerator BackgroundSpeedCoroutine(Vector3 target, float duration = 0f) { // Overloading
        if (duration == 0f) {
            m_BackgroundVector = target;
            yield break;
        }
        DOTween.To(()=>m_BackgroundVector, x=>m_BackgroundVector = x, target, duration).SetEase(Ease.Linear);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_BackgroundVector);
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
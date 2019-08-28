using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class StageManager : MonoBehaviour
{
    [SerializeField] protected AudioSource m_AudioStage = null;
    [SerializeField] protected AudioSource m_AudioBoss = null;
    [Space(10)]
    [SerializeField] protected GameObject m_BossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_MiddleBossUnit = null;
    [Space(10)]
    [SerializeField] protected GameObject[] m_EnemyPreloaded = new GameObject[Difficulty.DIFFICULTY_SIZE];
    [Space(10)]
    [SerializeField] protected GameObject m_EnemySpawners = null;

    [HideInInspector] public Vector3 m_BackgroundVector;
    [HideInInspector] public int m_Stage;

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected BossHealthHandler m_BossHealthBar;

    protected abstract IEnumerator MainTimeLine();
    protected abstract IEnumerator EnemyTimeLine();
    protected abstract IEnumerator TestTimeLine();

    void Start ()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        
        m_BossHealthBar = m_SystemManager.m_BossHealthBar;
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 0f;

        SetBackgroundSpeed(0f);
        if (m_SystemManager.m_DebugMod) {
            StartCoroutine(TestTimeLine());
        }
        else {
            StartCoroutine(MainTimeLine());
            StartCoroutine(EnemyTimeLine());
        }
    }

    protected GameObject CreateEnemy(GameObject obj, Vector3 pos, float attackable = 0f) { // attackable 후 attackable 활성화
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.DisableAttackable(attackable);
        return ins;
    }
    
    protected GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector2 vector, float time) {
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        ins.transform.DOMove(vector, time).SetEase(Ease.OutQuad);
        return ins;
    }
    
    protected GameObject CreateEnemyWithMoveVector(GameObject obj, Vector3 pos, MoveVector moveVector, float delay = -1, float time = -1) {
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = moveVector;
        if (delay == -1)
            return ins;
        else {
            DOTween.Sequence()
            .AppendInterval(delay)
            .Append(DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, 0f, time).SetEase(Ease.InQuad));
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

    protected IEnumerator BossStart(Vector3 pos, float delay) { // delay 후 체력바 활성화
        GameObject boss;
        boss = CreateEnemy(m_BossUnit, pos);
        m_BossHealthBar.m_EnemyUnitBoss = boss.GetComponent<EnemyUnit>();

        yield return new WaitForSeconds(delay);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }

    protected IEnumerator FadeOutMusic() {
        float duration = 3.3f;
        m_AudioStage.DOFade(0f, duration);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_AudioStage);
        m_AudioStage.Stop();
        yield break;
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
        DOTween.To(()=>m_BackgroundVector.z, x=>m_BackgroundVector.z = x, target, duration).SetEase(Ease.Linear);

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
}


// CreateEnemyWithTarget(m_EnemyUnits[9], new Vector2(- Size.GAME_WIDTH/2 - 2f, -4f), new Vector3(-3f, -3f));
// CreateEnemy(m_EnemyUnits[3], new Vector2(4f, 3f));

// CreateEnemyWithTarget(m_EnemyUnits[9], new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -4f), 1f); // gunship
// CreateEnemy(m_EnemyUnits[9], new Vector2(4f, 3f)); // plane medium 1
// CreateEnemy(m_EnemyUnits[8], new Vector2(-1f, 3f)); // plane medium 2
// CreateEnemy(m_EnemyUnits[7], new Vector2(-1f, 3f)); // plane medium 3
// CreateEnemy(m_EnemyUnits[8], new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -1.5f)); // plane medium 4
// CreateEnemy(m_EnemyUnits[9], new Vector2(2f, 4.5f)); // plane large 2
// CreateEnemy(m_EnemyUnits[9], new Vector2(Size.GAME_BOUNDARY_RIGHT + 4f, -3f)); // plane large 1
// CreateEnemy(m_EnemyUnits[9], new Vector2(1f, 3f)); // plane large 3



/*
StartCoroutine(MiddleBossStartTest(1, 0.5f)); // Middle Boss (TEST)
    private IEnumerator MiddleBossStartTest(byte number, float delay, float attackable = 0f) // TEST
    {
        GameObject middle_boss;
        Vector3 pos = new Vector3(0f, 4f, Depth.ENEMY);
        middle_boss = CreateEnemy(m_MiddleBossUnit[number], pos, attackable);
        EnemyMiddleBoss5b enemy_unit = middle_boss.GetComponent<EnemyMiddleBoss5b>();
        m_BossHealthBar.m_EnemyUnitBoss = enemy_unit;

        yield return new WaitForSeconds(delay);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }*/
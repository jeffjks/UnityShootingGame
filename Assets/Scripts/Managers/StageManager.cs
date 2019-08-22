using UnityEngine;
using System.Collections;
using DG.Tweening;

public class StageManager : MonoBehaviour
{

    public GameObject m_TankSmall_1, m_TankSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeli_1, m_ShipSmall_1, m_PlaneMedium_1, m_PlaneMedium_3;
    [Space(10)]
    [SerializeField] private GameObject[] m_EnemyPreloaded = new GameObject[3];
    [Space(10)]
    [SerializeField] private GameObject m_EnemySpawners = null;
    [Space(10)]
    [SerializeField] private GameObject[] m_MiddleBossUnit = null;
    [Space(10)]
    [SerializeField] private GameObject m_BossUnit = null;
    [Space(10)]
    [SerializeField] private AudioSource m_AudioStage = null;
    [SerializeField] private AudioSource m_AudioBoss = null;
    [SerializeField] private BossHealthHandler m_BossHealthBar = null;

    [HideInInspector] public Vector3 m_BackgroundVector;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;
    private const float WATER_HEIGHT = 2.32f;

    void Start ()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        SetBackgroundSpeed(0f);
        StartCoroutine(MainTimeLine());
        StartCoroutine(EnemyTimeLine());
        //StartCoroutine(TestTimeLine()); // Test
    }

    private IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        m_BackgroundVector.z = -0.3f;
        SetBackgroundSpeed(-0.3f);
        yield return new WaitForSeconds(5f);
        StartCoroutine(SetBackgroundSpeedSoftly(-0.03f, 0.16f));

        yield return new WaitForSeconds(36f);
        StartCoroutine(SetBackgroundSpeedSoftly(-0.045f, 0.02f));
        StartCoroutine(MiddleBossStart(0, 1f, 2f)); // Middle Boss

        yield return new WaitForSeconds(55f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_PlayerManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(4f);
        StartCoroutine(SetBackgroundSpeedSoftly(-0.12f, 0.08f));
        m_AudioBoss.Play();
        StartCoroutine(BossStart(2f));
        yield return new WaitForSeconds(2f);
        SetBackgroundSpeed(0f);
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 240f;
        yield return null;
    }

    private IEnumerator TestTimeLine()
    {
        yield return new WaitForSeconds(7f);

        yield return null;
    }

    private IEnumerator EnemyTimeLine()
    {
        yield return new WaitForSeconds(9f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3f, -3f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2f);
        if (m_SystemManager.m_Difficulty >= 1)
            CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(1f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(5f, 3f), new Vector2(4f, -4f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(1f, -4f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(5f, -5f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(0.5f);
        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(3f, 3f), new Vector2(2f, -1.5f), Random.Range(1.2f, 1.5f));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(6f, -3f), Random.Range(1.2f, 1.5f));
        }
        yield return new WaitForSeconds(1.5f);
        CreateEnemy(m_ItemHeli_1, new Vector2(-1f, 3f)); // Item Heli
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(3f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(6f, 3f));
        yield return new WaitForSeconds(0.5f);
        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(4f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 3f));
        }
        yield return new WaitForSeconds(1.5f);

        if (m_SystemManager.m_Difficulty >= Difficulty.HELL) { // 3 small ship
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-10.055f, WATER_HEIGHT, 147.5f), new MoveVector(4f, 70f), 3.2f);
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-11.06f, WATER_HEIGHT, 150.44f), new MoveVector(4f, 70f), 3.2f);
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-13.98f, WATER_HEIGHT, 148.93f), new MoveVector(4f, 70f), 3.2f);
        }
        yield return new WaitForSeconds(8f);
        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT)
            CreateEnemy(m_PlaneMedium_3, new Vector2(-1f, 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(2f, 3f));
        yield return new WaitForSeconds(10f); // Middle Boss ==========================

        for (int i = 0; i < 10; i++) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
            }
            yield return new WaitForSeconds(1f);
        }

        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneMedium_3, new Vector2(-4f, 3f));
            if (m_SystemManager.m_Difficulty >= Difficulty.HELL)
                CreateEnemy(m_PlaneMedium_3, new Vector2(4f, 3f));
        }
        yield return new WaitForSeconds(2f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1.2f, 1.5f));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), Random.Range(1.2f, 1.5f));
        }
        yield return new WaitForSeconds(1.5f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneMedium_3, new Vector2(0f, 3f));
        }
        yield return new WaitForSeconds(1.5f);
        if (m_SystemManager.m_PlayState == 0) {
            if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT)
                CreateEnemy(m_PlaneMedium_3, new Vector2(3f, 3f));
        }
        yield return new WaitForSeconds(1f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-3f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(-6f, 3f));
        }
        yield return new WaitForSeconds(0.5f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-3f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(-6f, 3f));
        }
        yield return new WaitForSeconds(20f);
        CreateEnemy(m_ItemHeli_1, new Vector2(1.5f, 3f)); // Item Heli
        yield return new WaitForSeconds(10f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(3f, 3f));

        

        yield return null;
    }

    private GameObject CreateEnemy(GameObject obj, Vector3 pos, float attackable = 0f) { // attackable 후 attackable 활성화
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.DisableAttackable(attackable);
        return ins;
    }
    
    private GameObject CreateEnemyWithTarget(GameObject obj, Vector3 pos, Vector2 vector, float time) {
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        ins.transform.DOMove(vector, time).SetEase(Ease.OutQuad);
        return ins;
    }
    
    private GameObject CreateEnemyWithMoveVector(GameObject obj, Vector3 pos, MoveVector moveVector, float time) {
        GameObject ins = Instantiate(obj, pos, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = moveVector;
        DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, 0f, time).SetEase(Ease.InQuad);
        return ins;
    }


    private IEnumerator MiddleBossStart(byte number, float delay, float attackable = 0f) // attackable 후 attackable 활성화, delay 후 체력바 활성화
    {
        GameObject middle_boss;
        Vector3 pos = new Vector3(12f, -13f, Depth.ENEMY);
        middle_boss = CreateEnemy(m_MiddleBossUnit[number], pos, attackable);
        EnemyMiddleBoss1 enemy_unit = middle_boss.GetComponent<EnemyMiddleBoss1>();
        m_BossHealthBar.m_EnemyUnitBoss = enemy_unit;

        yield return new WaitForSeconds(delay);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }


    private IEnumerator BossStart(float delay, float timer = 0f)
    {
        GameObject boss;
        Vector3 pos = new Vector3(0f, 4f, Depth.ENEMY);
        boss = CreateEnemy(m_BossUnit, pos, timer);
        m_BossHealthBar.m_EnemyUnitBoss = boss.GetComponent<EnemyUnit>();
        yield return new WaitForSeconds(delay);
        m_SystemManager.m_PlayState = 1;
        yield break;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }

    private IEnumerator FadeOutMusic() {
        while(m_AudioStage.volume > 0) {
            m_AudioStage.volume -= 0.005f;
            yield return null;
        }
        m_AudioStage.Stop();
        yield break;
    }

    public void StopMusic() {
        m_AudioBoss.Stop();
        m_AudioStage.Stop();
    }

    public void SetBackgroundSpeed(float speed)
    {
        m_BackgroundVector.z = speed;
    }

    private IEnumerator SetBackgroundSpeedSoftly(float target, float maxDelta)
    {
        while(m_BackgroundVector.z != target) {
            m_BackgroundVector.z = Mathf.MoveTowards(m_BackgroundVector.z, target, maxDelta * Time.deltaTime); // (/60)
            yield return null;
        }
        yield break;
    }

    private void InitEnemies() {
        m_EnemySpawners.SetActive(true);
        for (int i = 0; i < 3; i++)
            m_EnemyPreloaded[i].SetActive(true);
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
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage4Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_Helicopter, m_TankLarge_2, m_Gunship,
    m_PlaneSmall_1, m_ItemHeli_1, m_ItemHeli_2, m_PlaneMedium_3, m_PlaneMedium_4, m_PlaneLarge_2, m_PlaneLarge_3;
    public Transform[] m_BossTerrains = new Transform[3];

    private float m_BackgroundPos;
    private bool m_LoopTerrain = false;

    void Awake()
    {
        m_Stage = 3;
    }

    protected override void Update()
    {
        base.Update();
        if (m_LoopTerrain)
            LoopTerrain();
    }

    private void LoopTerrain() {
        for (int i = 0; i < m_BossTerrains.Length; i++) {
            m_BossTerrains[i].position = new Vector3(m_BossTerrains[i].position.x, m_BossTerrains[i].position.y, m_BossTerrains[i].position.z - 3.12f * Time.deltaTime);
        }
        Vector3 debris_pos;
        debris_pos = m_PoolingManager.transform.GetChild(PoolingParent.DEBRIS).position;
        if (debris_pos.z > -24f) {
            m_PoolingManager.transform.GetChild(PoolingParent.DEBRIS).position = new Vector3(debris_pos.x, debris_pos.y, debris_pos.z - 3.12f * Time.deltaTime);
        }

        if (m_BossTerrains[1].position.z < 24f) {
            for (int i = 0; i < m_BossTerrains.Length; i++) {
                m_BossTerrains[i].position = new Vector3(m_BossTerrains[i].position.x, m_BossTerrains[i].position.y, m_BossTerrains[i].position.z + 60f);
            }
        }
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        SetBackgroundSpeed(1.12f);

        yield return new WaitForSeconds(74f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 2.5f, Depth.ENEMY), 1f)); // Middle Boss

        yield return new WaitForSeconds(22f);
        SetBackgroundSpeed(0f, 1.2f);
        yield return new WaitForSeconds(1.2f);

        while(m_BackgroundPos < 270f) {
            float c_size = 1f;
            SetBackgroundSpeed(new Vector3(c_size*Mathf.Cos(Mathf.Deg2Rad * (180f + m_BackgroundPos)), 0f, c_size*Mathf.Sin(Mathf.Deg2Rad * (180f + m_BackgroundPos))));
            m_BackgroundPos += 4f * Time.deltaTime;
            
            Vector3 background_pos = m_SystemManager.m_BackgroundCamera.transform.position;
            yield return null;
        }
        SetBackgroundSpeed(new Vector3(0f, 0f, 1f));
        yield return new WaitForSeconds(17f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.WarningText();
        yield return new WaitForSeconds(1f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 3.12f), 1f);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BossStart(new Vector3(14.31f, 3f, 125f), 3f)); // Boss
        yield return new WaitForSeconds(0.5f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0f));
        m_LoopTerrain = true;
        yield return new WaitForSeconds(1.5f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield break;
    }

    protected override IEnumerator BossOnlyTimeLine()
    {
        m_SystemManager.m_BackgroundCamera.transform.position = new Vector3(14.31f, 40f, 83.9f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 1f));
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.WarningText();
        yield return new WaitForSeconds(1f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 3.12f), 1f);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(BossStart(new Vector3(14.31f, 3f, 125f), 3f)); // Boss
        yield return new WaitForSeconds(0.5f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0f));
        m_LoopTerrain = true;
        yield return new WaitForSeconds(1.5f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_ItemHeli_1, new Vector2(0f, 3f)); // Item Heli 1
        StartCoroutine(SpawnHelicopters1());
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(3f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-1f, 3f));
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(2f, 3f));
        yield return new WaitForSeconds(6f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-3f, -4f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(Random.Range(-1f, 1f), -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(Random.Range(-1f, 1f), -5f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(3f, -4f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-7f, 3f), new Vector2(Random.Range(-6f, 5f), -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(7f, 3f), new Vector2(Random.Range(5f, 6f), -5f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(0f, 3f));
        yield return new WaitForSeconds(7f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(3f, 3f));
        StartCoroutine(SpawnPlaneSmalls1());
        yield return new WaitForSeconds(6f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnPlaneSmalls2());
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(2.61f, 3.18f, 56.17f), new MoveVector(0.7f, -32f), new MovePattern[] {new MovePattern(2f, 8739f, 0f, 0.7f)});
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -4f), 1.2f);
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -1f), 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(1f, 4f), 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4.5f, 3f));
        CreateEnemy(m_PlaneMedium_4, new Vector2(4.5f, 3f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnHelicopters2());
        yield return new WaitForSeconds(9f);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-2f, 3f));
        yield return new WaitForSeconds(31f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(-2f, 3f));
        }
        yield return new WaitForSeconds(4f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(3f, 3f));
        }
        yield return new WaitForSeconds(6f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(-1f, 3f));
        }
        for (int i = 0; i < 6; i++) {
            yield return new WaitForSeconds(0.5f);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            }
        }
        yield return new WaitForSeconds(4f);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(1f, 3f));
        }
        for (int i = 0; i < 6; i++) {
            yield return new WaitForSeconds(0.5f);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            }
        }
        yield return new WaitForSeconds(4f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(-24.35f, 3.21f, 84.5f), new MoveVector(-4f, -90f), new MovePattern[] {new MovePattern(2.1f, 8739f, 0f, 1.4f)});
        yield return new WaitForSeconds(6f);
        CreateEnemy(m_ItemHeli_1, new Vector2(2f, 3f)); // Item Heli 1
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-5f, 3f), new Vector2(-5f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4.5f, 3f), new Vector2(-4f, -6f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0f, -6f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_2, new Vector2(-3f, 3f)); // Item Heli 2
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4.5f, 3f), new Vector2(4.5f, -6f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(1f, 3f), new Vector2(1f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(1f, 3f), new Vector2(1f, -6f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneLarge_2, new Vector2(2.5f, 5f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -3f), 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -3f), 3f));
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 4f), 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 4f), 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneLarge_2, new Vector2(-2f, 5f));
        yield return new WaitForSeconds(6f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(3f, 3f));
        yield return new WaitForSeconds(7f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4.5f, -4.5f), 1.2f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4.5f, -4.5f), 1.2f);
        yield return new WaitForSeconds(10f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(12.7f, 3.03f, 106f), new MoveVector(2.5f, 14.785f),
            new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f), new MovePattern(2f, 8739f, -0.8f, 1f), new MovePattern(6f, 8739f, 0f, 0.5f)});
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_ItemHeli_1, new Vector2(3f, 3f)); // Item Heli 1
        yield break;
    }

    private IEnumerator SpawnHelicopters1()
    {
        for (int i = 0; i < 8; i++) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(-6f, -2f), 3f), new Vector2(Random.Range(-6f, -2f), Random.Range(-2f, -7f)), Random.Range(1.2f, 1.5f));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(2f, 6f), 3f), new Vector2(Random.Range(2f, 6f), Random.Range(-2f, -7f)), Random.Range(1.2f, 1.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters2()
    {
        for (int i = 0; i < 8; i++) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(-3f, -0.5f), 3f), new Vector2(Random.Range(-3f, -0.5f), Random.Range(-2f, -8f)), Random.Range(1.2f, 1.5f));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(0.5f, 3f), 3f), new Vector2(Random.Range(0.5f, 3f), Random.Range(-2f, -8f)), Random.Range(1.2f, 1.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls1()
    {
        for (int i = 0; i < 10; i++) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
            yield return new WaitForSeconds(0.2f);
            CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls2()
    {
        for (int i = 0; i < 4; i++) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(2.25f - 1.5f*i, 3f));
            yield return new WaitForSeconds(0.4f);
        }
        yield break;
    }
}
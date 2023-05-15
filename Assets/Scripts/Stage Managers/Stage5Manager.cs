using UnityEngine;
using System.Collections;

public class Stage5Manager : StageManager
{
    public AudioSource m_AudioTrueLastBoss;
    [Space(10)]
    public GameObject m_Helicopter, m_ItemHeliRed, m_ItemHeliGreen,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_TankSmall_1, m_TankSmall_2, m_TankSmall_3,
    m_PlaneMedium_1, m_PlaneMedium_2, m_PlaneMedium_3, m_PlaneMedium_4, m_PlaneMedium_5, m_TankMedium_1, m_TankMedium_2, m_TankMedium_3, m_Gunship,
    m_PlaneLarge_1, m_PlaneLarge_2, m_PlaneLarge_3, m_TankLarge_1, m_TankLarge_2, m_TankLarge_3,
    m_ShipSmall_1, m_ShipSmall_2, m_ShipMedium, m_ShipLarge, m_ShipCarrier_2;

    private const float WATER_HEIGHT = 2.32f;

    protected override void Init()
    {
        m_SystemManager.SetCurrentStage(4);
    }

    protected override void Update()
    {
        base.Update();

        BackgroundLoop(460f, 32f);
    }

    protected override IEnumerator TestTimeline()
    {
        //StartCoroutine(BossTimeline());
        //yield break;
        m_SystemManager.m_BackgroundCamera.transform.position = new Vector3(0f, 40f, 389.4f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 1f));
        yield return new WaitForMillisecondFrames(3000);
        StartCoroutine(FadeOutMusic(5f));
        yield return new WaitForMillisecondFrames(5000);
        m_SystemManager.WarningText();
        StartCoroutine(DarkEffect());
        yield return new WaitForMillisecondFrames(6000);
        TrueLastBoss(new Vector3(0f, -10f, Depth.ENEMY));
        yield break;
    }

    protected override IEnumerator MainTimeline()
    {
        SetBackgroundSpeed(1f);
        yield return new WaitForMillisecondFrames(1000);
        InitEnemies();
        yield return new WaitForMillisecondFrames(100000);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 5f, Depth.ENEMY), 500, 0)); // Middle Boss 0
        SetBackgroundSpeed(0.9f, 1000);
        yield return new WaitForMillisecondFrames(40000);
        SetBackgroundSpeed(1f, 1000);
        yield return new WaitForMillisecondFrames(50000);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 5f, Depth.ENEMY), 500, 1)); // Middle Boss 1
        SetBackgroundSpeed(0.9f, 1000);
        yield return new WaitForMillisecondFrames(140000);
        StartBossTimeline();
        yield break;
    }

    protected override IEnumerator BossTimeline()
    {
        CheckTrueLastBossState();
        yield return new WaitForMillisecondFrames(3000);
        StartCoroutine(FadeOutMusic(5f));
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(BossStart(new Vector3(0f, 8f, Depth.ENEMY), 9000)); // Boss
        yield return new WaitForMillisecondFrames(4000);
        m_SystemManager.WarningText();
        StartCoroutine(DarkEffect());
        yield return new WaitForMillisecondFrames(6000);
        PlayBossMusic();
        yield break;
    }

    public void TrueLastBoss(Vector3 pos) {
        SetBackgroundSpeed(new Vector3(0f, 0f, 8f));
        m_AudioBoss = m_AudioTrueLastBoss;
        PlayBossMusic();
        StartCoroutine(BossStart(pos, 1700, 1)); // True Last Boss
    }

    private IEnumerator DarkEffect()
    {
        yield break;
    }

    protected override IEnumerator EnemyTimeline()
    {
        int[] period = new int[3];
        yield return new WaitForMillisecondFrames(3000);
        StartCoroutine(SpawnSmallTanks());
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_ItemHeliRed, new Vector2(-3f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 9f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 9f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(SpawnHelicopters_A(-3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_1, new Vector2(2.5f, 3f));
        yield return new WaitForMillisecondFrames(4000);
        StartCoroutine(SpawnHelicopters_A(3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 19f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 19f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1000, 1000, true, 0f)});
        CreateEnemy(m_PlaneMedium_1, new Vector2(-2.5f, 3f));
        yield return new WaitForMillisecondFrames(5000);
        StartCoroutine(SpawnHelicopters_A(0f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(-10f, 2f, 26f), new MoveVector(2f, 65f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(10f, 2f, 26f), new MoveVector(2f, -65f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(2000);
        period = new int[] { 1000, 750, 500};
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_1, 4000, period[m_SystemManager.GetDifficulty()]));
        CreateEnemy(m_ItemHeliGreen, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(5500);
        CreateEnemy(m_PlaneLarge_1, new Vector2(11f, -3f));
        yield return new WaitForMillisecondFrames(17500);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 5f), 3f));
        yield return new WaitForMillisecondFrames(500);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 3f), 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 5f), 3f));
        yield return new WaitForMillisecondFrames(500);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 2f), 3f));
        yield return new WaitForMillisecondFrames(1000);
        if (m_SystemManager.GetDifficulty() >= Difficulty.EXPERT)
            CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(3f, 4f), 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 5f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(9f, 2f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 2f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(9f, 5f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 5f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(9f, 2f));
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-3f, 3f));
        yield return new WaitForMillisecondFrames(4000);
        StartCoroutine(SpawnPlaneSmall_2s(1000, 800));
        yield return new WaitForMillisecondFrames(8000);
        StartCoroutine(SpawnHelicopters_B(false));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4.5f, 3f));
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(-7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(-7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-3f, -4f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-6f, 3f), new Vector2(-6f, -6f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -8f), 1200);
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithMoveVector(m_TankMedium_2, new Vector3(10.66f, 2f, 95.7f), new MoveVector(4f, -60f), new MovePattern[] {new MovePattern(1500, 1200, true, 0f)});
        CreateEnemyWithMoveVector(m_TankMedium_2, new Vector3(12.96f, 2f, 91.72f), new MoveVector(4f, -60f), new MovePattern[] {new MovePattern(1500, 1200, true, 0f)});
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-2f, -5f), 1200);
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(3f, -2f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3f, -2f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(5f, 3f), new Vector2(5f, -2f), 1200);
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -3f), 1200);
        yield return new WaitForMillisecondFrames(500);
        if (m_SystemManager.GetDifficulty() == Difficulty.HELL)
            CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(0f, -6f), 1200);
        yield return new WaitForMillisecondFrames(30500); // Middle Boss 1 (137s)

        period = new int[] { 600, 300, 250 };
        StartCoroutine(SpawnPlaneSmall_2s(15000, period[m_SystemManager.GetDifficulty()]));
        if (m_SystemManager.GetDifficulty() != 0)
            StartCoroutine(SpawnPlaneSmalls_B(m_PlaneSmall_1, 15000, 600));
        yield return new WaitForMillisecondFrames(16000);
        CreateEnemy(m_ItemHeliRed, new Vector2(2f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        StartCoroutine(SpawnHelicopters_B(true));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(-12.1f, 3.51f, 167f), new MoveVector(1.2f, 45f), new MovePattern[] {new MovePattern(9000, 2000, true, 0f)});
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(SpawnHelicopters_B(false));
        yield return new WaitForMillisecondFrames(10000);
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(12.1f, 3.51f, 177.6f), new MoveVector(1.2f, -45f), new MovePattern[] {new MovePattern(10000, 2000, true, 0f)});
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        yield return new WaitForMillisecondFrames(11000);
        if (m_SystemManager.GetDifficulty() != 0)
            CreateEnemy(m_PlaneMedium_4, new Vector2(0f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -2f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-2.5f, -4f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(2.5f, -5f), 1000);
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4f, 3f));
        CreateEnemy(m_PlaneMedium_4, new Vector2(4f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-3f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-5f, -6f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(3f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(5f, -6f), 1000);
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(-5f, 2f), new Vector2(-4f, -2f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-6f, -5f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(5f, 2f), new Vector2(4f, -2f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(6f, -5f), 1000);
        yield return new WaitForMillisecondFrames(20000); // Middle Boss 2 (216s)

        StartCoroutine(SpawnPlaneMedium_3s(10000));
        yield return new WaitForMillisecondFrames(4000);
        if (m_SystemManager.m_PlayState == 0) {
            StartCoroutine(SpawnPlaneSmalls_C(m_PlaneSmall_1, true));
        }
        yield return new WaitForMillisecondFrames(2000);
        if (m_SystemManager.GetDifficulty() == Difficulty.HELL) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -9f), 1200);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(-2f, 5f), new Vector2(-2f, -7f), 1200);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 5f), new Vector2(2f, -7f), 1200);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -9f), 1200);
        }
        yield return new WaitForMillisecondFrames(2000);
        if (m_SystemManager.m_PlayState == 0) {
            StartCoroutine(SpawnPlaneSmalls_C(m_PlaneSmall_1, false));
        }
        
        yield return new WaitForMillisecondFrames(2000); // Sea
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(-4f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(0.6f, 15f));
        yield return new WaitForMillisecondFrames(2000);
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_2, 6000, 1500));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, GetBackgroundZ() + 30f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2.5f, WATER_HEIGHT, GetBackgroundZ() + 28f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 3f, WATER_HEIGHT, GetBackgroundZ() + 26f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 6f, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 6.5f, WATER_HEIGHT, GetBackgroundZ() + 29f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 7f, WATER_HEIGHT, GetBackgroundZ() + 27f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(3000);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_RIGHT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, -75f), new MovePattern[] {new MovePattern(7000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_LEFT - 4f, WATER_HEIGHT, GetBackgroundZ() + 29f),
        new MoveVector(0.6f, 110f), new MovePattern[] {new MovePattern(12000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(5.5f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(0.5f, -30f), new MovePattern[] {new MovePattern(4000, 1000, true, 0f)});
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_2, 2000, 1000));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-1f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-5f, WATER_HEIGHT, GetBackgroundZ() + 29f), new MoveVector(0f, -130f));
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-1.2f, WATER_HEIGHT, GetBackgroundZ() + 28.2f), new MoveVector(0f, -120f));
        yield return new WaitForMillisecondFrames(5000);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_LEFT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, 75f), new MovePattern[] {new MovePattern(7000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 2f), new Vector2(Random.Range(-1f, 1f), -4f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 4f), new Vector2(Random.Range(-1f, 1f), -7f), 1200);
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 2f), new Vector2(-4f, -5f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 2f), new Vector2(6f, -8f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 2f), new Vector2(4f, -5f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-6f, 2f), new Vector2(-6f, -8f), 1200);
        yield return new WaitForMillisecondFrames(1500);

        CreateEnemy(m_PlaneMedium_1, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_1, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(1000);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_RIGHT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, -75f), new MovePattern[] {new MovePattern(7000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-Random.Range(5.5f, 6.5f), WATER_HEIGHT, GetBackgroundZ() + 28f),
        new MoveVector(2f, 0f), new MovePattern[] {new MovePattern(3000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-1.5f, 3f));
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(-3.5f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(3.5f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(SpawnSmallShips());
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(0f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4000, 1000, true, 0f)});
        SetBackgroundSpeed(1.2f, 1000);
        yield return new WaitForMillisecondFrames(9000); // Final Field (287s)
        
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
        yield return new WaitForMillisecondFrames(2000);
        StartCoroutine(SpawnHelicopters_C(4000, 1300));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        yield return new WaitForMillisecondFrames(4000);
        GameObject[] temp_tanks = new GameObject[3];
        temp_tanks[0] = CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(-5f, 2f, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2500, 2000, true, -1f), new MovePattern(4000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(2500);
        temp_tanks[1] = CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(0f, 2f, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2500, 2000, true, -1f), new MovePattern(4000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(2500);
        temp_tanks[2] = CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(5f, 2f, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2500, 2000, true, -1f), new MovePattern(4000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(7600);

        for (int i = 0; i < 5; i++) {
            if (temp_tanks[0] == null && temp_tanks[1] == null && temp_tanks[2] == null) {
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-3f - Mathf.Abs(i - 2), 3f), new Vector2(-3f - Mathf.Abs(i - 2), -4f), 1000);
                CreateEnemyWithTarget(m_Helicopter, new Vector2(3f + Mathf.Abs(i - 2), 3f), new Vector2(3f + Mathf.Abs(i - 2), -4f), 1000);
            }
            yield return new WaitForMillisecondFrames(600);
        }
        yield return new WaitForMillisecondFrames(1400);

        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        yield return new WaitForMillisecondFrames(7000); // 319s

        SetBackgroundSpeed(7f, 2000);
        yield return new WaitForMillisecondFrames(2000);
        period = new int[] { 5500, 2000, 1500 };
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_1, 10000, period[m_SystemManager.GetDifficulty()]));
        yield return new WaitForMillisecondFrames(11000);
        SetBackgroundSpeed(1f, 4000);
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_ItemHeliRed, new Vector2(-3.5f, 3f));
        CreateEnemy(m_ItemHeliRed, new Vector2(3.5f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_ItemHeliGreen, new Vector2(0f, 3f));
        yield break;
    }
    

    private IEnumerator SpawnSmallTanks()
    {
        for (int i = 0; i < 6; i++) {
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(-2.5f, -1f), 2f, GetBackgroundZ() + 29f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(1f, 2.5f), 2f, GetBackgroundZ() + 29f), new MoveVector(1f, 0f));
            yield return new WaitForMillisecondFrames(1400);
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(-2.5f, -1f), 2f, GetBackgroundZ() + 29f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(1f, 2.5f), 2f, GetBackgroundZ() + 29f), new MoveVector(1f, 0f));
            yield return new WaitForMillisecondFrames(1400);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters_A(float pos)
    {
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 2f, 3f), new Vector2(pos - 2f, -6f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -6f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 2f, 3f), new Vector2(pos + 2f, -6f), 1200);
        yield return new WaitForMillisecondFrames(400);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 0.75f, 3f), new Vector2(pos - 0.75f, -4f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 0.75f, 3f), new Vector2(pos + 0.75f, -4f), 1200);
        yield return new WaitForMillisecondFrames(400);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 2f, 3f), new Vector2(pos - 2f, -2f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -2f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 2f, 3f), new Vector2(pos + 2f, -2f), 1200);
        yield break;
    }

    private IEnumerator SpawnHelicopters_B(bool right_side)
    {
        float pos;
        for (int i = 0; i < 4; i++) {
            if (right_side)
                pos = 6f - i*1.8f;
            else
                pos = -6f + i*1.8f;
            CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -2f - (3-i)*0.2f), 1100);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -4f - (3-i)*0.2f), 1100);
            yield return new WaitForMillisecondFrames(320);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters_C(int duration, int period)
    {
        int timer = 0;
        float rand, target_y = -2f;
        while (timer < duration) {
            rand = Random.Range(-7f, 7f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(rand, 3f), new Vector2(rand, target_y), Random.Range(1200, 1500));
            yield return new WaitForMillisecondFrames(period);
            target_y -= 1f;
            if (target_y > -5f)
                target_y = -2f;
            timer += period;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_A(GameObject plane, int duration, int period) // 3 2 형식
    {
        int timer = 0;
        while (timer < duration) {
            CreateEnemy(plane, new Vector2(Random.Range(-7f, -5f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(5f, 7f), Random.Range(2f, 4f)));
            yield return new WaitForMillisecondFrames(period);
            CreateEnemy(plane, new Vector2(Random.Range(-4f, -2f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(2f, 4f), Random.Range(2f, 4f)));
            yield return new WaitForMillisecondFrames(period);
            timer += period*2;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_B(GameObject plane, int duration, int period) // 아래에서 소환
    {
        int timer = 0;
        while (timer < duration) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(plane, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, Random.Range(-2f, -3f)));
                CreateEnemy(plane, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, Random.Range(-2f, -3f)));
            }
            yield return new WaitForMillisecondFrames(period);
            timer += period;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_C(GameObject plane, bool right_side)
    {
        float pos;
        for (int i = 0; i < 4; i++) {
            if (right_side)
                pos = 7f - i*2f;
            else
                pos = -7f + i*2f;
            CreateEnemy(plane, new Vector2(pos, 2f));
            CreateEnemy(plane, new Vector2(pos, 4f));
            yield return new WaitForMillisecondFrames(320);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmall_2s(int duration, int period)
    {
        int timer = 0;
        while (timer < duration) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-7f, -5f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(5f, 7f), Random.Range(1f, 4f)));
            }
            yield return new WaitForMillisecondFrames(period);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-4f, -2f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(2f, 4f), Random.Range(1f, 4f)));
            }
            yield return new WaitForMillisecondFrames(period);
            timer += period*2;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneMedium_3s(int duration)
    {
        int timer = 0, rand_period;
        int[] period = { 1100, 850, 700 };
        while (timer < duration) {
            rand_period = Random.Range(0, 300);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 6f), 3f));
            }
            yield return new WaitForMillisecondFrames(period[m_SystemManager.GetDifficulty()] - rand_period);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(1f, 6f), 3f));
            }
            yield return new WaitForMillisecondFrames(period[m_SystemManager.GetDifficulty()] + rand_period);
            timer += period[m_SystemManager.GetDifficulty()]*2;
        }
        yield break;
    }

    private IEnumerator SpawnSmallShips() {
        GameObject[] ship = {m_ShipSmall_1, m_ShipSmall_2};
        for (int i = 0; i < 2; i++) {
            CreateEnemyWithMoveVector(ship[i], new Vector3(Size.GAME_BOUNDARY_LEFT - 2f, WATER_HEIGHT, GetBackgroundZ() + 26f),
            new MoveVector(1.3f, 50f), new MovePattern[] {new MovePattern(2700, 1200, true, 0f)});
            CreateEnemyWithMoveVector(ship[i], new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, GetBackgroundZ() + 26f),
            new MoveVector(1.3f, -50f), new MovePattern[] {new MovePattern(2700, 1200, true, 0f)});
            yield return new WaitForMillisecondFrames(4300);
        }
        yield break;
    }

    private float GetBackgroundZ() {
        return m_SystemManager.m_BackgroundCamera.transform.position.z;
    }

    private void CheckTrueLastBossState() {
        if (m_SystemManager.GetDifficulty() == Difficulty.HELL) {
            SetTrueLastBossState(true);
        }
    }
}
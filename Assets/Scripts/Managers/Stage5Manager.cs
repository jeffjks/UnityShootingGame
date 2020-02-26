using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage5Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_Helicopter, m_ItemHeli_1, m_ItemHeli_2,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_TankSmall_1, m_TankSmall_2, m_TankSmall_3,
    m_PlaneMedium_1, m_PlaneMedium_2, m_PlaneMedium_3, m_PlaneMedium_4, m_PlaneMedium_5, m_TankMedium_1, m_TankMedium_2, m_TankMedium_3, m_Gunship,
    m_PlaneLarge_1, m_PlaneLarge_2, m_PlaneLarge_3, m_TankLarge_1, m_TankLarge_2, m_TankLarge_3,
    m_ShipSmall_1, m_ShipSmall_2, m_ShipMedium, m_ShipLarge, m_ShipCarrier_2;

    private const float WATER_HEIGHT = 2.32f;

    void Awake()
    {
        m_Stage = 4;
    }

    protected override IEnumerator MainTimeLine()
    {
        SetBackgroundSpeed(1f);
        yield return new WaitForSeconds(1f);
        InitEnemies();
        yield return new WaitForSeconds(100f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 5f, Depth.ENEMY), 0.5f, 0)); // Middle Boss 0
        SetBackgroundSpeed(0.9f, 1f);
        yield return new WaitForSeconds(40f);
        SetBackgroundSpeed(1f, 1f);
        yield return new WaitForSeconds(50f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 5f, Depth.ENEMY), 0.5f, 1)); // Middle Boss 1
        SetBackgroundSpeed(0.9f, 1f);
        yield return new WaitForSeconds(142f);
        
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.WarningText();
        yield return new WaitForSeconds(1f);
        //StartCoroutine(BossStart(new Vector3(9.5f, -12.5f, Depth.ENEMY), 3f)); // Boss
        yield return new WaitForSeconds(5f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield break;
    }

    protected override IEnumerator BossOnlyTimeLine()
    {
        m_SystemManager.m_BackgroundCamera.transform.position = new Vector3(0f, 40f, 389.4f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 1f));
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.WarningText();
        yield return new WaitForSeconds(1f);
        //StartCoroutine(BossStart(new Vector3(9.5f, -12.5f, Depth.ENEMY), 3f)); // Boss
        yield return new WaitForSeconds(5f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        float[] period = new float[3];
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnSmallTanks());
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_1, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 9f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 9f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnHelicopters_A(-3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(2.5f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnHelicopters_A(3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 19f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 19f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemy(m_PlaneMedium_1, new Vector2(-2.5f, 3f));
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnHelicopters_A(0f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(-10f, 2f, 26f), new MoveVector(2f, 65f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(10f, 2f, 26f), new MoveVector(2f, -65f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2f);
        period = new float[] {1f, 0.75f, 0.5f};
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_1, 4f, period[m_SystemManager.m_Difficulty]));
        CreateEnemy(m_ItemHeli_2, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(6f);
        CreateEnemy(m_PlaneLarge_1, new Vector2(9f, -3f));
        yield return new WaitForSeconds(17f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 5f), 3f));
        yield return new WaitForSeconds(0.5f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 3f), 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 5f), 3f));
        yield return new WaitForSeconds(0.5f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 3f), 3f));
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(9f, -3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(9f, -3f));
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnPlaneSmall_2s(1f, 0.8f));
        yield return new WaitForSeconds(8f);
        StartCoroutine(SpawnHelicopters_B(false));
        yield return new WaitForSeconds(10f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(-7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(-7f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-3f, -4f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-6f, 3f), new Vector2(-6f, -6f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -8f), 1.2f);
        yield return new WaitForSeconds(3f);
        CreateEnemyWithMoveVector(m_TankMedium_2, new Vector3(10.66f, 2f, 95.7f), new MoveVector(4f, -60f), new MovePattern[] {new MovePattern(1.5f, 8739f, 0f, 1.2f)});
        CreateEnemyWithMoveVector(m_TankMedium_2, new Vector3(12.96f, 2f, 91.72f), new MoveVector(4f, -60f), new MovePattern[] {new MovePattern(1.5f, 8739f, 0f, 1.2f)});
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-2f, -5f), 1.2f);
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(3f, -2f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3f, -2f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(5f, 3f), new Vector2(5f, -2f), 1.2f);
        yield return new WaitForSeconds(1f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -3f), 1.2f);
        yield return new WaitForSeconds(0.5f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(0f, -6f), 1.2f);
        yield return new WaitForSeconds(30.5f); // Middle Boss 1 (137s)

        period = new float[] {0.6f, 0.3f, 0.25f};
        StartCoroutine(SpawnPlaneSmall_2s(15f, period[m_SystemManager.m_Difficulty]));
        if (m_SystemManager.m_Difficulty != 0)
            StartCoroutine(SpawnPlaneSmalls_B(m_PlaneSmall_1, 15f, 0.6f));
        yield return new WaitForSeconds(16f);
        CreateEnemy(m_ItemHeli_1, new Vector2(2f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnHelicopters_B(true));
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnHelicopters_B(false));
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(-12.1f, 3.51f, 167f), new MoveVector(1.2f, 45f), new MovePattern[] {new MovePattern(9f, 8739f, 0f, 2f)});
        yield return new WaitForSeconds(11f);
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(12.1f, 3.51f, 177.6f), new MoveVector(1.2f, -45f), new MovePattern[] {new MovePattern(10f, 8739f, 0f, 2f)});
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        yield return new WaitForSeconds(11f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(0f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-2.5f, -4f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(2.5f, -5f), 1f);
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4f, 3f));
        CreateEnemy(m_PlaneMedium_4, new Vector2(4f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-3f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-5f, -6f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(3f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(5f, -6f), 1f);
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(-5f, 2f), new Vector2(-4f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-6f, -5f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(5f, 2f), new Vector2(4f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(6f, -5f), 1f);
        yield return new WaitForSeconds(20f); // Middle Boss 2 (216s)

        StartCoroutine(SpawnPlaneMedium_3s(10f));
        yield return new WaitForSeconds(4f);
        if (m_SystemManager.m_PlayState == 0) {
            StartCoroutine(SpawnPlaneSmalls_C(m_PlaneSmall_1, true));
        }
        yield return new WaitForSeconds(4f);
        if (m_SystemManager.m_PlayState == 0) {
            StartCoroutine(SpawnPlaneSmalls_C(m_PlaneSmall_1, false));
        }
        
        yield return new WaitForSeconds(2f); // Sea
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(-4f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(0.6f, 15f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_2, 6f, 1.5f));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, GetBackgroundZ() + 30f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2.5f, WATER_HEIGHT, GetBackgroundZ() + 28f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 3f, WATER_HEIGHT, GetBackgroundZ() + 26f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 6f, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 6.5f, WATER_HEIGHT, GetBackgroundZ() + 29f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 7f, WATER_HEIGHT, GetBackgroundZ() + 27f),
        new MoveVector(2.5f, -64f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(3f);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_RIGHT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, -75f), new MovePattern[] {new MovePattern(7f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(5f);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_LEFT - 4f, WATER_HEIGHT, GetBackgroundZ() + 29f),
        new MoveVector(0.6f, 110f), new MovePattern[] {new MovePattern(12f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(4f);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(5.5f, WATER_HEIGHT, GetBackgroundZ() + 32f),
        new MoveVector(0.5f, -30f), new MovePattern[] {new MovePattern(4f, 8739f, 0f, 1f)});
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_2, 2f, 1f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-1f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-5f, WATER_HEIGHT, GetBackgroundZ() + 29f), new MoveVector(0f, -130f));
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-1.2f, WATER_HEIGHT, GetBackgroundZ() + 28.2f), new MoveVector(0f, -120f));
        yield return new WaitForSeconds(5f);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_LEFT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, 75f), new MovePattern[] {new MovePattern(7f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(4f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForSeconds(8f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(3f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(1f);

        CreateEnemyWithMoveVector(m_ShipCarrier_2, new Vector3(Size.GAME_BOUNDARY_RIGHT, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(0.7f, -75f), new MovePattern[] {new MovePattern(7f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(6f);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-Random.Range(5.5f, 6.5f), WATER_HEIGHT, GetBackgroundZ() + 28f),
        new MoveVector(2f, 0f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-1.5f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnSmallShips());
        yield return new WaitForSeconds(1f);
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(-3.5f, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(3.5f, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_ShipMedium, new Vector3(0f, WATER_HEIGHT, GetBackgroundZ() + 31f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(4f, 8739f, 0f, 1f)});
        SetBackgroundSpeed(1.2f, 1f);
        yield return new WaitForSeconds(8f); // Final Field (287s)
        
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnHelicopters_C(4f, 1.3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        yield return new WaitForSeconds(5f);
        CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(-5f, 2f, GetBackgroundZ() + 30f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2f, 8739f, -1f, 2f), new MovePattern(4f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2.5f);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(0f, 2f, GetBackgroundZ() + 30f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2f, 8739f, -1f, 2f), new MovePattern(4f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2.5f);
        CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(5f, 2f, GetBackgroundZ() + 30f),
        new MoveVector(1f, 0f), new MovePattern[] {new MovePattern(2f, 8739f, -1f, 2f), new MovePattern(4f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(11f);
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        yield return new WaitForSeconds(7f); // 319s

        SetBackgroundSpeed(7f, 2f);
        yield return new WaitForSeconds(2f);
        period = new float[] {5.5f, 2f, 1.5f};
        StartCoroutine(SpawnPlaneSmalls_A(m_PlaneSmall_1, 10f, period[m_SystemManager.m_Difficulty]));
        yield return new WaitForSeconds(11f);
        SetBackgroundSpeed(1f, 4f);
        yield return new WaitForSeconds(6f);
        CreateEnemy(m_ItemHeli_1, new Vector2(3f, 3f));
        CreateEnemy(m_ItemHeli_2, new Vector2(-3f, 3f));
        yield break;
    }
    

    private IEnumerator SpawnSmallTanks()
    {
        for (int i = 0; i < 6; i++) {
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(-2.5f, -1f), 2f, GetBackgroundZ() + 28f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(1f, 2.5f), 2f, GetBackgroundZ() + 28f), new MoveVector(1f, 0f));
            yield return new WaitForSeconds(1.4f);
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(-2.5f, -1f), 2f, GetBackgroundZ() + 28f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(1f, 2.5f), 2f, GetBackgroundZ() + 28f), new MoveVector(1f, 0f));
            yield return new WaitForSeconds(1.4f);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters_A(float pos)
    {
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 2f, 3f), new Vector2(pos - 2f, -6f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -6f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 2f, 3f), new Vector2(pos + 2f, -6f), 1.2f);
        yield return new WaitForSeconds(0.4f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 0.75f, 3f), new Vector2(pos - 0.75f, -4f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 0.75f, 3f), new Vector2(pos + 0.75f, -4f), 1.2f);
        yield return new WaitForSeconds(0.4f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos - 2f, 3f), new Vector2(pos - 2f, -2f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -2f), 1.2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(pos + 2f, 3f), new Vector2(pos + 2f, -2f), 1.2f);
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
            CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -2f - (3-i)*0.2f), 1.1f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -4f - (3-i)*0.2f), 1.1f);
            yield return new WaitForSeconds(0.32f);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters_C(float time, float period)
    {
        float timer = 0f, rand, target_y = -2f;
        while (timer < time) {
            rand = Random.Range(-7f, 7f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(rand, 3f), new Vector2(rand, target_y), Random.Range(1.2f, 1.5f));
            yield return new WaitForSeconds(period);
            target_y -= 1f;
            if (target_y > -5f)
                target_y = -2f;
            timer += period;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_A(GameObject plane, float time, float period) // 3 2 형식
    {
        float timer = 0f;
        while (timer < time) {
            CreateEnemy(plane, new Vector2(Random.Range(-7f, -5f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(5f, 7f), Random.Range(2f, 4f)));
            yield return new WaitForSeconds(period);
            CreateEnemy(plane, new Vector2(Random.Range(-4f, -2f), Random.Range(2f, 4f)));
            CreateEnemy(plane, new Vector2(Random.Range(2f, 4f), Random.Range(2f, 4f)));
            yield return new WaitForSeconds(period);
            timer += period*2f;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_B(GameObject plane, float time, float period) // 아래에서 소환
    {
        float timer = 0f;
        while (timer < time) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(plane, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, Random.Range(-2f, -3f)));
                CreateEnemy(plane, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, Random.Range(-2f, -3f)));
            }
            yield return new WaitForSeconds(period);
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
            yield return new WaitForSeconds(0.32f);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmall_2s(float time, float period)
    {
        float timer = 0f;
        while (timer < time) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-7f, -5f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(5f, 7f), Random.Range(1f, 4f)));
            }
            yield return new WaitForSeconds(period);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-4f, -2f), Random.Range(1f, 4f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(2f, 4f), Random.Range(1f, 4f)));
            }
            yield return new WaitForSeconds(period);
            timer += period*2f;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneMedium_3s(float time)
    {
        float timer = 0f, period = 0.6f, rand;
        while (timer < time) {
            rand = Random.Range(0f, 0.3f);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_3, new Vector2(-Random.Range(1f, 6f), 3f));
            }
            yield return new WaitForSeconds(period - rand);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(1f, 6f), 3f));
            }
            yield return new WaitForSeconds(period + rand);
            timer += period*2f;
        }
        yield break;
    }

    private IEnumerator SpawnSmallShips() {
        GameObject[] ship = {m_ShipSmall_1, m_ShipSmall_2};
        for (int i = 0; i < 2; i++) {
            CreateEnemyWithMoveVector(ship[i], new Vector3(Size.GAME_BOUNDARY_LEFT - 2f, WATER_HEIGHT, GetBackgroundZ() + 26f),
            new MoveVector(1.3f, 50f), new MovePattern[] {new MovePattern(2.7f, 8739f, 0f, 1.2f)});
            CreateEnemyWithMoveVector(ship[i], new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, GetBackgroundZ() + 26f),
            new MoveVector(1.3f, -50f), new MovePattern[] {new MovePattern(2.7f, 8739f, 0f, 1.2f)});
            yield return new WaitForSeconds(4.8f);
        }
        yield break;
    }

    private float GetBackgroundZ() {
        return m_SystemManager.m_BackgroundCamera.transform.position.z;
    }
}
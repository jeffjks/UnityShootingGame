using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage2Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_TankSmall_1, m_TankSmall_2, m_ShipSmall_1, m_ShipSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeli_1, m_ItemHeli_2, m_Gunship, m_PlaneMedium_2, m_PlaneMedium_3;

    private const float WATER_HEIGHT = 2.32f;
    private IEnumerator m_CurrentSpawn;

    void Awake()
    {
        m_Stage = 1;
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        SetBackgroundSpeed(0.96f);

        yield return new WaitForSeconds(35f);
        StartCoroutine(MiddleBossStart(new Vector3(-12.5f, 4.2f, 29f), 1f)); // Middle Boss (36s)

        yield return new WaitForSeconds(35f);
        SetBackgroundSpeed(new Vector3(1.2f, 0f, 0.6f), 0.75f);
        
        yield return new WaitForSeconds(13f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0.96f), 0.75f);

        yield return new WaitForSeconds(27f);
        StartCoroutine(BossStart(new Vector3(16f, WATER_HEIGHT, 115f), 9f)); // Boss
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(4f);
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 32f;
        SetBackgroundSpeed(0f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(BossStart(new Vector3(0f, 5.6f, 27f), 3f)); // Boss
        yield return null;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3.5f, 3f), new Vector2(-3f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3.5f, 3f), new Vector2(3f, -3f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(0.5f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4.5f, 3f), new Vector2(-4.5f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4.5f, 3f), new Vector2(4.5f, -3f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-6.2f, 3f, 11f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-1.2f, 3f, 12f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(2f, 3f, 10f), new MoveVector(1f, 0f), 7f, 1f);
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-6f, 3f), new Vector2(-6f, -5f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(6f, -5f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(-3.5f, 3f, 14f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(1f, 3f, 16f), new MoveVector(1f, 0f), 7f, 1f);
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_1, new Vector2(0f, 3f)); // Item Heli 1
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-5f, 3f, 17f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-3.5f, 3f, 15f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-1f, 3f, 18f), new MoveVector(1f, 0f), 7f, 1f);
        yield return new WaitForSeconds(2f);
        if (m_SystemManager.m_Difficulty >= Difficulty.HELL) {
            CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        }
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(4f, 3f, 21f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(0f, 3f, 23f), new MoveVector(1f, 0f), 7f, 1f);
        yield return new WaitForSeconds(3f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(1.5f, 3f, 23f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(6.5f, 3f, 24f), new MoveVector(1f, 0f), 7f, 1f);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(5f, 3f, 27f), new MoveVector(1f, 0f), 7f, 1f);
        yield return new WaitForSeconds(11f);
        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT) {
            CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        }
        yield return new WaitForSeconds(1.5f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3f, 3f), new Vector2(-3.5f, -3f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4.5f, 3f), new Vector2(-4.5f, -5f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3.5f, 3f), new Vector2(3.5f, -4f), Random.Range(1.2f, 1.5f));

        yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(12f); // Middle Boss ==========================

        sbyte side = 1;
        for (int i = 0; i < 4; i++) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_2, new Vector2(3.5f*side, 3f));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-1f*side, 3f), new Vector2(-1f*side, -3f), Random.Range(1.2f, 1.5f));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f*side, 3f), new Vector2(-4f*side, -3f), Random.Range(1.2f, 1.5f));
            }
            yield return new WaitForSeconds(1.5f);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-1f*side, 3f), new Vector2(-1f*side, -3f), Random.Range(1.2f, 1.5f));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f*side, 3f), new Vector2(-4f*side, -3f), Random.Range(1.2f, 1.5f));
            }
            side *= -1;
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(0.5f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-5.5f, 3f), new Vector2(-5.5f, -5f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3.5f, -6f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(1.5f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-2f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3f, 3f), new Vector2(3f, -7f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(5.5f, -5f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(1.5f);
        CreateEnemy(m_PlaneMedium_3, new Vector2(5f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3f, 3f), new Vector2(-3f, -6f), Random.Range(1.2f, 1.5f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0.5f, -4f), Random.Range(1.2f, 1.5f));
        yield return new WaitForSeconds(2.5f);
        if (m_SystemManager.m_Difficulty <= Difficulty.EXPERT) {
            CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        }
        else {
            CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
            CreateEnemy(m_PlaneMedium_2, new Vector2(4f, 3f));
        }
        yield return new WaitForSeconds(4.5f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -4f), 1f);
        yield return new WaitForSeconds(8f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -4f), 1f);
        yield return new WaitForSeconds(8f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -3f), 1f);
        m_CurrentSpawn = SpawnPlane1();
        StartCoroutine(m_CurrentSpawn);
        yield return new WaitForSeconds(8f);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1f);

        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(12.5f, WATER_HEIGHT, 92.5f), new MoveVector(0f, 45f));
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(19f, WATER_HEIGHT, 94f), new MoveVector(0f, -20f));

        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT) {
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(21.5f, WATER_HEIGHT, 93f), new MoveVector(0f, 100f));
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(15.5f, WATER_HEIGHT, 94f), new MoveVector(0f, -53f));
        }
        if (m_SystemManager.m_Difficulty >= Difficulty.HELL) {
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(22f, WATER_HEIGHT, 96f), new MoveVector(0f, -117f));
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(12f, WATER_HEIGHT, 95.5f), new MoveVector(0f, 100f));
        }
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(2f, 3f));
        StopCoroutine(m_CurrentSpawn);

        yield return new WaitForSeconds(4f);
        CreateEnemy(m_ItemHeli_1, new Vector2(-2f, 3f)); // Item Heli 1
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_2, new Vector2(3.5f, 3f)); // Item Heli 2
        yield return null;
    }

    private IEnumerator SpawnPlane1()
    {
        while (true) {
            for (int i = 0; i < 2; i++) {
                if (m_SystemManager.m_Difficulty == Difficulty.NORMAL) {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-2f, -1f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 2f), Random.Range(2f, 4f)));
                }
                else {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-3f, -2f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 3f), Random.Range(2f, 4f)));
                }
                yield return new WaitForSeconds(0.6f);
            }
            yield return new WaitForSeconds(5f);
        }
    }
}
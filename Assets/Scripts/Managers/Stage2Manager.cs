using UnityEngine;
using System.Collections;

public class Stage2Manager : StageManager
{
    [Space(10)]
    public GameObject m_TankSmall_1, m_TankSmall_2, m_ShipSmall_1, m_ShipSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeliRed, m_ItemHeliGreen, m_Gunship, m_PlaneMedium_2, m_PlaneMedium_3;

    private const float WATER_HEIGHT = 2.32f;
    private IEnumerator m_CurrentSpawn;

    protected override void Init()
    {
        m_SystemManager.SetCurrentStage(1);
    }

    protected override IEnumerator MainTimeline()
    {
        InitEnemies();
        SetBackgroundSpeed(0.96f);

        yield return new WaitForMillisecondFrames(35000);
        StartCoroutine(MiddleBossStart(new Vector3(-12.5f, 4.23f, 29f), 1000)); // Middle Boss (36s)

        yield return new WaitForMillisecondFrames(35000);
        SetBackgroundSpeed(new Vector3(1.2f, 0f, 0.6f), 750);
        
        yield return new WaitForMillisecondFrames(13320);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0.96f), 750);

        yield return new WaitForMillisecondFrames(26000);
        StartBossTimeline();
        yield break;
    }

    protected override IEnumerator TestTimeline()
    {
        yield break;
    }

    protected override IEnumerator BossTimeline()
    {
        StartCoroutine(BossStart(new Vector3(16f, WATER_HEIGHT, 116f), 10000)); // Boss
        yield return new WaitForMillisecondFrames(4000);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForMillisecondFrames(3000);
        m_SystemManager.WarningText();
        yield return new WaitForMillisecondFrames(4000);
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 32f;
        SetBackgroundSpeed(0f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator EnemyTimeline()
    {
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3.5f, 3f), new Vector2(-3f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3.5f, 3f), new Vector2(3f, -3f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4.5f, 3f), new Vector2(-4.5f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4.5f, 3f), new Vector2(4.5f, -3f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(1000);
        MovePattern[] movePatterns7000 = { new MovePattern(7000, 8739f, 0f, 1000) };
        MovePattern[] movePatterns6000 = { new MovePattern(7000, 8739f, 0f, 1000) };
        MovePattern[] movePatterns5000 = { new MovePattern(7000, 8739f, 0f, 1000) };
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-6.2f, 3f, 11f), new MoveVector(1f, 0f), movePatterns7000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-1.2f, 3f, 12f), new MoveVector(1f, 0f), movePatterns7000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(2f, 3f, 10f), new MoveVector(1f, 0f), movePatterns7000);
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-6f, 3f), new Vector2(-6f, -5f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(6f, -5f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(-3.5f, 3f, 14f), new MoveVector(1f, 0f), movePatterns7000);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(1f, 3f, 16f), new MoveVector(1f, 0f), movePatterns7000);
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_ItemHeliRed, new Vector2(0f, 3f)); // Item Heli 1
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-5f, 3f, 17f), new MoveVector(1f, 0f), movePatterns5000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-3.5f, 3f, 15f), new MoveVector(1f, 0f), movePatterns5000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(-1f, 3f, 18f), new MoveVector(1f, 0f), movePatterns5000);
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(3.5f, 3f, 21f), new MoveVector(1f, 0f), movePatterns7000);
        CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(-1f, 3f, 23f), new MoveVector(1f, 0f), movePatterns7000);
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(1.5f, 3f, 23f), new MoveVector(1f, 0f), movePatterns7000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(6.2f, 3f, 24f), new MoveVector(1f, 0f), movePatterns6000);
        CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(4.8f, 3f, 27f), new MoveVector(1f, 0f), movePatterns7000);
        if (m_SystemManager.GetDifficulty() >= Difficulty.EXPERT) {
            StartCoroutine(SpawnHelicopters(true));
        }
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(2f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -5f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -5f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-3f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        if (m_SystemManager.GetDifficulty() >= Difficulty.HELL) {
            CreateEnemy(m_PlaneMedium_3, new Vector2(3f, 3f));
        }
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
        yield return new WaitForMillisecondFrames(1000);
        if (m_SystemManager.GetDifficulty() >= Difficulty.HELL) {
            StartCoroutine(SpawnHelicopters(false));
        }
        yield return new WaitForMillisecondFrames(14000); // Middle Boss ==========================

        sbyte side = 1;
        for (int i = 0; i < 4; i++) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneMedium_2, new Vector2(3.5f*side, 3f));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-1f*side, 3f), new Vector2(-1f*side, -3f), Random.Range(1200, 1500));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f*side, 3f), new Vector2(-4f*side, -3f), Random.Range(1200, 1500));
            }
            yield return new WaitForMillisecondFrames(1500);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-1f*side, 3f), new Vector2(-1f*side, -3f), Random.Range(1200, 1500));
                CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f*side, 3f), new Vector2(-4f*side, -3f), Random.Range(1200, 1500));
            }
            side *= -1;
            yield return new WaitForMillisecondFrames(1500);
        }
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(0.5f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-5.5f, 3f), new Vector2(-5.5f, -5f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3.5f, -6f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-2f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(3f, 3f), new Vector2(3f, -7f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(5.5f, -5f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemy(m_PlaneMedium_3, new Vector2(5f, 3f));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-3f, 3f), new Vector2(-3f, -6f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0.5f, -4f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(2500);
        if (m_SystemManager.GetDifficulty() <= Difficulty.EXPERT) {
            CreateEnemy(m_PlaneMedium_2, new Vector2(0f, 3f));
        }
        else {
            CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
            CreateEnemy(m_PlaneMedium_2, new Vector2(4f, 3f));
        }
        yield return new WaitForMillisecondFrames(4500);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -4f), 1000);
        yield return new WaitForMillisecondFrames(8000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -4f), 1000);
        yield return new WaitForMillisecondFrames(8000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -3f), 1000);
        m_CurrentSpawn = SpawnPlane1();
        StartCoroutine(m_CurrentSpawn);
        yield return new WaitForMillisecondFrames(8000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1000);

        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(12.5f, WATER_HEIGHT, 92.5f), new MoveVector(0f, 45f));
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(19f, WATER_HEIGHT, 94f), new MoveVector(0f, -20f));

        if (m_SystemManager.GetDifficulty() >= Difficulty.EXPERT) {
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(21.5f, WATER_HEIGHT, 93f), new MoveVector(0f, 100f));
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(15.5f, WATER_HEIGHT, 94f), new MoveVector(0f, -53f));
        }
        if (m_SystemManager.GetDifficulty() >= Difficulty.HELL) {
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(22f, WATER_HEIGHT, 96f), new MoveVector(0f, -117f));
            CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(12f, WATER_HEIGHT, 95.5f), new MoveVector(0f, 100f));
        }
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_2, new Vector2(2f, 3f));
        StopCoroutine(m_CurrentSpawn);

        yield return new WaitForMillisecondFrames(4000);
        CreateEnemy(m_ItemHeliRed, new Vector2(-2f, 3f)); // Item Heli 1
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_ItemHeliGreen, new Vector2(3.5f, 3f)); // Item Heli 2
        yield break;
    }

    private IEnumerator SpawnPlane1()
    {
        while (true) {
            for (int i = 0; i < 2; i++) {
                if (m_SystemManager.GetDifficulty() == Difficulty.NORMAL) {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-2f, -1f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 2f), Random.Range(2f, 4f)));
                }
                else {
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-3f, -2f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(2f, 4f)));
                    CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 3f), Random.Range(2f, 4f)));
                }
                yield return new WaitForMillisecondFrames(600);
            }
            yield return new WaitForMillisecondFrames(5000);
        }
    }

    private IEnumerator SpawnHelicopters(bool right_side)
    {
        float pos;
        for (int i = 0; i < 4; i++) {
            if (right_side)
                pos = 6f - i*1.8f;
            else
                pos = -6f + i*1.8f;
            if (m_SystemManager.GetDifficulty() == Difficulty.NORMAL) {
                CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -3f - (3-i)*0.2f), 1100);
            }
            else {
                CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -2f - (3-i)*0.2f), 1100);
                CreateEnemyWithTarget(m_Helicopter, new Vector2(pos, 3f), new Vector2(pos, -4f - (3-i)*0.2f), 1100);
            }
            yield return new WaitForMillisecondFrames(320);
        }
        yield break;
    }
}
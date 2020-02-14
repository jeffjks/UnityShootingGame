using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage1Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_TankSmall_1, m_TankSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeli_1, m_ShipSmall_1, m_PlaneMedium_1, m_PlaneMedium_3;

    private const float WATER_HEIGHT = 2.32f;

    void Awake()
    {
        m_Stage = 0;
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        SetBackgroundSpeed(18f);
        yield return new WaitForSeconds(5f);
        SetBackgroundSpeed(1.8f, 1.6875f);

        yield return new WaitForSeconds(36f);
        SetBackgroundSpeed(2.7f, 0.75f);
        StartCoroutine(MiddleBossStart(new Vector3(12f, -13f, Depth.ENEMY), 1f)); // Middle Boss (42s)

        yield return new WaitForSeconds(55f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(4f);
        SetBackgroundSpeed(7.2f, 0.9375f);
        PlayBossMusic();
        StartCoroutine(BossStart(new Vector3(0f, 4.5f, Depth.ENEMY), 1f));
        yield return new WaitForSeconds(2f);
        SetBackgroundSpeed(0f);
        UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 240f;
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(4f);
        SetBackgroundSpeed(7.2f, 0.9375f);
        PlayBossMusic();
        StartCoroutine(BossStart(new Vector3(0f, 4.5f, Depth.ENEMY), 1f));
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        float random_value;
        yield return new WaitForSeconds(9f);
        random_value = Random.Range(1.2f, 1.5f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3f, -3f), random_value);
        yield return new WaitForSeconds(2f);
        if (m_SystemManager.m_Difficulty >= 1) {
            random_value = Random.Range(1.2f, 1.5f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(1f, -3f), random_value);
        }
        random_value = Random.Range(1.2f, 1.5f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(5f, 3f), new Vector2(4f, -4f), random_value);
        yield return new WaitForSeconds(2f);
        random_value = Random.Range(1.2f, 1.5f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), random_value);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(1f, -4f), random_value);
        yield return new WaitForSeconds(1f);
        random_value = Random.Range(1.2f, 1.5f);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(5f, -5f), random_value);
        yield return new WaitForSeconds(0.5f);
        if (m_SystemManager.m_Difficulty >= Difficulty.EXPERT) {
            random_value = Random.Range(1.2f, 1.5f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(3f, 3f), new Vector2(2f, -1.5f), random_value);
            random_value = Random.Range(1.2f, 1.5f);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(6f, -3f), random_value);
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
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-10.055f, WATER_HEIGHT, 132.5f), new MoveVector(4f, 70f), new MovePattern[] {new MovePattern(0f, 8739f, 0f, 3.2f)});
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-11.06f, WATER_HEIGHT, 135.44f), new MoveVector(4f, 70f), new MovePattern[] {new MovePattern(0f, 8739f, 0f, 3.2f)});
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-13.98f, WATER_HEIGHT, 133.93f), new MoveVector(4f, 70f), new MovePattern[] {new MovePattern(0f, 8739f, 0f, 3.2f)});
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
        yield return new WaitForSeconds(14f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(3f, 3f));
        yield break;
    }
}
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage3Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_ShipSmall_1, m_ShipSmall_2, m_ShipMedium, m_ShipLarge, m_ShipCarrier_1, m_TankLarge_3,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_ItemHeli_1, m_ItemHeli_2, m_PlaneMedium_5;

    private const float WATER_HEIGHT = 2.32f;

    void Awake()
    {
        m_Stage = 2;
    }

    protected override void Update()
    {
        base.Update();

        BackgroundLoop(98f, 24f);
    }

    protected override IEnumerator MainTimeLine()
    {
        yield return new WaitForSeconds(1f);
        InitEnemies();
        SetBackgroundSpeed(0.96f);

        yield return new WaitForSeconds(55f);
        SetBackgroundSpeed(-0.96f, 2f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, -1f, 52f), 3f)); // Middle Boss (56s)

        yield return new WaitForSeconds(26f);
        SetBackgroundSpeed(new Vector3(-1.92f, 0f, -0.72f), 0.75f);
        
        yield return new WaitForSeconds(13f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0.96f), 0.75f);

        yield return new WaitForSeconds(38f);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(3f);
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(1f);
        StartCoroutine(BossStart(new Vector3(9.5f, -12.5f, Depth.ENEMY), 3f)); // Boss
        yield return new WaitForSeconds(2f);
        SetBackgroundSpeed(new Vector3(0f, 0f, 3.84f), 1f);
        PlayBossMusic();
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 2.5f, Depth.ENEMY), 1f)); // Boss
        yield break;
    }

/*
    protected override IEnumerator TestTimeLine()
    {
        StartCoroutine(FadeOutMusic());
        m_SystemManager.StartCoroutine("WarningText");
        yield return new WaitForSeconds(2f);
        StartCoroutine(BossStart(new Vector3(9.5f, -12.5f, Depth.ENEMY), 3f)); // Boss
        StartCoroutine(BossStart(new Vector3(0f, 5.6f, 3.6f), 3f)); // Boss
        yield return new WaitForSeconds(2f);
        PlayBossMusic();
        yield break;
    }*/

    protected override IEnumerator EnemyTimeLine()
    {
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 5f), Random.Range(2f, 4f)));
        yield return new WaitForSeconds(0.7f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-6f, -2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 6f), Random.Range(2f, 4f)));
        yield return new WaitForSeconds(0.7f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-6f, -2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 6f), Random.Range(2f, 4f)));
        yield return new WaitForSeconds(0.6f);
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -2f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 5f), Random.Range(2f, 4f)));
        CreateEnemy(m_ItemHeli_1, new Vector2(2f, 3f)); // Item Heli 1
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-6f, -3f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-3f, 0f), Random.Range(2f, 4f)));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-6f, -3f), Random.Range(2f, 4f)));
        CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-3f, 0f), Random.Range(2f, 4f)));
        yield return new WaitForSeconds(1f);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_LEFT - 3f, WATER_HEIGHT, 13f), new MoveVector(1.5f, 80f), new MovePattern[] {new MovePattern(4f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(5f);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(1f, WATER_HEIGHT, 20f), new MoveVector(1.5f, 5f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(3.2f, WATER_HEIGHT, 22f), new MoveVector(1.5f, -2f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(5.6f, WATER_HEIGHT, 21f), new MoveVector(1.5f, 1f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(4f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(4.5f, -5.5f), 1f);
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(0f, WATER_HEIGHT, 25f), new MoveVector(1.5f, 0f), new MovePattern[] {new MovePattern(3.7f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-4.5f, WATER_HEIGHT, 27f), new MoveVector(1.5f, -3f), new MovePattern[] {new MovePattern(3.7f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-6.5f, WATER_HEIGHT, 26f), new MoveVector(1.5f, 2f), new MovePattern[] {new MovePattern(3.7f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-2.8f, WATER_HEIGHT, 26.5f), new MoveVector(1.5f, 0f), new MovePattern[] {new MovePattern(3.7f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 3f, WATER_HEIGHT, 26f), new MoveVector(1.5f, -50f), new MovePattern[] {new MovePattern(5f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 4f, WATER_HEIGHT, 24f), new MoveVector(1.5f, -46f), new MovePattern[] {new MovePattern(5f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, 21f), new MoveVector(1.5f, -40f), new MovePattern[] {new MovePattern(5f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-4.5f, -5.5f), 1f);
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnPlane2());
        yield return new WaitForSeconds(6f);
        CreateEnemyWithMoveVector(m_ShipCarrier_1, new Vector3(Size.GAME_BOUNDARY_LEFT + 1f, WATER_HEIGHT, 34.8f), new MoveVector(0.6f, 70f), new MovePattern[] {new MovePattern(7f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(12f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(3f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(5f, -3.5f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(3.4f, -5f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(5.2f, -6.5f), 1f);
        yield return new WaitForSeconds(3f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-3f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-5f, -3.5f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-3.4f, -5f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-5.2f, -6.5f), 1f);
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_RIGHT + 3f, WATER_HEIGHT, 47f), new MoveVector(1.4f, -80f));
        yield return new WaitForSeconds(5f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-1.25f, -3f), 1.5f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(1.25f, -3f), 1.5f);
        yield return new WaitForSeconds(1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5.5f), new Vector2(-3f, -6f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5.5f), new Vector2(3f, -6f), 1f);
        yield return new WaitForSeconds(4f);
        // Middle Boss (56s)
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < 33; i++) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-8f, -6f), Random.Range(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-5f, -2.5f), Random.Range(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(2.5f, 5f), Random.Range(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(6f, 8f), Random.Range(2f, 3f)));
            }
            yield return new WaitForSeconds(0.6f);
        }
        yield return new WaitForSeconds(9.2f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -7f), new Vector2(4f, -7f), 1f);
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_ItemHeli_1, new Vector2(1f, 3f)); // Item Heli 1
        yield return new WaitForSeconds(2f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(2f, -4f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -6f), new Vector2(5f, -6f), 1f);
        yield return new WaitForSeconds(3f);
        CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(-36.5f, 3f, 26f), new MoveVector(4f, 85f), new MovePattern[] {new MovePattern(1.2f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(10f);
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
        yield return new WaitForSeconds(12f);
        CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_ItemHeli_1, new Vector2(3f, 3f)); // Item Heli 1
        CreateEnemy(m_ItemHeli_2, new Vector2(-3f, 3f)); // Item Heli 2
        yield break;
    }

    private IEnumerator SpawnPlane2()
    {
        for (int i = 0; i < 10; i++) {
            CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-7f, -4f), Random.Range(2f, 4f)));
            CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f)));
            CreateEnemy(m_PlaneSmall_2, new Vector2(Random.Range(4f, 7f), Random.Range(2f, 4f)));
            yield return new WaitForSeconds(0.6f);
        }
    }
}
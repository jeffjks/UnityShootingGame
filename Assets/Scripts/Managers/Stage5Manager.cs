using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Stage5Manager : StageManager
{
    [Space(10)]
    public GameObject m_Test;
    public GameObject m_Helicopter, m_ItemHeli_1, m_ItemHeli_2,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_TankSmall_1, m_TankSmall_2, m_TankSmall_3,
    m_PlaneMedium_1, m_PlaneMedium_2, m_PlaneMedium_3, m_PlaneMedium_4, m_TankMedium_1, m_TankMedium_2, m_TankMedium_3, m_Gunship,
    m_PlaneLarge_1, m_PlaneLarge_2, m_PlaneLarge_3, m_TankLarge_1, m_TankLarge_2;

    private const float WATER_HEIGHT = 1.32f;

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
        yield break;
    }

    protected override IEnumerator TestTimeLine()
    {
        SetBackgroundSpeed(1f);
        yield return new WaitForSeconds(2f);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 5f, Depth.ENEMY), 0.5f, 0)); // Middle Boss
        yield break;
    }

    protected override IEnumerator BossOnlyTimeLine()
    {
        yield break;
    }

    protected override IEnumerator EnemyTimeLine()
    {
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnSmallTanks());
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_ItemHeli_1, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 9f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 9f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(1f);
        StartCoroutine(SpawnHelicopters_1(-3f));
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_1, new Vector2(2.5f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnHelicopters_1(3f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(-10f, 2f, 19f), new MoveVector(3f, 90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankSmall_3, new Vector3(10f, 2f, 19f), new MoveVector(3f, -90f), new MovePattern[] {new MovePattern(1f, 8739f, 0f, 1f)});
        CreateEnemy(m_PlaneMedium_1, new Vector2(-2.5f, 3f));
        yield return new WaitForSeconds(5f);
        StartCoroutine(SpawnHelicopters_1(0f));
        yield return new WaitForSeconds(2f);
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(-10f, 2f, 26f), new MoveVector(2f, 65f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        CreateEnemyWithMoveVector(m_TankMedium_3, new Vector3(10f, 2f, 26f), new MoveVector(2f, -65f), new MovePattern[] {new MovePattern(3f, 8739f, 0f, 1f)});
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnPlaneSmall_1s_A(4f, 0.5f));
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
        yield return new WaitForSeconds(10f);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-3f, 3f));
        yield return new WaitForSeconds(4f);
        StartCoroutine(SpawnPlaneSmall_2s(1f, 0.8f));
        yield return new WaitForSeconds(8f);
        StartCoroutine(SpawnHelicopters_2(false));
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
        yield return new WaitForSeconds(30.5f);

        StartCoroutine(SpawnPlaneSmall_2s(15f, 0.25f));
        StartCoroutine(SpawnPlaneSmall_1s_B(15f));
        yield return new WaitForSeconds(16f);
        CreateEnemy(m_ItemHeli_1, new Vector2(2f, 3f));
        yield return new WaitForSeconds(1f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        yield return new WaitForSeconds(5f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-4f, 3f));
        CreateEnemy(m_PlaneMedium_2, new Vector2(3f, 3f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnHelicopters_2(true));
        yield return new WaitForSeconds(3f);
        StartCoroutine(SpawnHelicopters_2(false));
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(-12.1f, 3.51f, 167f), new MoveVector(1.2f, 45f), new MovePattern[] {new MovePattern(8f, 8739f, 0f, 2f)});
        yield return new WaitForSeconds(11f);
        CreateEnemyWithMoveVector(m_TankLarge_1, new Vector3(12.1f, 3.51f, 177.6f), new MoveVector(1.2f, -45f), new MovePattern[] {new MovePattern(10f, 8739f, 0f, 2f)});
        yield return new WaitForSeconds(2f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(2f, 3f));
        yield return new WaitForSeconds(3f);
        CreateEnemy(m_PlaneMedium_2, new Vector2(-5f, 3f));
        yield return new WaitForSeconds(12f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -2f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-2.5f, -4f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1f);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(2.5f, -5f), 1f);
        yield break;
    }
    

    private IEnumerator SpawnSmallTanks()
    {
        for (int i = 0; i < 6; i++) {
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(-2.5f, -1f), 2f, m_SystemManager.m_BackgroundCamera.transform.position.z + 28f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_1, new Vector3(Random.Range(1f, 2.5f), 2f, m_SystemManager.m_BackgroundCamera.transform.position.z + 28f), new MoveVector(1f, 0f));
            yield return new WaitForSeconds(1.4f);
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(-2.5f, -1f), 2f, m_SystemManager.m_BackgroundCamera.transform.position.z + 28f), new MoveVector(1f, 0f));
            CreateEnemyWithMoveVector(m_TankSmall_2, new Vector3(Random.Range(1f, 2.5f), 2f, m_SystemManager.m_BackgroundCamera.transform.position.z + 28f), new MoveVector(1f, 0f));
            yield return new WaitForSeconds(1.4f);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters_1(float pos)
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

    private IEnumerator SpawnHelicopters_2(bool right_side)
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

    private IEnumerator SpawnPlaneSmall_1s_A(float time, float period)
    {
        float timer = 0f;
        while (timer < time) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-7f, -5f), Random.Range(2f, 4f)));
            CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 4f)));
            CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(5f, 7f), Random.Range(2f, 4f)));
            yield return new WaitForSeconds(period);
            CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-4f, -2f), Random.Range(2f, 4f)));
            CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(2f, 4f), Random.Range(2f, 4f)));
            yield return new WaitForSeconds(period);
            timer += period*2f;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmall_1s_B(float time)
    {
        float timer = 0f;
        float period = 0.6f;
        while (timer < time) {
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, Random.Range(-4f, -6f)));
                CreateEnemy(m_PlaneSmall_1, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, Random.Range(-4f, -6f)));
            }
            yield return new WaitForSeconds(period);
            timer += period;
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
}
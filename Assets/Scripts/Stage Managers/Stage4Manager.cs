using UnityEngine;
using System.Collections;

public class Stage4Manager : StageManager
{
    [Space(10)]
    public GameObject m_Helicopter, m_TankLarge_2, m_Gunship,
    m_PlaneSmall_1, m_ItemHeliRed, m_ItemHeliGreen, m_PlaneMedium_3, m_PlaneMedium_4, m_PlaneLarge_2, m_PlaneLarge_3;
    public Transform[] m_BossTerrains = new Transform[3];

    private float m_BackgroundPos;
    private GameObject m_TankLarge_2_Move;
    private float m_BossBackgroundSpeed;

    protected override void Init()
    {
        m_SystemManager.SetCurrentStage(3);
        m_BossBackgroundSpeed = 3.12f / Application.targetFrameRate * Time.timeScale;
        
        AudioService.LoadMusics("Stage4");
        AudioService.PlayMusic("Stage4");
    }

    protected override IEnumerator MainTimeline()
    {
        InitEnemies();
        SetBackgroundSpeed(1.12f);

        yield return new WaitForMillisecondFrames(74000);
        StartCoroutine(MiddleBossStart(new Vector3(0f, 2.5f, Depth.ENEMY), 1000)); // Middle Boss

        yield return new WaitForMillisecondFrames(22000);
        SetBackgroundSpeed(0f, 1200);
        yield return new WaitForMillisecondFrames(1200);

        while(m_BackgroundPos < 270f) {
            float c_horizontal = 0.9783f, c_vertical = 1f;
            SetBackgroundSpeed(new Vector3(c_horizontal*Mathf.Cos(Mathf.Deg2Rad * (180f + m_BackgroundPos)), 0f, c_vertical*Mathf.Sin(Mathf.Deg2Rad * (180f + m_BackgroundPos))));
            m_BackgroundPos += 4f / Application.targetFrameRate * Time.timeScale;
            yield return new WaitForFrames(0);
        }
        SetBackgroundSpeed(new Vector3(0f, 0f, 1f));
        yield return new WaitForMillisecondFrames(14000);
        StartBossTimeline();
    }

    protected override IEnumerator TestTimeline()
    {
        yield break;
    }

    protected override IEnumerator BossTimeline()
    {
        yield return new WaitForMillisecondFrames(2000);
        AudioService.FadeOutMusic();
        StartCoroutine(BossStart(new Vector3(14f, 3f, 121.5f), 7500)); // Boss
        SetBackgroundSpeed(new Vector3(0f, 0f, 1.5f), 1000);
        yield return new WaitForMillisecondFrames(3000);
        m_SystemManager.WarningText();
        yield return new WaitForMillisecondFrames(4000);
        AudioService.PlayMusic("Boss1");
        yield return new WaitForMillisecondFrames(64000);
        SetBackgroundSpeed(new Vector3(0f, 0f, 0f), 2000);
    }

    protected override IEnumerator EnemyTimeline()
    {
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_ItemHeliRed, new Vector2(0f, 3f)); // Item Heli 1
        StartCoroutine(SpawnHelicopters1());
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-1f, 3f));
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(2f, 3f));
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-3f, -4f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(Random.Range(-1f, 1f), -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(Random.Range(-1f, 1f), -5f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(3f, -4f), 1200);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-7f, 3f), new Vector2(Random.Range(-6f, 5f), -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(7f, 3f), new Vector2(Random.Range(5f, 6f), -5f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(0f, 3f));
        yield return new WaitForMillisecondFrames(7000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(3f, 3f));
        StartCoroutine(SpawnPlaneSmalls1());
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4f, 3f));
        yield return new WaitForMillisecondFrames(4000);
        StartCoroutine(SpawnPlaneSmalls2());
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(2.61f, 3.18f, 56.17f), new MoveVector(0f, -32f),
            new MovePattern[] {new MovePattern(2000, 0, true, 0.7f), new MovePattern(2000, 720, true, 0f)});
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4f, -4f), 1200);
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -1f), 3f));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(1f, 4f), 3f));
        yield return new WaitForMillisecondFrames(2000);
        int rand = 1 - 2*Random.Range(0, 2); // -1 or 1
        if (m_SystemManager.GetDifficulty() != Difficulty.NORMAL) {
            CreateEnemy(m_PlaneMedium_4, new Vector2(4.5f * rand, 3f));
        }
        CreateEnemy(m_PlaneMedium_4, new Vector2(-4.5f * rand, 3f));
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(SpawnHelicopters2());
        yield return new WaitForMillisecondFrames(9000);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-2f, 3f));
        yield return new WaitForMillisecondFrames(31000);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(-2f, 3f));
        }
        yield return new WaitForMillisecondFrames(4000);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(3f, 3f));
        }
        yield return new WaitForMillisecondFrames(6000);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(-1f, 3f));
        }
        for (int i = 0; i < 6; i++) {
            yield return new WaitForMillisecondFrames(500);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            }
        }
        yield return new WaitForMillisecondFrames(4000);
        if (m_SystemManager.m_PlayState == 0) {
            CreateEnemy(m_PlaneLarge_3, new Vector2(1f, 3f));
        }
        for (int i = 0; i < 6; i++) {
            yield return new WaitForMillisecondFrames(500);
            if (m_SystemManager.m_PlayState == 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            }
        }
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(-24.35f, 3.21f, 84.5f), new MoveVector(-4f, -90f), new MovePattern[] {new MovePattern(2100, 1400, true, 0f)});
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemy(m_ItemHeliRed, new Vector2(2f, 3f)); // Item Heli 1
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-5f, 3f), new Vector2(-5f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4.5f, 3f), new Vector2(-4f, -6f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(0f, 3f), new Vector2(0f, -6f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_ItemHeliGreen, new Vector2(-3f, 3f)); // Item Heli 2
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4.5f, 3f), new Vector2(4.5f, -6f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(1f, 3f), new Vector2(1f, -3f), Random.Range(1200, 1500));
        CreateEnemyWithTarget(m_Helicopter, new Vector2(1f, 3f), new Vector2(1f, -6f), Random.Range(1200, 1500));
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneLarge_2, new Vector2(2.5f, 5f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -3f), 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(-4f, -3f), 3f));
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemy(m_PlaneLarge_3, new Vector2(-3f, 3f));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 4f), 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(Random.Range(3f, 4f), 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneLarge_2, new Vector2(-2f, 5f));
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemy(m_PlaneMedium_4, new Vector2(3f, 3f));
        yield return new WaitForMillisecondFrames(7000);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-4.5f, -4.5f), 1200);
        CreateEnemyWithTarget(m_Gunship, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4.5f, -4.5f), 1200);
        yield return new WaitForMillisecondFrames(9000);
        m_TankLarge_2_Move = CreateEnemyWithMoveVector(m_TankLarge_2, new Vector3(12.7f, 3.03f, 106f), new MoveVector(2.5f, 14.785f),
            new MovePattern[] {new MovePattern(1000, 1000, true, 0f), new MovePattern(2000, 1000, true, -0.8f), new MovePattern(6000, 500, true, 0f)});
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemy(m_ItemHeliRed, new Vector2(3f, 3f)); // Item Heli 1
        yield break;
    }

    private IEnumerator SpawnHelicopters1()
    {
        for (int i = 0; i < 8; i++) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(-6f, -2f), 3f), new Vector2(Random.Range(-6f, -2f), Random.Range(-2f, -7f)), Random.Range(1200, 1500));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(2f, 6f), 3f), new Vector2(Random.Range(2f, 6f), Random.Range(-2f, -7f)), Random.Range(1200, 1500));
            yield return new WaitForMillisecondFrames(500);
        }
        yield break;
    }

    private IEnumerator SpawnHelicopters2()
    {
        for (int i = 0; i < 8; i++) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(-3f, -0.5f), 3f), new Vector2(Random.Range(-3f, -0.5f), Random.Range(-2f, -8f)), Random.Range(1200, 1500));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(Random.Range(0.5f, 3f), 3f), new Vector2(Random.Range(0.5f, 3f), Random.Range(-2f, -8f)), Random.Range(1200, 1500));
            yield return new WaitForMillisecondFrames(500);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls1()
    {
        for (int i = 0; i < 10; i++) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-5f, 3f));
            yield return new WaitForMillisecondFrames(200);
            CreateEnemy(m_PlaneSmall_1, new Vector2(5f, 3f));
            yield return new WaitForMillisecondFrames(200);
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls2()
    {
        for (int i = 0; i < 4; i++) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(2.25f - 1.5f*i, 3f));
            yield return new WaitForMillisecondFrames(400);
        }
        yield break;
    }
}
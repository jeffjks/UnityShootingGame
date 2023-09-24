using UnityEngine;
using System.Collections;

public class Stage3Manager : StageManager
{
    [Space(10)]
    public GameObject m_ShipSmall_1, m_ShipSmall_2, m_ShipMedium, m_ShipLarge, m_ShipCarrier_1, m_TankLarge_3,
    m_PlaneSmall_1, m_PlaneSmall_2, m_PlaneSmall_3, m_ItemHeliRed, m_ItemHeliGreen, m_PlaneMedium_5;

    private const float WATER_HEIGHT = 2.32f;

    protected override void Init()
    {
        SystemManager.Stage = 2;
        
        AudioService.LoadMusics("Stage3");
        AudioService.PlayMusic("Stage3");
    }

    private void Update()
    {
        BackgroundLoop(98f, 24f);
    }

    protected override IEnumerator MainTimeline()
    {
        InitEnemies();
        BackgroundCamera.SetBackgroundCameraSpeed(0.96f);

        yield return new WaitForMillisecondFrames(55000);
        BackgroundCamera.SetBackgroundCameraSpeed(-0.96f, 2000);
        StartCoroutine(MiddleBossStart(new Vector3(0f, -1f, 52f), 3000)); // Middle Boss (56s)

        yield return new WaitForMillisecondFrames(26000);
        BackgroundCamera.SetBackgroundCameraSpeed(new Vector3(-1.92f, 0f, -0.72f), 750);
        
        yield return new WaitForMillisecondFrames(13000);
        BackgroundCamera.SetBackgroundCameraSpeed(new Vector3(0f, 0f, 0.96f), 750);

        yield return new WaitForMillisecondFrames(35000);
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
        yield return new WaitForMillisecondFrames(3000);
        ShowBossWarningSign();
        yield return new WaitForMillisecondFrames(1000);
        StartCoroutine(BossStart(new Vector3(9.5f, -12.5f, Depth.ENEMY), 3000)); // Boss
        yield return new WaitForMillisecondFrames(2000);
        BackgroundCamera.SetBackgroundCameraSpeed(new Vector3(0f, 0f, 3.84f), 1000);
        AudioService.PlayMusic("Boss1");
    }

    protected override IEnumerator EnemyTimeline()
    {
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-5f, -2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(2f, 5f), GameManager.RandomTest(2f, 4f)));
        yield return new WaitForMillisecondFrames(700);
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-6f, -2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-2f, 2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(2f, 6f), GameManager.RandomTest(2f, 4f)));
        yield return new WaitForMillisecondFrames(700);
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-6f, -2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-2f, 2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(2f, 6f), GameManager.RandomTest(2f, 4f)));
        yield return new WaitForMillisecondFrames(600);
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-5f, -2f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(2f, 5f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_ItemHeliRed, new Vector2(2f, 3f)); // Item Heli 1
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-6f, -3f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-3f, 0f), GameManager.RandomTest(2f, 4f)));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-6f, -3f), GameManager.RandomTest(2f, 4f)));
        CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-3f, 0f), GameManager.RandomTest(2f, 4f)));
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_LEFT - 4f, WATER_HEIGHT, 13f), new MoveVector(1.5f, 80f), new MovePattern[] {new MovePattern(5000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(1f, WATER_HEIGHT, 20f), new MoveVector(1.5f, 5f), new MovePattern[] {new (3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(3.2f, WATER_HEIGHT, 22f), new MoveVector(1.5f, -2f), new MovePattern[] {new (3000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(6f, WATER_HEIGHT, 21f), new MoveVector(1.5f, 1f), new MovePattern[] {new (3000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(4f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(4.5f, -5.5f), 1000);
        yield return new WaitForMillisecondFrames(500);
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-6.5f, WATER_HEIGHT, 24f), new MoveVector(1.5f, 2f), new MovePattern[] {new (3700, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(-4.5f, WATER_HEIGHT, 24f), new MoveVector(1.5f, -3f), new MovePattern[] {new (3700, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-2.8f, WATER_HEIGHT, 24.5f), new MoveVector(1.5f, 0f), new MovePattern[] {new (3700, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(0f, WATER_HEIGHT, 25f), new MoveVector(1.5f, 0f), new MovePattern[] {new (3700, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 3f, WATER_HEIGHT, 26f), new MoveVector(1.5f, -50f), new MovePattern[] {new (5000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_2, new Vector3(Size.GAME_BOUNDARY_RIGHT + 4f, WATER_HEIGHT, 24f), new MoveVector(1.5f, -46f), new MovePattern[] {new (5000, 1000, true, 0f)});
        CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(Size.GAME_BOUNDARY_RIGHT + 2f, WATER_HEIGHT, 21f), new MoveVector(1.5f, -40f), new MovePattern[] {new (5000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-4f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-4.5f, -5.5f), 1000);
        yield return new WaitForMillisecondFrames(2000);
        StartCoroutine(SpawnPlaneSmall2s_A());
        yield return new WaitForMillisecondFrames(4000);
        CreateEnemyWithMoveVector(m_ShipCarrier_1, new Vector3(Size.GAME_BOUNDARY_LEFT, WATER_HEIGHT, 34.8f), new MoveVector(0.6f, 70f), new MovePattern[] {new (9000, 1000, true, 0f)});
        yield return new WaitForMillisecondFrames(9000);
        StartCoroutine(SpawnPlaneSmalls_A(4500));
        yield return new WaitForMillisecondFrames(5000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -2f), new Vector2(3f, -2f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(5f, -3.5f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(3.4f, -5f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5f), new Vector2(5.2f, -6.5f), 1000);
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -2f), new Vector2(-3f, -2f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-5f, -3.5f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -4f), new Vector2(-3.4f, -5f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5f), new Vector2(-5.2f, -6.5f), 1000);
        CreateEnemyWithMoveVector(m_ShipLarge, new Vector3(Size.GAME_BOUNDARY_RIGHT + 4f, WATER_HEIGHT, 46f), new MoveVector(1.4f, -80f));
        yield return new WaitForMillisecondFrames(6000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -3f), new Vector2(-1.25f, -3f), 1500);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(1.25f, -3f), 1500);
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_LEFT - 2f, -5.5f), new Vector2(-3f, -6f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -5.5f), new Vector2(3f, -6f), 1000);
        yield return new WaitForMillisecondFrames(4000);
        // Middle Boss (56s)
        yield return new WaitForMillisecondFrames(10000);
        StartCoroutine(SpawnPlaneSmall2s_B());
        yield return new WaitForMillisecondFrames(29200);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -3f), new Vector2(4f, -3f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -7f), new Vector2(4f, -7f), 1000);
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemy(m_ItemHeliRed, new Vector2(3f, 3f)); // Item Heli 1
        CreateEnemy(m_ItemHeliGreen, new Vector2(-3f, 3f)); // Item Heli 2
        yield return new WaitForMillisecondFrames(2000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -4f), new Vector2(2f, -4f), 1000);
        CreateEnemyWithTarget(m_PlaneSmall_3, new Vector2(Size.GAME_BOUNDARY_RIGHT + 2f, -6f), new Vector2(5f, -6f), 1000);
        yield return new WaitForMillisecondFrames(2000);
        if (SystemManager.Difficulty == GameDifficulty.Hell)
            StartCoroutine(SpawnPlaneSmalls_B(5000, 600));
        yield return new WaitForMillisecondFrames(1000);
        CreateEnemyWithMoveVector(m_TankLarge_3, new Vector3(-36.5f, 3f, 26f), new MoveVector(4f, 85f), new MovePattern[] {new MovePattern(1200, 1000, true, 0f)});
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            yield return new WaitForMillisecondFrames(10000);
            CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
            yield return new WaitForMillisecondFrames(12000);
            CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        }
        else {
            yield return new WaitForMillisecondFrames(8000);
            CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
            yield return new WaitForMillisecondFrames(7000);
            CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_LEFT - 3f, -2f));
            yield return new WaitForMillisecondFrames(7000);
            CreateEnemy(m_PlaneMedium_5, new Vector2(Size.GAME_BOUNDARY_RIGHT + 3f, -2f));
        }
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_ItemHeliRed, new Vector2(0f, 3f)); // Item Heli 1
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_A(int duration) {
        int timer = 0;
        int[] period = { 2200, 1400, 1000 };
        while (timer < duration) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(-2f, 0f), 2f));
            if (SystemManager.Difficulty > 0) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(1f, 3f), 4.8f));
            }
            CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(4f, 6f), 2f));
            
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-2f, 0f), 2f));
            if (SystemManager.Difficulty > 0) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(GameManager.RandomTest(1f, 3f), 4.8f));
            }
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(4f, 6f), 2f));
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
            timer += period[(int) SystemManager.Difficulty];
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmalls_B(int duration, int period) {
        int timer = 0;
        while (timer < duration) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(3f, GameManager.RandomTest(2f, 3f)));
            CreateEnemy(m_PlaneSmall_1, new Vector2(8f, GameManager.RandomTest(2f, 3f)));
            yield return new WaitForMillisecondFrames(period);
            timer += period;
            if (timer >= duration) {
                break;
            }
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(5f, 6f), GameManager.RandomTest(2f, 3f)));
            yield return new WaitForMillisecondFrames(period);
            timer += period;
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmall2s_A() {
        int timer = 0;
        int[] period = { 2000, 1000, 600 };
        while (timer < 6000) {
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-7f, -4f), GameManager.RandomTest(2f, 4f)));
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-2f, 2f), GameManager.RandomTest(2f, 4f)));
            CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(4f, 7f), GameManager.RandomTest(2f, 4f)));
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
            timer += period[(int) SystemManager.Difficulty];
        }
        yield break;
    }

    private IEnumerator SpawnPlaneSmall2s_B() {
        var timer = 0;
        int[] period = { 1800, 1000, 600 };
        while (timer < 19800) {
            if (SystemManager.PlayState == PlayState.None) {
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-8f, -6f), GameManager.RandomTest(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-5f, -2.5f), GameManager.RandomTest(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(-1.5f, 1.5f), GameManager.RandomTest(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(2.5f, 5f), GameManager.RandomTest(2f, 3f)));
                CreateEnemy(m_PlaneSmall_2, new Vector2(GameManager.RandomTest(6f, 8f), GameManager.RandomTest(2f, 3f)));
            }
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
            timer += period[(int) SystemManager.Difficulty];
        }
        yield break;
    }
}
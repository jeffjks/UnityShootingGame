﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage1Manager : StageManager
{
    [Space(10)]
    public GameObject m_TankSmall_1, m_TankSmall_2, m_Helicopter, m_PlaneSmall_1, m_ItemHeliRed, m_ShipSmall_1, m_PlaneMedium_1, m_PlaneMedium_3;

    private const float WATER_HEIGHT = 2.32f;

    protected override void Init()
    {
        SystemManager.Stage = 0;
        
        AudioService.LoadMusics("Stage1");
        AudioService.PlayMusic("Stage1");
    }

    protected override IEnumerator MainTimeline()
    {
        InitEnemies();
        BackgroundCamera.SetBackgroundCameraSpeed(18f);
        yield return new WaitForMillisecondFrames(5000);
        BackgroundCamera.SetBackgroundCameraSpeed(1.8f, 1688);

        yield return new WaitForMillisecondFrames(36000);
        BackgroundCamera.SetBackgroundCameraSpeed(2.7f, 750);
        StartCoroutine(MiddleBossStart(new Vector3(12f, -13f, Depth.ENEMY), 1000)); // Middle Boss (42s)

        yield return new WaitForMillisecondFrames(52000);
        
        StartBossTimeline();
    }

    protected override IEnumerator TestTimeline()
    {
        yield break;
    }

    protected override IEnumerator BossTimeline()
    {
        yield return new WaitForMillisecondFrames(3000);
        AudioService.FadeOutMusic();
        yield return new WaitForMillisecondFrames(3000);
        ShowBossWarningSign();
        yield return new WaitForMillisecondFrames(4000);
        BackgroundCamera.SetBackgroundCameraSpeed(7.2f, 938);
        AudioService.PlayMusic("Boss1");
        StartCoroutine(BossStart(new Vector3(0f, 4.5f, Depth.ENEMY), 1000));
        yield return new WaitForMillisecondFrames(2000);
        BackgroundCamera.RepeatBackgroundCamera(16f);
        //BackgroundCamera.SetBackgroundCameraSpeed(0f);
        //UnityStandardAssets.Water.TerrainWater.m_WaveSpeed = 240f;
    }

    protected override IEnumerator EnemyTimeline()
    {
        int random_duration;
        yield return new WaitForMillisecondFrames(8000);
        random_duration = Random.Range(1200, 1500);
        //GetEnemyBuilder("Helicopter").SetPosition(new Vector3(-4f, 3f)).AddTarget(random_duration, new Vector2(-3f, -3f)).Build();
        
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-3f, -3f), random_duration);
        yield return new WaitForMillisecondFrames(2000);
        if (SystemManager.Difficulty >= GameDifficulty.Expert) {
            random_duration = Random.Range(1200, 1500);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(1f, -3f), random_duration);
        }
        random_duration = Random.Range(1200, 1500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(5f, 3f), new Vector2(4f, -4f), random_duration);
        yield return new WaitForMillisecondFrames(2000);
        random_duration = Random.Range(1200, 1500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), random_duration);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(1f, -4f), random_duration);
        yield return new WaitForMillisecondFrames(1000);
        random_duration = Random.Range(1200, 1500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(2f, 3f), new Vector2(5f, -5f), random_duration);
        yield return new WaitForMillisecondFrames(500);
        if (SystemManager.Difficulty >= GameDifficulty.Expert) {
            random_duration = Random.Range(1200, 1500);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(3f, 3f), new Vector2(2f, -1.5f), random_duration);
            random_duration = Random.Range(1200, 1500);
            CreateEnemyWithTarget(m_Helicopter, new Vector2(6f, 3f), new Vector2(6f, -3f), random_duration);
        }
        yield return new WaitForMillisecondFrames(1500);
        CreateEnemy(m_ItemHeliRed, new Vector2(-1f, 3f)); // Item Heli
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneSmall_1, new Vector2(3f, 3f));
        CreateEnemy(m_PlaneSmall_1, new Vector2(6f, 3f));
        yield return new WaitForMillisecondFrames(500);
        if (SystemManager.Difficulty >= GameDifficulty.Expert) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(4f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(7f, 3f));
        }
        yield return new WaitForMillisecondFrames(500);

        if (SystemManager.Difficulty >= GameDifficulty.Hell) { // 3 small ship
            MovePattern[] movePatterns = { new MovePattern(2000, 2000, true, 0f) };
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-10.055f, WATER_HEIGHT, 132.5f), new MoveVector(3f, 70f), movePatterns);
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-11.06f, WATER_HEIGHT, 135.44f), new MoveVector(3f, 70f), movePatterns);
            CreateEnemyWithMoveVector(m_ShipSmall_1, new Vector3(-13.98f, WATER_HEIGHT, 133.93f), new MoveVector(3f, 70f), movePatterns);
        }
        yield return new WaitForMillisecondFrames(9000);
        if (SystemManager.Difficulty >= GameDifficulty.Expert)
            CreateEnemy(m_PlaneMedium_3, new Vector2(-1f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_3, new Vector2(-3f, 3f));
        yield return new WaitForMillisecondFrames(3000);
        CreateEnemy(m_PlaneMedium_1, new Vector2(2f, 3f));
        yield return new WaitForMillisecondFrames(5000);
        random_duration = Random.Range(1200, 1500);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-5f, 4f), new Vector2(-6f, -4f), random_duration);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), random_duration);
        CreateEnemyWithTarget(m_Helicopter, new Vector2(-2f, 3f), new Vector2(-1f, -5f), random_duration);
        yield return new WaitForMillisecondFrames(5000); // Middle Boss ==========================

        for (int i = 0; i < 10; i++) {
            if (SystemManager.PlayState == PlayState.None) {
                CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(-5f, -1f), 3f));
                CreateEnemy(m_PlaneSmall_1, new Vector2(Random.Range(1f, 5f), 3f));
            }
            yield return new WaitForMillisecondFrames(1000);
        }

        if (SystemManager.PlayState == PlayState.None) {
            CreateEnemy(m_PlaneMedium_3, new Vector2(-4f, 3f));
            if (SystemManager.Difficulty >= GameDifficulty.Hell)
                CreateEnemy(m_PlaneMedium_3, new Vector2(4f, 3f));
        }
        yield return new WaitForMillisecondFrames(2000);
        if (SystemManager.PlayState == PlayState.None) {
            CreateEnemyWithTarget(m_Helicopter, new Vector2(4f, 3f), new Vector2(4f, -3f), Random.Range(1200, 1500));
            CreateEnemyWithTarget(m_Helicopter, new Vector2(-4f, 3f), new Vector2(-4f, -3f), Random.Range(1200, 1500));
        }
        yield return new WaitForMillisecondFrames(1500);
        if (SystemManager.PlayState == PlayState.None) {
            CreateEnemy(m_PlaneMedium_3, new Vector2(0f, 3f));
        }
        yield return new WaitForMillisecondFrames(1500);
        if (SystemManager.PlayState == PlayState.None) {
            if (SystemManager.Difficulty >= GameDifficulty.Expert)
                CreateEnemy(m_PlaneMedium_3, new Vector2(3f, 3f));
        }
        yield return new WaitForMillisecondFrames(1000);
        if (SystemManager.PlayState == PlayState.None) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-3f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(-6f, 3f));
        }
        yield return new WaitForMillisecondFrames(500);
        if (SystemManager.PlayState == PlayState.None) {
            CreateEnemy(m_PlaneSmall_1, new Vector2(-3f, 3f));
            CreateEnemy(m_PlaneSmall_1, new Vector2(-6f, 3f));
        }
        yield return new WaitForMillisecondFrames(20000);
        CreateEnemy(m_ItemHeliRed, new Vector2(1.5f, 3f)); // Item Heli
        yield return new WaitForMillisecondFrames(13000);
        CreateEnemy(m_PlaneMedium_1, new Vector2(3f, 3f));
    }
}
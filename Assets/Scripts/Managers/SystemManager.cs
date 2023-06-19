using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SystemManager : MonoBehaviour
{
    [HideInInspector] public int m_UsedCost;
    
    public bool m_DebugMod;
    
    private Vector3 m_BackgroundCameraDefaultLocalPos;

    public static GameMode GameMode  { get; private set; }
    public static GameDifficulty Difficulty { get; private set; }
    public static GameDifficulty DebugDifficulty;
    public static TrainingInfo TrainingInfo;
    public static PlayState PlayState = PlayState.OutGame;
    public static int Stage = -1;
    
    public static SystemManager Instance { get; set; }
    
    public static event Action Action_OnBossClear;
    public static event Action Action_OnStageClear;
    public static event Action Action_OnShowOverview;
    public static event Action<bool> Action_OnNextStage;
    public static event Action Action_OnQuitInGame;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        //gameObject.SetActive(false);
    }

    public static void MiddleBossClear() {
        PlayState = PlayState.OnField;
    }

    public static void BossClear()
    {
        AudioService.StopMusic();
        if (StageManager.IsTrueBossEnabled)
            return;
        Action_OnBossClear?.Invoke();
        PlayState = PlayState.OnBossCleared;
    }

    private IEnumerator StageClear() {
        if (StageManager.IsTrueBossEnabled)
            yield break;
        yield return new WaitForMillisecondFrames(3000);
        PlayState = PlayState.OnStageResult;
        AudioService.PlayMusic("StageClear");
        Action_OnStageClear?.Invoke();
        
        yield return new WaitForMillisecondFrames(2000);
        Action_OnShowOverview?.Invoke();
    }

    public void StartStageClearCoroutine() {
        StartCoroutine(StageClear());
    }

    public void StartNextStageCoroutine() {
        StartCoroutine(NextStage());
    }

    public void StartStage(int stage)
    {
        SceneManager.LoadScene($"Stage{(stage + 1)}");
        PlayState = PlayState.OnField;
        Stage = stage;
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        PlayState = PlayState.OnStageTransition;
        InGameScreenEffectService.TransitionOut();
        yield return new WaitForMillisecondFrames(2000);

        if (GameMode == GameMode.Training && !GameManager.InvincibleMod) {
            QuitGame();
        }
        
        if (Stage < 4) {
            string scene_name = "Stage" + (Stage + 2);
            Stage++;
            Action_OnNextStage?.Invoke(true);
            SceneManager.LoadScene(scene_name);

            PlayState = PlayState.OnField;
        }
        else {
            string scene_name = "Ending";
            Stage = -1;
            
            gameObject.SetActive(false);
            Action_OnNextStage?.Invoke(false);
            SceneManager.LoadScene(scene_name);

            PlayState = PlayState.OnStageResult;
            
            yield return new WaitForMillisecondFrames(1000);
            InGameScreenEffectService.TransitionOut(1.5f);
        }
    }

    public void QuitGame() {
        Action_OnQuitInGame?.Invoke();
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }



    

    public bool SpawnAtSpawnPointCondition() {
        if (Stage == 0) {
            if (GameMode == GameMode.Training)
            {
                return !TrainingInfo.bossOnly;
            }
            return true;
        }
        return false;
    }

    public static void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }

    public static void SetDifficulty(GameDifficulty gameDifficulty)
    {
        Difficulty = gameDifficulty;
    }

    public static bool IsOnGamePlayState()
    {
        if (PlayState == PlayState.OnField)
        {
            return true;
        }
        if (PlayState == PlayState.OnMiddleBoss)
        {
            return true;
        }
        if (PlayState == PlayState.OnBoss)
        {
            return true;
        }

        return false;
    }
}
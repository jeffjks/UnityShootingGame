using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SystemManager : MonoBehaviour
{
    [HideInInspector] public int m_UsedCost;
    
    private Vector3 m_BackgroundCameraDefaultLocalPos;

    public static GameMode GameMode  { get; private set; }
    public static GameDifficulty Difficulty { get; private set; }
    public static GameDifficulty DebugDifficulty;
    public static TrainingInfo TrainingInfo;
    public static PlayState PlayState = PlayState.OutGame;
    public static int Stage = -1;
    
    public static SystemManager Instance { get; private set; }
    
    public static event Action Action_OnBossClear;
    public static event Action Action_OnStageClear;
    public static event Action Action_OnShowOverview;
    public static event Action<bool> Action_OnNextStage;
    public static event Action Action_OnFinishEndingCredit;
    public static event Action Action_OnQuitInGame;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        if (DebugOption.SceneMode == 3)
            StartNextStageCoroutine();
    }

    public static void OnMiddleBossStart()
    {
        PlayState = PlayState.OnMiddleBoss;
    }

    public static void OnBossStart()
    {
        PlayState = PlayState.OnBoss;
    }

    public static void OnMiddleBossClear() {
        PlayState = PlayState.OnField;
    }

    public static void OnBossClear()
    {
        if (DebugOption.SceneMode == 1 || DebugOption.SceneMode == 2)
        {
            PlayState = PlayState.OnField;
            return;
        }
        
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

    public void StartStageClearCoroutine()
    {
        if (DebugOption.SceneMode == 1 || DebugOption.SceneMode == 2)
            return;
        StartCoroutine(StageClear());
    }

    public void StartNextStageCoroutine()
    {
        if (DebugOption.SceneMode == 1 || DebugOption.SceneMode == 2)
            return;
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

        if (GameMode == GameMode.Training && !DebugOption.InvincibleMod) {
            QuitGame(null);
            yield break;
        }
        
        if (Stage < 4 && DebugOption.SceneMode == 0) {
            var sceneName = $"Stage{Stage + 2}";
            Stage++;
            Action_OnNextStage?.Invoke(true);
            SceneManager.LoadScene(sceneName);

            PlayState = PlayState.OnField;
        }
        else {
            var sceneName = "EndingCredit";
            Stage = -1;
            
            //gameObject.SetActive(false);
            Action_OnNextStage?.Invoke(false);
            SceneManager.LoadScene(sceneName);

            PlayState = PlayState.OnStageResult;
            
            yield return new WaitForMillisecondFrames(1000);
            InGameScreenEffectService.TransitionOut(1.5f);
        }
    }

    public void FinishEndingCredit()
    {
        Action_OnFinishEndingCredit?.Invoke();
        SceneManager.LoadScene("EndingRanking");
    }

    public void QuitGame(Action onCompleted) {
        Action_OnQuitInGame?.Invoke();
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        onCompleted?.Invoke();
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
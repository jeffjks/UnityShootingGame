using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class SystemManager : MonoBehaviour
{
    [HideInInspector] public int m_UsedCost;
    
    private Vector3 m_BackgroundCameraDefaultLocalPos;

    private static PlayState _playState = PlayState.None;
    public static GameMode GameMode  { get; private set; }
    public static GameDifficulty Difficulty { get; private set; }
    public static GameDifficulty DebugDifficulty;
    public static TrainingInfo TrainingInfo;
    public static bool IsInGame;
    public static int Stage = -1;
    public static int CurrentSeed;

    public static PlayState PlayState
    {
        get => _playState;
        set
        {
            _playState = value;
            Action_OnPlayStateChanged?.Invoke(_playState);
        }
    }
    
    public static SystemManager Instance { get; private set; }
    
    public static event Action Action_OnBossInteractable;
    public static event Action Action_OnBossClear;
    public static event Action Action_OnStageClear;
    public static event Action Action_OnShowOverview;
    public static event Action<bool> Action_OnNextStage;
    public static event Action Action_OnFinishEndingCredit;
    public static event Action Action_OnQuitInGame;
    public static event Action<PlayState> Action_OnPlayStateChanged;
    
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        Action_OnQuitInGame += () => IsInGame = false;

        if (DebugOption.SceneMode == 3)
            StartNextStageCoroutine();
    }

    public static void OnMiddleBossStart()
    {
        PlayState = PlayState.OnMiddleBoss;
    }

    public static void OnBossInteractable()
    {
        Action_OnBossInteractable?.Invoke();
    }

    public static void OnMiddleBossClear() {
        PlayState = PlayState.None;
    }

    public static void OnBossClear()
    {
        if (DebugOption.SceneMode == 1 || DebugOption.SceneMode == 2)
        {
            PlayState = PlayState.None;
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
        yield return new WaitForMillisecondFrames(4000);
        PlayState = PlayState.OnStageResult;
        AudioService.PlayMusic("StageClear");
        Action_OnStageClear?.Invoke();
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

    public static void StartStage(int stage, int seed)
    {
        CurrentSeed = seed;
        IsInGame = true;
        SceneManager.LoadScene($"Stage{stage + 1}");
        Stage = stage;
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        PlayState = PlayState.OnStageTransition;
        FadeScreenService.ScreenFadeOut(2f);
        yield return new WaitForMillisecondFrames(2000);

        if (GameMode == GameMode.Training && !DebugOption.InvincibleMod) {
            QuitGame(null);
            yield break;
        }
        if (GameMode == GameMode.Replay && Stage >= 5)
        {
            QuitGame(null);
            yield break;
        }
        
        if (Stage < 4 && DebugOption.SceneMode == 0) {
            var sceneName = $"Stage{Stage + 2}";
            Stage++;
            Action_OnNextStage?.Invoke(true);
            SceneManager.LoadScene(sceneName);
        }
        else {
            var sceneName = "EndingCredit";
            Stage = -1;
            
            //gameObject.SetActive(false);
            Action_OnNextStage?.Invoke(false);
            SceneManager.LoadScene(sceneName);
            
            // yield return new WaitForMillisecondFrames(1000);
            // FadeScreenService.ScreenFadeOut(1.5f);
        }
    }

    public void FinishEndingCredit()
    {
        Action_OnFinishEndingCredit?.Invoke();
        SceneManager.LoadScene("EndingRecord");
    }

    public static void QuitGame(Action onCompleted) {
        Action_OnQuitInGame?.Invoke();
        Instance.StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
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
}
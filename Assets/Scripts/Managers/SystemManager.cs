using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SystemManager : MonoBehaviour
{
    [HideInInspector] public StageManager m_StageManager;
    [HideInInspector] public int BulletsSortingLayer;
    [HideInInspector] public int m_BulletsEraseTimer;
    [HideInInspector] public int m_UsedCost;
    
    public bool m_DebugMod;
    
    private GameManager m_GameManager = null;
    
    private Vector3 m_BackgroundCameraDefaultLocalPos;

    public static GameMode GameMode  { get; private set; }
    public static GameDifficulty Difficulty { get; private set; }
    public static GameDifficulty DebugDifficulty;
    public static TrainingInfo TrainingInfo;
    public static PlayState PlayState = PlayState.OutGame;
    public static int Stage = -1;
    
    public static SystemManager instance_sm = null;
    
    public event Action Action_OnBossClear;
    public event Action Action_OnStageClear;
    public event Action Action_OnShowOverview;
    public event Action<bool> Action_OnNextStage;
    public event Action Action_OnQuitInGame;

    void Awake()
    {
        if (instance_sm != null) {
            Destroy(gameObject);
            return;
        }
        instance_sm = this;

        m_GameManager = GameManager.instance_gm;
        
        DontDestroyOnLoad(gameObject);
        
        PlayState = PlayState.OnField;

        //gameObject.SetActive(false);
    }

    void Update()
    {
        BulletEraseTimer();
    }


    public void MiddleBossClear() {
        PlayState = PlayState.OnField;
    }

    public void BossClear()
    {
        AudioService.StopMusic();
        if (m_StageManager.GetTrueLastBossState())
            return;
        Action_OnBossClear?.Invoke();
        PlayState = PlayState.OnBossCleared;
    }

    private IEnumerator StageClear() {
        if (m_StageManager.GetTrueLastBossState())
            yield break;
        yield return new WaitForMillisecondFrames(3000);
        PlayState = PlayState.OnStageResult;
        AudioService.PlayMusic("StageClear");
        Action_OnStageClear?.Invoke();
        
        yield return new WaitForMillisecondFrames(2000);
        Action_OnShowOverview?.Invoke();
        m_StageManager.StopCoroutine("MainTimeline");
    }

    public void StartStageClearCoroutine() {
        StartCoroutine(StageClear());
    }

    public void StartNextStageCoroutine() {
        StartCoroutine(NextStage());
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        PlayState = PlayState.OnStageTransition;
        InGameScreenEffectService.TransitionOut();
        yield return new WaitForMillisecondFrames(2000);

        if (GameMode == GameMode.Training && !m_GameManager.m_InvincibleMod) {
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


    private void BulletEraseTimer() {
        if (m_BulletsEraseTimer > 0) {
            m_BulletsEraseTimer--;
        }
        else {
            m_BulletsEraseTimer = 0;
        }
    }

    public void BulletsToGems(int millisecond) {
        List<GameObject> bullet_list = new List<GameObject>();
        bullet_list.AddRange(GameObject.FindGameObjectsWithTag("EnemyBulletParent"));
        int index, num = 0, count = bullet_list.Count;

        while (count > 0) {
            index = UnityEngine.Random.Range(0, count);
            if (num < 50) {
                Vector3 pos = bullet_list[index].transform.position;
                if (Size.GAME_BOUNDARY_LEFT < pos.x && pos.x < Size.GAME_BOUNDARY_RIGHT) {
                    if (Size.GAME_BOUNDARY_BOTTOM < pos.y && pos.y < Size.GAME_BOUNDARY_TOP) {
                        GameObject gem = PoolingManager.PopFromPool("ItemGemAir", PoolingParent.GemAir); // Gem 생성
                        gem.transform.position = new Vector3(bullet_list[index].transform.position.x, bullet_list[index].transform.position.y, Depth.ITEMS);
                        gem.SetActive(true);
                        num++;
                    }
                }
            }
            EnemyBullet enemy_bullet = bullet_list[index].GetComponent<EnemyBullet>();
            enemy_bullet.ReturnToPool();
            bullet_list.RemoveAt(index);

            count--;
        }
        EraseBullets(millisecond);
    }

    public void EraseBullets(int millisecond) {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (m_BulletsEraseTimer < frame) {
            m_BulletsEraseTimer = frame;
        }
    }


    public bool GetInvincibleMod() {
        return m_GameManager.m_InvincibleMod;
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
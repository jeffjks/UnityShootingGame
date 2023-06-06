using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SystemManager : MonoBehaviour
{
    public Camera m_BackgroundCamera;
    public MainCamera m_MainCamera;
    public OverviewHandler m_OverviewHandler;
    public PlayerManager m_PlayerManager;

    [HideInInspector] public StageManager m_StageManager;
    [HideInInspector] public Vector2 m_BackgroundCameraSize;
    [HideInInspector] public int BulletsSortingLayer;
    [HideInInspector] public int m_BulletsEraseTimer;
    [HideInInspector] public int m_UsedCost;

    public Action<long> Action_OnUpdateScore;
    public Action<int> Action_OnUpdateBombNumber;
    
    public bool m_DebugMod;
    
    private GameManager m_GameManager = null;
    private PoolingManager m_PoolingManager = null;

    private PlayerController m_PlayerController;
    private Vector3 m_BackgroundCameraDefaultLocalPos;
    private long m_TotalScore;
    private long[] m_StageScore = new long[5] {0, 0, 0, 0, 0};
    private int m_GemsGround, m_GemsAir; // 점수가 아닌 먹은 개수
    private int m_Stage;
    private int m_BombNumber, m_MaxBombNumber;
    private int m_TotalMiss;
    private int[] m_StageMiss = new int[5] {0, 0, 0, 0, 0};
    private int m_BulletNumber;
    private long m_ClearedTime;

    public static GameMode GameMode  { get; private set; }
    public static GameDifficulty Difficulty { get; private set; }
    public static GameDifficulty DebugDifficulty;
    public static TrainingInfo TrainingInfo;
    public static PlayState PlayState = PlayState.OutGame;
    
    public static SystemManager instance_sm = null;

    public event Action Action_OnEnableSystemManager;
    public event Action Action_OnDisableSystemManager;

    void Awake()
    {
        if (instance_sm != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_sm = this;

        m_GameManager = GameManager.instance_gm;
        //m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;

        m_BackgroundCameraSize.x = m_BackgroundCamera.orthographicSize * 2 * ((float) Screen.width/(float) Screen.height); // 256/9 = 28.444..
        m_BackgroundCameraSize.y = m_BackgroundCamera.orthographicSize * 2; // 16
        m_BackgroundCameraDefaultLocalPos = m_BackgroundCamera.transform.localPosition;
        
        DontDestroyOnLoad(gameObject);
        
        m_PlayerManager.Init();

        gameObject.SetActive(false);
    }

    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
    }

    void OnEnable() { // Start였음
        InitCamera();
        Init();
        Action_OnEnableSystemManager?.Invoke();
        
        UpdateBombNumber();
        
        UpdateScore();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Action_OnDisableSystemManager?.Invoke();
    }

    void Update()
    {
        MoveBackgroundCamera();
        BulletEraseTimer();
    }

    private void Init()
    {
        m_TotalScore = 0;
        m_StageScore = new long[5] {0, 0, 0, 0, 0};
        m_GemsGround = 0; // 점수가 아닌 먹은 개수
        m_GemsAir = 0; // 점수가 아닌 먹은 개수
        m_TotalMiss = 0;
        m_StageMiss = new int[5] {0, 0, 0, 0, 0};
        m_BulletNumber = 0;
        
        PlayState = PlayState.OnField;

        m_OverviewHandler.gameObject.SetActive(false);
    }

    private void MoveBackgroundCamera() {
        if (m_StageManager != null) {
            m_BackgroundCamera.transform.position += m_StageManager.m_BackgroundVector / Application.targetFrameRate * Time.timeScale;
        }
    }

    public IEnumerator MoveBackgroundCameraDuration(bool relative, float position_z, int duration) {
        int frame = duration * Application.targetFrameRate / 1000;
        float init_position_z = m_BackgroundCamera.transform.position.z;
        float target_position_z;

        if (relative) {
            target_position_z = init_position_z + position_z;
        }
        else {
            target_position_z = position_z;
        }

        for (int i = 0; i < frame; ++i) {
            float t_pos_z = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            
            position_z = Mathf.Lerp(init_position_z, target_position_z, t_pos_z);
            m_BackgroundCamera.transform.position = new Vector3(m_BackgroundCamera.transform.position.x, m_BackgroundCamera.transform.position.y, position_z);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private void InitCamera() {
        m_MainCamera.InitMainCamera();
        m_BackgroundCamera.transform.localPosition = m_BackgroundCameraDefaultLocalPos;
    }
    
    public void SetPlayerController(PlayerController playerController) {
        m_PlayerController = playerController;
    }

    public void ShakeCamera(float duration) {
        m_MainCamera.ShakeCamera(duration);
    }


    public void MiddleBossClear() {
        PlayState = PlayState.OnField;
    }

    public void BossClear()
    {
        AudioService.StopMusic();
        PlayState = PlayState.OnBossCleared;
        if (m_StageManager.GetTrueLastBossState())
            return;
        m_PlayerController.DisableInvincibility(5000);
    }

    private IEnumerator StageClear() {
        if (m_StageManager.GetTrueLastBossState())
            yield break;
        yield return new WaitForMillisecondFrames(3000);
        PlayState = PlayState.OnStageResult;
        m_PlayerManager.m_PlayerControlable = false;
        AudioService.PlayMusic("StageClear");
        yield return new WaitForMillisecondFrames(2000);
        m_StageManager.SetBackgroundSpeed(0f);
        m_StageManager.StopCoroutine("MainTimeline");
        m_OverviewHandler.gameObject.SetActive(true);
    }

    public void StartStageClearCoroutine() {
        StartCoroutine(StageClear());
    }

    public void StartNextStageCoroutine() {
        StartCoroutine(NextStage());
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        PlayState = PlayState.OnStageTransition;
        ScreenEffectService.ScreenFadeIn();
        yield return new WaitForMillisecondFrames(2000);

        if (GameMode == GameMode.Training && !m_GameManager.m_InvincibleMod) {
            QuitGame();
        }
        else {
            m_OverviewHandler.gameObject.SetActive(false);
            Vector3 player_pos = m_PlayerController.transform.position;
            m_PlayerController.transform.position = new Vector3(0f, player_pos.y, player_pos.z);
            InitCamera();
            
            if (m_Stage < 4) {
                string scene_name = "Stage" + (m_Stage + 2);
                m_Stage++;
                SceneManager.LoadScene(scene_name);

                PlayState = PlayState.OnField;
                m_PlayerManager.m_PlayerControlable = true;
                m_PlayerController.DisableInvincibility(3000);
                ScreenEffectService.ScreenTransitionIn();
            }
            else {
                string scene_name = "Ending";
                m_Stage++;

                m_PlayerManager.DestroyPlayer();
                gameObject.SetActive(false);
                SceneManager.LoadScene(scene_name);

                PlayState = PlayState.OnStageResult;
                yield return new WaitForMillisecondFrames(1000);
                ScreenEffectService.ScreenFadeOut(1.5f);
            }
        }
    }

    public void QuitGame() {
        //Time.timeScale = 1;
        //AudioListener.pause = false;
        m_PlayerManager.DestroyPlayer();
        //Destroy(m_PoolingManager.gameObject);
        //Destroy(gameObject);
        gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void AddScore(long score) {
        m_TotalScore += score;
        m_StageScore[m_Stage] += score;
        UpdateScore();
    }

    private void UpdateScore()
    {
        Action_OnUpdateScore?.Invoke(m_TotalScore);
    }

    public void AddScoreEffect(long score) {
        AddScore(score);
        DisplayScoreText(""+score);
    }

    public void AddScoreEffect(long score, bool ground_gem) { // Overload
        AddScore(score);
        if (ground_gem) {
            m_GemsGround++;
        }
        else {
            m_GemsAir++;
        }
        DisplayScoreText(""+score);
    }

    public void DisplayScoreText(string score) {
        GameObject obj = m_PoolingManager.PopFromPool("ScoreText", PoolingParent.SCORE_TEXT);
        ScoreText score_text = obj.GetComponent<ScoreText>();
        Vector3 pos = m_PlayerController.transform.position;
        score_text.m_TextMesh.text = score;
        if (pos.x < 0f) {
            obj.transform.position = new Vector3(pos.x + 1f, pos.y + 1f, Depth.SCORE_TEXT);
            score_text.m_TextMesh.alignment = TextAlignment.Left;
            score_text.m_TextMesh.anchor = TextAnchor.LowerLeft;
        }
        else {
            obj.transform.position = new Vector3(pos.x - 1f, pos.y + 1f, Depth.SCORE_TEXT);
            score_text.m_TextMesh.alignment = TextAlignment.Right;
            score_text.m_TextMesh.anchor = TextAnchor.LowerRight;
        }
        obj.SetActive(true);
        score_text.OnStart();
    }


    public void SetCurrentStage(int stage) {
        m_Stage = stage;
    }

    public int GetCurrentStage() {
        return m_Stage;
    }

    public void InitBombNumber() {
        m_BombNumber = m_MaxBombNumber;
        UpdateBombNumber();
    }

    public void AddBombNumber(int add) {
        m_BombNumber += add;
        UpdateBombNumber();
    }

    private void UpdateBombNumber()
    {
        Action_OnUpdateBombNumber?.Invoke(m_BombNumber);
    }

    public int GetBombNumber() {
        return m_BombNumber;
    }

    public void SetMaxBombNumber(int max_bomb_number) {
        m_MaxBombNumber = max_bomb_number;
    }

    public int GetMaxBombNumber() {
        return m_MaxBombNumber;
    }

    public long GetTotalScore() {
        long total_score = 0;
        for (int i = 0; i < m_StageScore.Length; i++) {
            total_score += m_StageScore[i];
        }
        return total_score;
    }

    public long GetCurrentStageScore() {
        return m_StageScore[m_Stage];
    }

    public void AddMiss() {
        m_TotalMiss++;
        m_StageMiss[m_Stage]++;
    }

    public int GetTotalMiss() {
        int total_miss = 0;
        for (int i = 0; i < m_StageMiss.Length; i++) {
            total_miss += m_StageMiss[i];
        }
        return total_miss;
    }

    public int GetCurrentStageMiss() {
        return m_StageMiss[m_Stage];
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
                        GameObject gem = m_PoolingManager.PopFromPool("ItemGemAir", PoolingParent.ITEM_GEM_AIR); // Gem 생성
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


    public void AddBullet() {
        m_BulletNumber++;
        //Debug.Log("Bullet: "+m_BulletNumber);
    }

    public void SubtractBullet() {
        m_BulletNumber--;
        //Debug.Log("Bullet: "+m_BulletNumber);
    }

    public int GemsGround {
        get { return m_GemsGround; }
        set { m_GemsGround = value; }
    }

    public int GemsAir {
        get { return m_GemsAir; }
        set { m_GemsAir = value; }
    }

    public bool GetInvincibleMod() {
        return m_GameManager.m_InvincibleMod;
    }

    public void SaveClearedTime() {
        m_ClearedTime = DateTime.Now.Ticks;
    }

    public long GetClearedTime() {
        return m_ClearedTime;
    }

    public bool SpawnAtSpawnPointCondition() {
        if (GetCurrentStage() == 0) {
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
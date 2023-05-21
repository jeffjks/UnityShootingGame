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
    public TextUI_GameMode m_TextUI_GameMode;
    public PlayerManager m_PlayerManager;

    [SerializeField] private Text m_ScoreNumberText = null;
    [SerializeField] private Text m_DifficultyText = null;
    [SerializeField] private Text m_BombNumberText = null;
    [SerializeField] private WarningUI m_WarningUI = null;

    [HideInInspector] public StageManager m_StageManager;
    [HideInInspector] public Vector2 m_BackgroundCameraSize;
    [HideInInspector] public int BulletsSortingLayer;
    [HideInInspector] public int m_BulletsEraseTimer;

    [HideInInspector] public int m_PlayState; // 0: 평소, 1: 보스/중간보스전, 2: 보스 클리어, 3: 점수/엔딩 화면, 4: 다음 스테이지 전환중
    [HideInInspector] public GameMode m_GameMode;
    [HideInInspector] public int m_UsedCost;
    [HideInInspector] public TrainingInfo m_TrainingInfo;
    
    public bool m_DebugMod;
    public DebugDifficulty m_DebugDifficulty;
    
    private GameManager m_GameManager = null;
    private PoolingManager m_PoolingManager = null;

    private PlayerController m_PlayerController;
    private Vector3 m_BackgroundCameraDefaultLocalPos;
    private long m_TotalScore;
    private long[] m_StageScore = new long[5] {0, 0, 0, 0, 0};
    private int m_GemsGround, m_GemsAir; // 점수가 아닌 먹은 개수
    private int m_Stage;
    private int m_BombNumber, m_MaxBombNumber;
    private byte m_TotalMiss;
    private byte[] m_StageMiss = new byte[5] {0, 0, 0, 0, 0};
    private int m_BulletNumber;
    private int m_Difficulty;
    private long m_ClearedTime;

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

        m_TextUI_GameMode.FadeEffect();
        
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
        m_TextUI_GameMode.UpdateGameModeText(m_GameMode);
        
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
        m_StageMiss = new byte[5] {0, 0, 0, 0, 0};
        m_BulletNumber = 0;
        
        m_PlayState = 0;

        m_OverviewHandler.gameObject.SetActive(false);
        m_WarningUI.gameObject.SetActive(false);
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
        yield break;
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
        m_PlayState = 0;
    }

    public void BossClear()
    {
        AudioService.StopMusic();
        m_PlayState = 2;
        if (m_StageManager.GetTrueLastBossState())
            return;
        m_PlayerController.DisableInvincibility(5000);
    }

    private IEnumerator StageClear() {
        if (m_StageManager.GetTrueLastBossState())
            yield break;
        yield return new WaitForMillisecondFrames(3000);
        m_PlayState = 3;
        m_PlayerManager.m_PlayerControlable = false;
        AudioService.PlayMusic("StageClear");
        yield return new WaitForMillisecondFrames(2000);
        m_StageManager.SetBackgroundSpeed(0f);
        m_StageManager.StopCoroutine("MainTimeline");
        m_OverviewHandler.gameObject.SetActive(true);
        m_OverviewHandler.DisplayOverview();
    }

    public void StartStageClearCoroutine() {
        StartCoroutine(StageClear());
    }

    public void StartNextStageCoroutine() {
        StartCoroutine(NextStage());
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        m_PlayState = 4;
        ScreenEffectService.ScreenFadeIn();
        yield return new WaitForMillisecondFrames(2000);

        if (m_GameMode == GameMode.GAMEMODE_TRAINING && !m_GameManager.m_InvincibleMod) {
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

                m_PlayState = 0;
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

                m_PlayState = 3;
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


    private void UpdateScore() {
        m_ScoreNumberText.text = "" + m_TotalScore;
    }

    public void AddScore(long score) {
        m_TotalScore += score;
        m_StageScore[m_Stage] += score;
        UpdateScore();
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

    public void SetBombNumber(int add) {
        m_BombNumber += add;
        UpdateBombNumber();
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

    public void UpdateBombNumber() {
        m_BombNumberText.text = m_BombNumber + " / " + m_MaxBombNumber;
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

    public byte GetCurrentStageMiss() {
        return m_StageMiss[m_Stage];
    }


    public int GetDifficulty() {
        return m_Difficulty;
    }

    public void SetDifficulty(int difficulty)
    {
        m_Difficulty = difficulty;
        SetDifficultyText();
    }

    private void SetDifficultyText() {
        switch(m_Difficulty) {
            case 0:
                m_DifficultyText.text = Difficulty.DIFFICULTY1;
                break;
            case 1:
                m_DifficultyText.text = Difficulty.DIFFICULTY2;
                break;
            case 2:
                m_DifficultyText.text = Difficulty.DIFFICULTY3;
                break;
            default:
                break;
        }
    }

    public void WarningText() {
        StartCoroutine(WarningTextAnimation());
    }

    private IEnumerator WarningTextAnimation() {
        float value = 0f;
        float time = 0f;
        AudioService.PlaySound("BossAlert1");

        m_WarningUI.m_WarningPanelWhite.color = new Color(1f, 1f, 1f, 0f);
        m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, 0f, m_WarningUI.transform.localScale.z);
        m_WarningUI.gameObject.SetActive(true);

        while (value < 1f) { // Increase yScale
            m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, value, m_WarningUI.transform.localScale.z);
            value += Time.deltaTime * 4f;
            value = Mathf.Clamp01(value);
            yield return new WaitForFrames(0);
        }
        value = 0f;

        while (value < 1f) { // White Blink Effect
            m_WarningUI.m_WarningPanelWhite.color = new Color(1f, 1f, 1f, 1f - value);
            value += Time.deltaTime * 3f;
            value = Mathf.Clamp01(value);
            yield return new WaitForFrames(0);
        }
        value = 0f;

        while (time < 3f) {
            float alpha_gap = 0.4f;
            while (value < 1f) { // Text Effect
                m_WarningUI.m_WarningText.color = new Color(1f - value*alpha_gap, 1f - value*alpha_gap, 1f - value*alpha_gap, 1f);
                value += Time.deltaTime * 4f;
                value = Mathf.Clamp01(value);
                time += Time.deltaTime;
                yield return new WaitForFrames(0);
            }
            value = 0f;
            yield return new WaitForFrames(0);
        }
        value = 0f;

        while (value < 1f) { // Decrease yScale
            m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, 1f - value, m_WarningUI.transform.localScale.z);
            value += Time.deltaTime * 4f;
            value = Mathf.Clamp01(value);
            yield return new WaitForFrames(0);
        }
        value = 0f;

        m_WarningUI.gameObject.SetActive(false);
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
            if (m_GameMode == GameMode.GAMEMODE_TRAINING) {
                if (m_TrainingInfo.m_BossOnly) {
                    return false;
                }
                else {
                    return true;
                }
            }
            return true;
        }
        return false;
    }
}
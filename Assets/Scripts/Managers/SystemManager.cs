using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public static class Depth // Z Axis
{
    // Far
    public const sbyte ENEMY = 32 + (sbyte) Size.MAIN_CAMERA_POS; // Only Air Enemy
    public const sbyte EXPLOSION = 22 + (sbyte) Size.MAIN_CAMERA_POS; // Only Air Explosion
    public const sbyte OVERVIEW = 20 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte ITEMS = 20 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte TRANSITION = 19 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte PLAYER = 18 + (sbyte) Size.MAIN_CAMERA_POS; // Player, Laser
    public const sbyte PLAYER_MISSILE = 17 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte WHITE_EFFECT = 16 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte SCORE_TEXT = 15 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte ENEMY_BULLET = 14 + (sbyte) Size.MAIN_CAMERA_POS;
    public const sbyte CAMERA = 0 + (sbyte) Size.MAIN_CAMERA_POS;
    // Close
}

public static class Layer // AirSmall(9), AirLarge(10), GroundSmall(11), GroundLarge(12)
{
    public const int SMALL = 2560; // (1 << 9 | 1 << 11) LayerMask.GetMask("AirSmallEnemy", "GroundSmallEnemy")
    public const int LARGE = 5120; // (1 << 10 | 1 << 12) LayerMask.GetMask("AirLargeEnemy", "GroundLargeEnemy")
    public const int AIR = 1536; // (1 << 9 | 1 << 10) LayerMask.GetMask("AirSmallEnemy", "AirLargeEnemy")
    public const int GROUND = 6144; // (1 << 11 | 1 << 12) LayerMask.GetMask("GroundSmallEnemy", "GroundLargeEnemy")
}

public static class ItemScore // Z Axis
{
    // Far
    public const ushort GEM_GROUND = 200;
    public const ushort GEM_AIR = 100;
    public const ushort POWERUP = 2000;
    public const ushort BOMB = 5000;
    // Close
}

public static class BonusScale // Overview
{
    public const float BONUS_0 = 0.5f;
    public const float BONUS_1 = 0.3f;
    public const float BONUS_2 = 0.1f;
}

public static class Difficulty
{
    public const byte DIFFICULTY_SIZE = 3;
    public const string DIFFICULTY1 = "Normal";
    public const string DIFFICULTY2 = "Expert";
    public const string DIFFICULTY3 = "Hell";
    public const byte NORMAL = 0;
    public const byte EXPERT = 1;
    public const byte HELL = 2;
}

public class SystemManager : MonoBehaviour
{
    public enum DebugDifficulty
    {
        Normal,
        Expert,
        Hell,
    }
    
    public Camera m_BackgroundCamera;
    public SoundManager m_SoundManager;
    public ScreenEffecter m_ScreenEffecter;
    public MainCamera m_MainCamera;
    public BossHealthHandler m_BossHealthBar;
    public OverviewHandler m_OverviewHandler;

    [SerializeField] private Text m_ScoreNuberText = null;
    [SerializeField] private Text m_DifficultyText = null;
    [SerializeField] private Text m_StateText = null;
    [SerializeField] private Text m_BombNumberText = null;
    [SerializeField] private WarningUI m_WarningUI = null;
    [SerializeField] private GameObject m_Transition = null;
    [SerializeField] private AudioSource m_AudioWarning = null;
    [SerializeField] private AudioSource m_AudioStageClear = null;

    [HideInInspector] public StageManager m_StageManager;
    [HideInInspector] public Vector2 m_BackgroundCameraSize;
    [HideInInspector] public byte m_PlayState; // 0: 평소, 1: 보스/중간보스전, 2: 보스 클리어, 3: 점수/엔딩 화면, 4: 다음 스테이지 전환중
    [HideInInspector] public int BulletsSortingLayer;
    [HideInInspector] public float m_BulletsEraseTimer;
    [HideInInspector] public byte m_Difficulty;
    [HideInInspector] public bool m_ReplayState, m_TrainingState, m_BossOnlyState;
    
    public bool m_DebugMod, m_InvincibleMod;
    public DebugDifficulty m_DebugDifficulty;
    
    private GameManager m_GameManager = null;
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    private List<ScreenEffectAnimation> m_TransitionList = new List<ScreenEffectAnimation>();
    private PlayerController m_PlayerController;
    private Vector3 m_BackgroundCameraDefaultPos;
    private uint m_TotalScore;
    private uint[] m_StageScore = new uint[5] {0, 0, 0, 0, 0};
    private uint m_GemsGround, m_GemsAir; // 점수가 아닌 먹은 개수
    private int m_Stage;
    private int m_BombNumber, m_MaxBombNumber;
    private byte m_TotalMiss;
    private byte[] m_StageMiss = new byte[5] {0, 0, 0, 0, 0};
    private uint m_BulletNumber;

    private Sequence m_SequenceReplayText;

    public static SystemManager instance_sm = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;

        Application.targetFrameRate = 60;

        if (instance_sm != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_sm = this;
        
        try {
            m_ReplayState = m_GameManager.m_ReplayState;
            m_TrainingState = m_GameManager.m_TrainingState;
            if (m_TrainingState) {
                SetDifficulty(m_GameManager.m_TrainingInfo.m_Difficulty);
                m_BossOnlyState = m_GameManager.m_TrainingInfo.m_BossOnly;
            }
            else {
                SetDifficulty(m_GameManager.m_Difficulty);
            }
            AudioListener audioListener = gameObject.GetComponent<AudioListener>();
            audioListener.enabled = false;
        }
        catch (System.NullReferenceException) {
            m_ReplayState = false;
            m_TrainingState = false;
            SetDifficulty((byte) m_DebugDifficulty);
        }

        m_BackgroundCameraSize.x = m_BackgroundCamera.orthographicSize * 2 * ((float) Screen.width/(float) Screen.height); // 256/9 = 28.444..
        m_BackgroundCameraSize.y = m_BackgroundCamera.orthographicSize * 2; // 16
        m_BackgroundCameraDefaultPos = m_BackgroundCamera.transform.position;
        
        DontDestroyOnLoad(gameObject);

        DOTween.SetTweensCapacity(1024, 64);
    }

    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
        UpdateBombNumber();

        if (m_ReplayState || m_TrainingState) {
            if (m_ReplayState) {
                m_StateText.text = "REPLAY";
            }
            else {
                m_StateText.text = "Training";
            }
            m_StateText.gameObject.SetActive(true);
            m_SequenceReplayText = DOTween.Sequence()
            .Append(m_StateText.DOFade(0f, 0.2f))
            .Append(m_StateText.DOFade(1f, 0.6f))
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        }

        CreateTransition();
        ScreenEffect(2);
        UpdateScore();
    }

    void Update()
    {
        MoveBackgroundCamera();
        BulletEraseTimer();
    }

    private void MoveBackgroundCamera() {
        if (m_StageManager != null)
            m_BackgroundCamera.transform.position += m_StageManager.m_BackgroundVector*Time.deltaTime;
    }

    public void SetPlayerManager() {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PlayerController = m_PlayerManager.m_Player.GetComponent<PlayerController>();
    }

    private void CreateTransition() {
        int r = 1;
        for(int i = 0; i < 8; i++) { // 가로 6개, 세로 8개. 12*16
            for(int j = 0; j < 6; j++) {
                GameObject ins = Instantiate(m_Transition, new Vector3(j*2f-5f, i*2f-15f, Depth.TRANSITION), Quaternion.Euler(0, 0, 45+45*r)); // depth = -4f
                ins.transform.parent = m_ScreenEffecter.transform;
                ScreenEffectAnimation animation = ins.GetComponent<ScreenEffectAnimation>();
                m_TransitionList.Add(animation);
                r *= -1;
            }
            r *= -1;
        }
    }

    public void ScreenEffect(byte num) {
        switch (num) {
            case 0:
                m_ScreenEffecter.StartCoroutine("WhiteEffectSmall");
                break;
            case 1:
                m_ScreenEffecter.StartCoroutine("WhiteEffectBig");
                break;
            case 2: // TransitionEffect
                for (int i = 0; i < m_TransitionList.Count; i++) {
                    m_TransitionList[i].gameObject.SetActive(true);
                    m_TransitionList[i].PlayTransition();
                }
                break;
            case 3: // FadeIn
                for (int i = 0; i < m_TransitionList.Count; i++) { // Fade In (End Overview)
                    m_TransitionList[i].gameObject.SetActive(true);
                    m_TransitionList[i].PlayFadeIn();
                }
                break;
            case 4: // FadeOut
                for (int i = 0; i < m_TransitionList.Count; i++) { // Fade Out (Ending Credit)
                    m_TransitionList[i].gameObject.SetActive(true);
                    m_TransitionList[i].PlayFadeOut();
                }
                break;
            default:
                break;
        }
    }

    public void ShakeCamera(float duration) {
        m_MainCamera.ShakeCamera(duration);
    }


    public void MiddleBossClear() {
        m_PlayState = 0;
    }

    public void BossClear() {
        m_StageManager.StopMusic();
        m_PlayState = 2;
        if (m_StageManager.GetTrueLastBossState())
            return;
        m_PlayerController.EnableInvincible(5f);
    }

    public IEnumerator StageClear() {
        if (m_StageManager.GetTrueLastBossState())
            yield break;
        yield return new WaitForSeconds(3f);
        m_PlayState = 3;
        m_PlayerManager.PlayerControlable = false;
        m_AudioStageClear.Play();
        yield return new WaitForSeconds(2f);
        m_StageManager.SetBackgroundSpeed(0f);
        m_StageManager.StopCoroutine("MainTimeLine");
        m_OverviewHandler.gameObject.SetActive(true);
        m_OverviewHandler.DisplayOverview();
        yield break;
    }

    private IEnumerator NextStage() { // 2스테이지 부터
        m_PlayState = 4;
        ScreenEffect(3); // FadeIn
        yield return new WaitForSeconds(2f);

        if (m_TrainingState) {
            QuitGame();
        }
        else {
            m_BackgroundCamera.transform.position = m_BackgroundCameraDefaultPos;
            m_OverviewHandler.gameObject.SetActive(false);
            Vector3 player_pos = m_PlayerController.transform.position;
            m_PlayerController.transform.position = new Vector3(0f, player_pos.y, player_pos.z);
            
            if (m_Stage < 4) {
                string scene_name = "Stage" + (m_Stage + 2);
                m_Stage++;
                SceneManager.LoadScene(scene_name);

                m_PlayState = 0;
                m_PlayerManager.PlayerControlable = true;
                m_PlayerController.EnableInvincible(3f);
                ScreenEffect(2); // Transition
            }
            else {
                string scene_name = "Ending";
                SceneManager.LoadScene(scene_name);

                m_PlayState = 3;
                yield return new WaitForSeconds(1f);
                ScreenEffect(4); // FadeOut
            }
        }
        yield break;
    }

    public void QuitGame() {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Destroy(m_PlayerManager.m_Player);
        Destroy(m_PoolingManager.gameObject);
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }


    private void UpdateScore() {
        m_ScoreNuberText.text = "" + m_TotalScore;
    }

    public void AddScore(uint score) {
        m_TotalScore += score;
        m_StageScore[m_Stage] += score;
        UpdateScore();
    }

    public void AddScoreEffect(uint score) {
        AddScore(score);
        DisplayScoreText(""+score);
    }

    public void AddScoreEffect(uint score, bool ground_gem) { // Overload
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
    }


    public void SetStage(int stage) {
        m_Stage = stage;
    }

    public int GetStage() {
        return m_Stage;
    }

    public uint GetStageScore() {
        return m_StageScore[m_Stage];
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

    public void AddMiss() {
        m_TotalMiss++;
        m_StageMiss[m_Stage]++;
    }

    public byte GetTotalMiss() {
        return m_TotalMiss;
    }

    public byte GetStageMiss() {
        return m_StageMiss[m_Stage];
    }


    public void SetDifficulty(byte difficulty)
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
                m_Difficulty = 1;
                SetDifficultyText();
                break;
        }
    }

    public void WarningText() {
        StartCoroutine(WarningTextAnimation());
    }

    private IEnumerator WarningTextAnimation() {
        float value = 0f;
        float time = 0f;
        m_AudioWarning.Play();

        m_WarningUI.m_WarningPanelWhite.color = new Color(1f, 1f, 1f, 0f);
        m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, 0f, m_WarningUI.transform.localScale.z);
        m_WarningUI.gameObject.SetActive(true);

        while (value < 1f) { // Increase yScale
            m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, value, m_WarningUI.transform.localScale.z);
            value += Time.deltaTime * 4f;
            value = Mathf.Clamp01(value);
            yield return null;
        }
        value = 0f;

        while (value < 1f) { // White Blink Effect
            m_WarningUI.m_WarningPanelWhite.color = new Color(1f, 1f, 1f, 1f - value);
            value += Time.deltaTime * 3f;
            value = Mathf.Clamp01(value);
            yield return null;
        }
        value = 0f;

        while (time < 3f) {
            float alpha_gap = 0.4f;
            while (value < 1f) { // Text Effect
                m_WarningUI.m_WarningText.color = new Color(1f - value*alpha_gap, 1f - value*alpha_gap, 1f - value*alpha_gap, 1f);
                value += Time.deltaTime * 4f;
                value = Mathf.Clamp01(value);
                time += Time.deltaTime;
                yield return null;
            }
            value = 0f;
            yield return null;
        }
        value = 0f;

        while (value < 1f) { // Decrease yScale
            m_WarningUI.transform.localScale = new Vector3(m_WarningUI.transform.localScale.x, 1f - value, m_WarningUI.transform.localScale.z);
            value += Time.deltaTime * 4f;
            value = Mathf.Clamp01(value);
            yield return null;
        }
        value = 0f;

        m_WarningUI.gameObject.SetActive(false);
    }


    private void BulletEraseTimer() {
        if (m_BulletsEraseTimer > 0) {
            m_BulletsEraseTimer -= Time.deltaTime;
        }
        else {
            m_BulletsEraseTimer = 0f;
        }
    }

    public void BulletsToGems(float timer) {
        List<GameObject> bullet_list = new List<GameObject>();
        bullet_list.AddRange(GameObject.FindGameObjectsWithTag("EnemyBulletParent"));
        int index, num = 0, count = bullet_list.Count;

        while (count > 0) {
            index = Random.Range(0, count);
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
            enemy_bullet.Erase();
            bullet_list.RemoveAt(index);

            count--;
        }
        EraseBullets(timer);
    }

    public void EraseBullets(float timer) {
        if (m_BulletsEraseTimer < timer) {
            m_BulletsEraseTimer = timer;
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

    public uint GemsGround {
        get { return m_GemsGround; }
        set { m_GemsGround = value; }
    }

    public uint GemsAir {
        get { return m_GemsAir; }
        set { m_GemsAir = value; }
    }
}
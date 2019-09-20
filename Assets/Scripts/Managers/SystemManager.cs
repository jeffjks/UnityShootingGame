using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public static class Depth // Z Axis
{
    // Far
    public const sbyte ENEMY = 0; // Only Air Enemy
    public const sbyte EXPLOSION = -2; // Only Air Explosion
    public const sbyte OVERVIEW = -4;
    public const sbyte ITEMS = -4;
    public const sbyte TRANSITION = -5;
    public const sbyte PLAYER = -6; // Player, Laser
    public const sbyte PLAYER_MISSILE = -7;
    public const sbyte ENEMY_BULLET = -10;
    public const sbyte CAMERA = -24;
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
    public const ushort GEM_GROUND = 500;
    public const ushort GEM_AIR = 700;
    public const ushort POWERUP = 1000;
    public const ushort BOMB = 2500;
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
    [SerializeField] private Text m_ReplayText = null;
    [SerializeField] private Text m_MissNumberText = null;
    [SerializeField] private Text m_WarningText = null;
    [SerializeField] private GameObject m_Transition = null;
    [SerializeField] private AudioSource m_AudioWarning = null;
    [SerializeField] private AudioSource m_AudioStageClear = null;

    [HideInInspector] public StageManager m_StageManager;
    [HideInInspector] public Vector2 m_BackgroundCameraSize;
    [HideInInspector] public byte m_PlayState; // 0: 평소, 1: 보스/중간보스전, 2: 보스 클리어, 3: 점수 화면, 4: 다음 스테이지 전환중
    [HideInInspector] public int BulletsSortingLayer;
    [HideInInspector] public float m_BulletsEraseTimer;
    [HideInInspector] public byte m_Difficulty;
    [HideInInspector] public bool m_ReplayState;
    
    public bool m_DebugMod, m_InvincibleMod;
    public DebugDifficulty m_DebugDifficulty;
    
    private GameManager m_GameManager = null;
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    private List<ScreenEffectAnimation> m_TransitionList = new List<ScreenEffectAnimation>();
    private PlayerController m_PlayerController;
    private uint m_TotalScore = 0;
    private uint[] m_StageScore = new uint[5] {0, 0, 0, 0, 0};
    private uint m_GemsGround = 0, m_GemsAir = 0; // 점수가 아닌 먹은 개수
    private int m_Stage = 0;
    private byte m_TotalMiss = 0;
    private byte[] m_StageMiss = new byte[5] {0, 0, 0, 0, 0};

    private Sequence m_SequenceReplayText;

    public static SystemManager instance_sm = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;

        m_ReplayState = m_GameManager.m_ReplayState;

        Application.targetFrameRate = 60;

        if (instance_sm != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_sm = this;
        
        try {
            SetDifficulty(m_GameManager.m_Difficulty);
            AudioListener audioListener = gameObject.GetComponent<AudioListener>();
            audioListener.enabled = false;
        }
        catch (System.NullReferenceException) {
            SetDifficulty((byte) m_DebugDifficulty);
        }

        m_BackgroundCameraSize.x = m_BackgroundCamera.orthographicSize * 2 * ((float) Screen.width/(float) Screen.height); // 256/9 = 28.444..
        m_BackgroundCameraSize.y = m_BackgroundCamera.orthographicSize * 2; // 16
        
        DontDestroyOnLoad(gameObject);

        DOTween.SetTweensCapacity(512, 64);
    }

    void Start()
    {
        SetStageManager();
        m_PoolingManager = PoolingManager.instance_op;

        m_MissNumberText.text = "" + m_TotalMiss;

        if (m_ReplayState) {
            m_ReplayText.gameObject.SetActive(true);
            m_SequenceReplayText = DOTween.Sequence()
            .Append(m_ReplayText.DOFade(0f, 0.2f))
            .Append(m_ReplayText.DOFade(1f, 0.6f))
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        }

        CreateTransition();
        ScreenEffect(2);
        UpdateScore();
    }

    void Update()
    {
        m_BackgroundCamera.transform.position += m_StageManager.m_BackgroundVector*Time.deltaTime;

        BulletEraseTimer();
    }

    private void SetStageManager() {
        m_StageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManager>();
        m_Stage = m_StageManager.m_Stage;
    }

    public void SetPlayerManager() {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PlayerController = m_PlayerManager.m_Player.GetComponent<PlayerController>();
    }

    private void CreateTransition() {
        int r = 1;
        for(int i=0; i<8; i++) { // 가로 6개, 세로 8개. 12*16
            for(int j=0; j<6; j++) {
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
                    m_TransitionList[i].PlayTransition();
                }
                break;
            case 3: // FadeIn
                for (int i = 0; i < m_TransitionList.Count; i++) { // Fade In (End Overview)
                    m_TransitionList[i].gameObject.SetActive(true);
                    m_TransitionList[i].PlayFadeIn();
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
        m_PlayState = 2;
        m_PlayerController.EnableInvincible(5f);
        m_StageManager.StopMusic();
    }

    public IEnumerator StageClear() {
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
        SceneManager.LoadScene("Stage" + (m_Stage + 2));
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        m_BackgroundCamera.transform.position = new Vector3(0f, 40f, -24f);
        ScreenEffect(2); // Transition
        m_PlayState = 0;
        m_OverviewHandler.gameObject.SetActive(false);
        m_PlayerManager.PlayerControlable = true;
        m_PlayerController.EnableInvincible(3f);
        yield break;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetStageManager();
    }


    private void UpdateScore() {
        m_ScoreNuberText.text = "" + m_TotalScore;
    }

    public void AddScore(uint score) {
        m_TotalScore += score;
        m_StageScore[m_Stage] += score;
        UpdateScore();
    }

    public void AddScore(uint score, bool ground_gem) { // Overload
        m_TotalScore += score;
        m_StageScore[m_Stage] += score;
        if (ground_gem) {
            m_GemsGround++;
        }
        else {
            m_GemsAir++;
        }
        UpdateScore();
    }

    public int GetStage() {
        return m_Stage;
    }

    public uint GetStageScore() {
        return m_StageScore[m_Stage];
    }

    public void AddMiss() {
        m_TotalMiss++;
        m_StageMiss[m_Stage]++;
        m_MissNumberText.text = "" + m_TotalMiss;
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

    public IEnumerator WarningText() {
        byte time = 0;
        m_AudioWarning.Play();

        m_WarningText.gameObject.SetActive(true);
        while (time < 10) {
            m_WarningText.CrossFadeAlpha(1f, 0.4f, true);
            time++;
            yield return new WaitForSeconds(0.1f);
        }
        
        time = 0;
        yield return new WaitForSeconds(2f);

        while (time < 10) {
            m_WarningText.CrossFadeAlpha(0f, 0.4f, true);
            time++;
            yield return new WaitForSeconds(0.1f);
        }
        m_WarningText.gameObject.SetActive(false);
        yield break;
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
                        GameObject gem = m_PoolingManager.PopFromPool("ItemGemAir", PoolingParent.ITEM_GEM); // Gem 생성
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


    public uint GemsGround {
        get { return m_GemsGround; }
        set { m_GemsGround = value; }
    }

    public uint GemsAir {
        get { return m_GemsAir; }
        set { m_GemsAir = value; }
    }
}
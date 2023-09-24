using UnityEngine;
using System.Collections.Generic;
using System;

public class InGameDataManager : MonoBehaviour
{
    public RectTransform m_UICanvas;
    
    public event Action<long> Action_OnUpdateScore;
    public event Action<int, int> Action_OnUpdateBombNumber;
    
    private bool _destroySingleton;
    private long _totalScore;
    private int _bombNumber;
    private int _maxBombNumber;
    
    private readonly long[] _stageScore = {0, 0, 0, 0, 0};
    private readonly int[] _stageMiss = {0, 0, 0, 0, 0};
    private readonly Dictionary<ItemType, int>[] _itemCount = new Dictionary<ItemType, int>[5];
    
#if UNITY_EDITOR
    private PlayerUnit _debugPlayerUnit;
#endif
    
    public long TotalScore
    {
        get => _totalScore;
        private set
        {
            _totalScore = value;
            Action_OnUpdateScore?.Invoke(value);
        }
    }

    public ShipAttributes CurrentShipAttributes { get; private set; }
    
    public int TotalMiss { get; private set; }
    public int BombNumber
    {
        get => _bombNumber;
        set
        {
            _bombNumber = value;
            Action_OnUpdateBombNumber?.Invoke(value, MaxBombNumber);
        }
    }

    public long ElapsedTime { get; private set; }
    
    private int MaxBombNumber
    {
        get => _maxBombNumber;
        set
        {
            _maxBombNumber = value;
            Action_OnUpdateBombNumber?.Invoke(BombNumber, value);
        }
    }
    
    public static InGameDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        SystemManager.Action_OnQuitInGame += DestroySelf;

        for (var i = 0; i < _itemCount.Length; ++i)
        {
            _itemCount[i] = new Dictionary<ItemType, int>();
            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                _itemCount[i][itemType] = 0;
            }
        }

        if (DebugOption.SceneMode == 3)
        {
            TotalScore = GameManager.RandomTest(0, 10000);
            CurrentShipAttributes = new ShipAttributes();
            TotalMiss = GameManager.RandomTest(0, 50);
            ElapsedTime = DateTime.Now.Ticks;
        }
    }
    
    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        SystemManager.Action_OnQuitInGame -= DestroySelf;
    }

    private void Start()
    {
        TotalScore = 0;
        CurrentShipAttributes = PlayerManager.CurrentAttributes;
        MaxBombNumber = CurrentShipAttributes.GetAttributes(AttributeType.Bomb) + 2;
        InitBombNumber();
        
#if UNITY_EDITOR
        if (DebugOption.SceneMode == 1)
            _debugPlayerUnit = FindObjectOfType<PlayerUnit>();
#endif
    }

    public void AddScore(long score, bool effect, ItemType itemType)
    {
        AddScore(score, effect);

        _itemCount[SystemManager.Stage][itemType]++;
    }

    public void AddScore(long score, bool effect = false)
    {
        if (SystemManager.Stage == -1)
            return;
        TotalScore += score;
        _stageScore[SystemManager.Stage] += score;

        if (effect)
        {
            DisplayTextEffect(score);
        }
    }

    public void DisplayTextEffect(long score)
    {
        DisplayTextEffect(score.ToString());
    }

    public void DisplayTextEffect(string text, float timeScale = 1f)
    {
#if UNITY_EDITOR
        var playerPos = (DebugOption.SceneMode > 0) ? _debugPlayerUnit.transform.position : PlayerManager.GetPlayerPosition();
#else
        var playerPos = PlayerManager.GetPlayerPosition();
#endif
        var isTextOnRight = (playerPos.x <= 0f);

        playerPos += new Vector3(isTextOnRight ? 1f : -1f, 1f);
        playerPos.y = Mathf.Min(playerPos.y, -1f);
        var playerViewportPos = MainCamera.Instance.Camera.WorldToViewportPoint(playerPos);

        var uiCanvasSize = new Vector2(
            m_UICanvas.rect.width * m_UICanvas.lossyScale.x,
            m_UICanvas.rect.height * m_UICanvas.lossyScale.y
        );

        var leftBottom = new Vector2((Screen.width - uiCanvasSize.x) / 2f, (Screen.height - uiCanvasSize.y) / 2f);
        
        var pos = new Vector3(
            playerViewportPos.x * uiCanvasSize.x + leftBottom.x,
            playerViewportPos.y * uiCanvasSize.y + leftBottom.y,
            0f
            );
        
        GameObject obj = PoolingManager.PopFromPool("ScoreText", PoolingParent.ScoreText);
        obj.transform.SetParent(m_UICanvas);
        
        obj.SetActive(true);
        ScoreText scoreText = obj.GetComponent<ScoreText>();
        scoreText.OnStart(pos, text, timeScale, isTextOnRight);
    }

    public void InitBombNumber() {
        BombNumber = _maxBombNumber;
    }

    public bool AddBomb() {
        if (BombNumber < MaxBombNumber)
        {
            BombNumber++;
            return true;
        }
        return false;
    }

    public long GetCurrentStageScore() {
        if (SystemManager.Stage == -1)
            return -1;
        return _stageScore[SystemManager.Stage];
    }

    public void AddMiss() {
        if (SystemManager.Stage == -1)
            return;
        TotalMiss++;
        _stageMiss[SystemManager.Stage]++;
    }

    public int GetCurrentStageMiss() {
        if (SystemManager.Stage == -1)
            return -1;
        return _stageMiss[SystemManager.Stage];
    }

    public int GetItemCount(ItemType itemType)
    {
        if (_itemCount[SystemManager.Stage].TryGetValue(itemType, out int count))
        {
            return count;
        }

        return -1;
    }
    

    public void SaveElapsedTime() {
        ElapsedTime = DateTime.Now.Ticks;
    }

    private void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
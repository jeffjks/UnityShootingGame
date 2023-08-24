using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Random = System.Random;

public class InGameDataManager : MonoBehaviour
{
    public Transform m_InGameWorldCanvasTransform;
    
    public event Action<long> Action_OnUpdateScore;
    public event Action<int, int> Action_OnUpdateBombNumber;
    
    private long _totalScore;
    private int _bombNumber;
    private int _maxBombNumber;
    private long _elapsedTime;
    
    private readonly long[] _stageScore = {0, 0, 0, 0, 0};
    private readonly int[] _stageMiss = {0, 0, 0, 0, 0};
    private readonly Dictionary<ItemType, int> _itemCount = new();
    
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

    public long ElapsedTime
    {
        get => _elapsedTime;
        private set
        {
            _elapsedTime = value;
        }
    }
    
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
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        SystemManager.Action_OnQuitInGame += DestroySelf;
        
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            _itemCount[itemType] = 0;
        }

        if (DebugOption.SceneMode == 3)
        {
            TotalScore = UnityEngine.Random.Range(0, 10000);
            TotalMiss = UnityEngine.Random.Range(0, 50);
            ElapsedTime = DateTime.Now.Ticks;
        }
    }

    private void Start()
    {
        TotalScore = 0;
        CurrentShipAttributes = PlayerManager.CurrentAttributes;
        MaxBombNumber = CurrentShipAttributes.GetAttributes(AttributeType.Bomb) + 2;
        InitBombNumber();
    }

    public void AddScore(long score, bool effect, ItemType itemType)
    {
        AddScore(score, effect);

        _itemCount[itemType]++;
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

    public void DisplayTextEffect(string text, float timeScale = 1f) {
        Vector3 pos = PlayerManager.GetPlayerPosition();
        
        GameObject obj = PoolingManager.PopFromPool("ScoreText", PoolingParent.ScoreText);
        obj.transform.SetParent(m_InGameWorldCanvasTransform);
        bool dir = (pos.x > 0f);
        if (dir)
            pos.x += 1f;
        else
            pos.x -= 1f;
        pos.y += 1f;
        pos.z = Depth.SCORE_TEXT;
        
        obj.SetActive(true);
        ScoreText score_text = obj.GetComponent<ScoreText>();
        score_text.OnStart(pos, text, timeScale, dir);
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
        if (_itemCount.TryGetValue(itemType, out int count))
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
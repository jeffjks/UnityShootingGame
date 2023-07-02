using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class InGameDataManager : MonoBehaviour
{
    public event Action<long> Action_OnUpdateScore;
    public event Action<int, int> Action_OnUpdateBombNumber;

    private long _totalScore;
    private int _bombNumber;
    private int _maxBombNumber;
    private long m_ElapsedTime;
    
    private readonly long[] m_StageScore;
    private readonly int[] m_StageMiss;
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

    private InGameDataManager()
    {
        TotalScore = 0;
        m_StageScore = new long[] {0, 0, 0, 0, 0};
        TotalMiss = 0;
        m_StageMiss = new int[] {0, 0, 0, 0, 0};
        
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            _itemCount[itemType] = 0;
        }
    }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        SystemManager.Action_OnQuitInGame += DestroySelf;
    }

    private void Start()
    {
        TotalScore = 0;
        MaxBombNumber = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Bomb) + 2;
        InitBombNumber();
    }

    public void AddScore(long score, bool effect, ItemType itemType)
    {
        AddScore(score, effect);

        _itemCount[itemType]++;
    }

    public void AddScore(long score, bool effect = false) {
        TotalScore += score;
        m_StageScore[SystemManager.Stage] += score;

        if (effect)
        {
            DisplayTextEffect(score);
        }
    }

    public void DisplayTextEffect(long score)
    {
        DisplayTextEffect(score.ToString());
    }

    public void DisplayTextEffect(string text) {
        Vector3 pos = PlayerManager.GetPlayerPosition();
        
        GameObject obj = PoolingManager.PopFromPool("ScoreText", PoolingParent.ScoreText);
        bool dir = (pos.x > 0f);
        if (dir)
            pos.x += 1f;
        else
            pos.x -= 1f;
        pos.y += 1f;
        pos.z = Depth.SCORE_TEXT;
        
        obj.SetActive(true);
        ScoreText score_text = obj.GetComponent<ScoreText>();
        score_text.OnStart(pos, text, dir);
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
        return m_StageScore[SystemManager.Stage];
    }

    public void AddMiss() {
        TotalMiss++;
        m_StageMiss[SystemManager.Stage]++;
    }

    public int GetCurrentStageMiss() {
        return m_StageMiss[SystemManager.Stage];
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
        m_ElapsedTime = DateTime.Now.Ticks;
    }

    public long GetElapsedTime() {
        return m_ElapsedTime;
    }

    private void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
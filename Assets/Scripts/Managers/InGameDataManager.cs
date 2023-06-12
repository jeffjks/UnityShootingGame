using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class InGameDataManager : MonoBehaviour
{
    public Action<long> Action_OnUpdateScore;
    public Action<int> Action_OnUpdateBombNumber;

    public int BulletNumber { get; set; }
    public long TotalScore { get; private set; }
    public int TotalMiss { get; private set; }
    private readonly long[] m_StageScore;

    private int _bombNumber;
    private int _maxBombNumber;
    public int BombNumber
    {
        get => _bombNumber;
        set
        {
            _bombNumber = value;
            UpdateBombNumber();
        }
    }
    public int MaxBombNumber { private get; set; }
    
    private readonly int[] m_StageMiss;
    private long m_ElapsedTime;
    private readonly Dictionary<ItemType, int> _itemCount = new();
    
    public static InGameDataManager Instance { get; private set; }

    private InGameDataManager()
    {
        TotalScore = 0;
        m_StageScore = new long[5] {0, 0, 0, 0, 0};
        TotalMiss = 0;
        m_StageMiss = new int[5] {0, 0, 0, 0, 0};
        BulletNumber = 0;
        
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            _itemCount[itemType] = 0;
        }
    }

    void Awake()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        UpdateBombNumber();
        UpdateScore();
    }

    public void AddScore(long score, bool effect, ItemType itemType)
    {
        AddScore(score, effect);

        _itemCount[itemType]++;
    }

    public void AddScore(long score, bool effect = false) {
        TotalScore += score;
        m_StageScore[SystemManager.Stage] += score;
        UpdateScore();

        if (effect)
        {
            DisplayTextEffect(score);
        }
    }

    private void UpdateScore()
    {
        Action_OnUpdateScore?.Invoke(TotalScore);
    }

    public void DisplayTextEffect(long score)
    {
        DisplayTextEffect(score.ToString());
    }

    public void DisplayTextEffect(string text) {
        Vector3 pos = PlayerManager.instance_pm.m_PlayerController.transform.position;
        
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

    private void UpdateBombNumber()
    {
        Action_OnUpdateBombNumber?.Invoke(_bombNumber);
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
}
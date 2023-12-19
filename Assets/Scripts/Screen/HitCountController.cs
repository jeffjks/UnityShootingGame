using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Events;

public class HitCountController : MonoBehaviour
{
    public enum HitCountType
    {
        None = -1,
        Field,
        Boss
    }
    
    public enum HitCountState
    {
        Default,
        Decreasing,
        BreakDown,
        EndField
    }

    [SerializeField] private HitCountConstData m_HitCountConstData;
    public int CurrentHitCountBonusPercent { get; private set; } = 100;
    
    public static event UnityAction<int, bool> Action_OnUpdateHitCount;
    public static event UnityAction<HitCountState> Action_OnChangeHitCountState;
    public static event UnityAction<HitCountType> Action_OnChangeHitCountType;
    
    private int _hitCount;
    private int _hitCountDecreasingTimer;
    private int _hitCountLaserCounter;
    private HitCountState _currentHitCountState = HitCountState.Default;
    private HitCountType _currentHitCountType = HitCountType.None;

    public HitCountType CurrentHitCountType
    {
        get => _currentHitCountType;
        set
        {
            if (_currentHitCountType == value)
                return;
            _currentHitCountType = value;
            InitHitCount();
            Action_OnChangeHitCountType?.Invoke(_currentHitCountType);
        }
    }
    
    public static HitCountController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        //SystemManager.Action_OnNextStage += OnNextStage;
        SystemManager.Action_OnPlayStateChanged += EndFieldHitCount;
        SystemManager.Action_OnBossInteractable += OnBossInteractable;
        PlayerManager.Action_OnPlayerRevive += InitHitCount;
        PlayerManager.Action_OnPlayerDead += BreakDownHitCount;
        PlayerBombHandler.Action_OnBombUse += DecreaseHitCount;
    }
    
    private void OnDestroy()
    {
        Instance = null;
        
        //SystemManager.Action_OnNextStage -= OnNextStage;
        SystemManager.Action_OnPlayStateChanged -= EndFieldHitCount;
        SystemManager.Action_OnBossInteractable -= OnBossInteractable;
        PlayerManager.Action_OnPlayerRevive -= InitHitCount;
        PlayerManager.Action_OnPlayerDead -= BreakDownHitCount;
        PlayerBombHandler.Action_OnBombUse -= DecreaseHitCount;
        
        StopAllCoroutines();
    }

    private void Start()
    {
        StartCoroutine(HitCountDecreasingController());
    }

    private void Update()
    {
        if (_hitCountDecreasingTimer > 0)
            _hitCountDecreasingTimer -= 1;
        else if (_hitCountDecreasingTimer == 0)
            DecreaseHitCount();
    }

    public void AddHitCount(int value = 1)
    {
        if (!PlayerManager.IsPlayerAlive)
            return;
        if (PlayerBombHandler.IsBombInUse)
            return;
        if (_currentHitCountState == HitCountState.EndField)
            return;
        
        _hitCount += value;
        _hitCount = Mathf.Min(_hitCount, m_HitCountConstData.MaxHitCount);
        Action_OnUpdateHitCount?.Invoke(_hitCount, true);
        SetHitCountState(HitCountState.Default);
        UpdateHitCountBonus();
        _hitCountDecreasingTimer = m_HitCountConstData.HitCountDecreasingFrame;
    }

    private void SubtractHitCount(int value = 1)
    {
        if (!PlayerManager.IsPlayerAlive)
            return;
        if (_currentHitCountState == HitCountState.EndField)
            return;
        
        _hitCount -= value;
        _hitCount = Mathf.Max(_hitCount, m_HitCountConstData.MinHitCount);
        Action_OnUpdateHitCount?.Invoke(_hitCount, false);
        UpdateHitCountBonus();
    }

    private void InitHitCount()
    {
        _hitCount = 0;
        _hitCountDecreasingTimer = -1;
        _hitCountLaserCounter = 0;
        Action_OnUpdateHitCount?.Invoke(_hitCount, true);
        UpdateHitCountBonus();
        SetHitCountState(HitCountState.Default);
    }

    private void BreakDownHitCount()
    {
        _hitCount = 0;
        _hitCountDecreasingTimer = -1;
        _hitCountLaserCounter = 0;
        SetHitCountState(HitCountState.BreakDown);
        UpdateHitCountBonus();
    }

    private void EndFieldHitCount(PlayState playState)
    {
        if (playState is PlayState.OnBoss or PlayState.OnBossCleared)
            SetHitCountState(HitCountState.EndField);
    }

    private void SetHitCountState(HitCountState hitCountState)
    {
        if (_currentHitCountState == hitCountState)
            return;
        if (_currentHitCountState == HitCountState.EndField && hitCountState != HitCountState.Default)
            return;
        _currentHitCountState = hitCountState;
        
        Action_OnChangeHitCountState?.Invoke(_currentHitCountState);
    }

    private void OnBossInteractable()
    {
        CurrentHitCountType = HitCountType.Boss;
    }

    private void OnNextStage(bool hasNextStage)
    {
        InitHitCount();
    }

    public int HitCountLaserCounter
    {
        get => _hitCountLaserCounter;
        set
        {
            if (!PlayerManager.IsPlayerAlive)
                return;
            if (PlayerBombHandler.IsBombInUse)
                return;
            
            _hitCountLaserCounter = value;
            _hitCountDecreasingTimer = m_HitCountConstData.HitCountDecreasingFrame;
            
            if (_hitCountLaserCounter >= m_HitCountConstData.HitCountLaserMaxCount)
            {
                _hitCountLaserCounter %= m_HitCountConstData.HitCountLaserMaxCount;
                AddHitCount();
            }
        }
    }

    private void DecreaseHitCount()
    {
        _hitCountDecreasingTimer = -1;
        SetHitCountState(HitCountState.Decreasing);
    }

    private IEnumerator HitCountDecreasingController()
    {
        while (true)
        {
            while (_hitCountDecreasingTimer != -1)
                yield return new WaitForFrames(1);
            SubtractHitCount();
            yield return new WaitForFrames(m_HitCountConstData.HitCountDecreasingPeriodFrame);
        }
    }

    private void UpdateHitCountBonus()
    {
        var lastBonusPercent = 100;
        foreach (var hitCountBonus in m_HitCountConstData.hitCountBonus)
        {
            if (_hitCount < hitCountBonus.hitCount)
                break;

            lastBonusPercent = hitCountBonus.bonusPercent;
        }

        CurrentHitCountBonusPercent = lastBonusPercent;
    }
}
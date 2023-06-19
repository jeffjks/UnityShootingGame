using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class OverviewFlow
{
    public readonly float waitTime;
    public Action Action_OnExecute;

    public OverviewFlow(float waitTime)
    {
        this.waitTime = waitTime;
    }
}

public class OverviewFlowBuilder
{
    private readonly OverviewFlow _overviewFlow;

    public OverviewFlowBuilder(float waitTime)
    {
        _overviewFlow = new OverviewFlow(waitTime);
    }

    public OverviewFlowBuilder AddAction(Action action)
    {
        _overviewFlow.Action_OnExecute += action;
        return this;
    }

    public OverviewFlow Build()
    {
        return _overviewFlow;
    }
}

public class OverviewHandler : MonoBehaviour
{
    public IngameInputController m_InGameInputController;
    public ItemDatas m_GemGroundData;
    public ItemDatas m_GemAirData;
    
    [SerializeField] private GameObject[] m_OverviewContents;

    [SerializeField] private TextMeshProUGUI m_GemGroundNumberText;
    [SerializeField] private TextMeshProUGUI m_GemGroundScoreText;
    [SerializeField] private TextMeshProUGUI m_GemAirNumberText;
    [SerializeField] private TextMeshProUGUI m_GemAirScoreText;
    [SerializeField] private TextMeshProUGUI m_RemainText;
    [SerializeField] private TextMeshProUGUI m_TotalStageText;
    [SerializeField] private TextMeshProUGUI m_MissText;
    [SerializeField] private TextMeshProUGUI m_MissBonusScaleText;
    
    public event Action<long> Action_OnUpdateFinalBonus;

    private readonly Queue<OverviewFlow> _overviewFlowQueue = new();
    private bool _inputSkip;
    
    private int _missBonusPercent;
    private long _finalBonusScore;

    public long FinalBonusScore
    {
        get => _finalBonusScore;
        set
        {
            _finalBonusScore = value;
            Action_OnUpdateFinalBonus?.Invoke(_finalBonusScore);
        }
    }

    private IEnumerator _calculateFinalBonusCoroutine;

    private readonly int[] _missBonusPercents = { 50, 30, 10 };

    private void OnEnable()
    {
        m_InGameInputController.Action_OnFireInput += SkipOverviewPhase;
        m_InGameInputController.Action_OnBombInput += SkipOverviewPhase;
        StartOverview();
    }

    private void OnDisable()
    {
        m_InGameInputController.Action_OnFireInput -= SkipOverviewPhase;
        m_InGameInputController.Action_OnBombInput -= SkipOverviewPhase;
    }

    private void StartOverview() {
        Init();

        EnqueueOverviewFlow(new OverviewFlowBuilder(1f) // Gem 점수
            .AddAction(OverviewFlowAction1)
            .Build());

        EnqueueOverviewFlow(new OverviewFlowBuilder(1f) // 미스 횟수
            .AddAction(OverviewFlowAction2)
            .Build());

        EnqueueOverviewFlow(new OverviewFlowBuilder(1f) // 최종 보너스
            .AddAction(OverviewFlowAction3)
            .Build());

        EnqueueOverviewFlow(new OverviewFlowBuilder(-1f) // 최종 보너스 계산 시작
            .AddAction(OverviewFlowAction4)
            .Build());

        EnqueueOverviewFlow(new OverviewFlowBuilder(-1f) // 최종 보너스 계산중
            .AddAction(OverviewFlowAction5)
            .Build());

        EnqueueOverviewFlow(new OverviewFlowBuilder(-1f) // 최종 보너스 계산 완료
            .AddAction(OverviewFlowAction6)
            .Build());

        StartCoroutine(OverviewFlow());
    }

    private void EnqueueOverviewFlow(OverviewFlow item)
    {
        _overviewFlowQueue.Enqueue(item);
    }

    private void OverviewFlowAction1()
    {
        m_OverviewContents[0].SetActive(true);
        m_OverviewContents[1].SetActive(true);
        m_OverviewContents[2].SetActive(true);
        m_OverviewContents[3].SetActive(true);
    }

    private void OverviewFlowAction2()
    {
        m_OverviewContents[4].SetActive(true);
    }

    private void OverviewFlowAction3()
    {
        m_OverviewContents[5].SetActive(true);
    }

    private void OverviewFlowAction4()
    {
        _calculateFinalBonusCoroutine = CalculateFinalBonus();
        StartCoroutine(_calculateFinalBonusCoroutine);
    }

    private void OverviewFlowAction5()
    {
        if (_calculateFinalBonusCoroutine != null)
        {
            StopCoroutine(_calculateFinalBonusCoroutine);
        }
        InGameDataManager.Instance.AddScore(_finalBonusScore);
        FinalBonusScore = 0;
    }

    private void OverviewFlowAction6()
    {
        GoToNextStage();
    }

    private IEnumerator CalculateFinalBonus()
    {
        for (int i = 10; i >= 0; i--) {
            long target_score = (long) Mathf.Pow(7, i);
            if (FinalBonusScore >= target_score) {
                FinalBonusScore -= target_score;
                InGameDataManager.Instance.AddScore(target_score);
                yield return null;
            }
        }
    }

    private void Init() {
        SetOverviewText();
        
        foreach (var overviewContent in m_OverviewContents)
        {
            overviewContent.SetActive(false);
        }
        //m_OverviewTimer = 0f;
        //m_OverviewPhase = 0;
    }

    private void SkipOverviewPhase()
    {
        _inputSkip = true;
        /*
        switch(m_OverviewPhase) {
            case 4:
                GoToNextOverviewPhase();
                break;
            case 5:
                m_SystemManager.AddScore(FinalBonusScore);
                FinalBonusScore = 0;
                m_OverviewTimer = 0f;
                UpdateFinalBonusScore();
                break;
            case 6:
                GoToNextOverviewPhase();
                GoToNextStage(); // 6일때 버튼 클릭시 다음 스테이지
                break;
            default:
                m_OverviewTimer = 90f;
                break;
        }*/
    }

    private IEnumerator OverviewFlow()
    {
        while (_overviewFlowQueue.Count > 0)
        {
            var current = _overviewFlowQueue.Dequeue();
            var startTime = Time.time;
            var waitTime = current.waitTime;

            while (waitTime >= 0 && Time.time < startTime + waitTime)
            {
                if (_inputSkip)
                {
                    _inputSkip = false;
                    break;
                }
                yield return null;
            }
            current.Action_OnExecute?.Invoke();
        }
    }

    void LateUpdate()
    {   /*
        //if (m_OverviewPhase < 4) // 최종 점수 화면 이전에는 점수판 자동 진행
                                 // //m_OverviewTimer += Time.deltaTime*60f;
            
        if (m_OverviewPhase == 0) { // 첫 화면
            if (m_OverviewTimer > 60f) {
                GoToNextOverviewPhase();
            }
        }
        else if (m_OverviewPhase == 1) { // Gem 점수
            if (m_OverviewTimer > 60f) {
                GoToNextOverviewPhase();
                foreach (var obj in _overviewContents[0])
                {
                    obj.SetActive(true);
                }
            }
        }
        else if (m_OverviewPhase == 2) { // 미스 횟수
            if (m_OverviewTimer > 60f) {
                GoToNextOverviewPhase();
                m_OverviewContentsMissBonus.SetActive(true);
            }
        }
        else if (m_OverviewPhase == 3) { // 최종 보너스
            if (m_OverviewTimer > 90f) {
                GoToNextOverviewPhase();
                m_OverviewContentsFinalBonus.SetActive(true);
            }
        }
        else if (m_OverviewPhase == 4) { // 최종 보너스가 0일 경우 합산 패스
            if (FinalBonusScore == 0) {
                GoToNextOverviewPhase();
            }
        }
        else if (m_OverviewPhase == 5) { // 최종 보너스 합산
            for (sbyte i = 10; i >= 0; i--) {
                long target_score = (long) Mathf.Pow(7f, i);
                if (FinalBonusScore >= target_score) {
                    FinalBonusScore -= target_score;
                    m_SystemManager.AddScore(target_score);
                    UpdateFinalBonusScore();
                    break;
                }
            }
            if (FinalBonusScore == 0) {
                GoToNextOverviewPhase();
            }
        }*/
    }

    private void GoToNextOverviewPhase() {
        //m_OverviewTimer = 0f;
        //m_OverviewPhase++;
    }
    
    private void GoToNextStage() {
        SystemManager.Instance.StartNextStageCoroutine();
    }

    private void SetOverviewText()
    {
        int gemGroundCount = InGameDataManager.Instance.GetItemCount(ItemType.GemGround);
        long gemGroundScore = m_GemGroundData.itemScore * gemGroundCount;
        int gemAirCount = InGameDataManager.Instance.GetItemCount(ItemType.GemAir);
        long gemAirScore = m_GemAirData.itemScore * gemAirCount;
        int stageMiss = InGameDataManager.Instance.GetCurrentStageMiss();
        long stageScore = InGameDataManager.Instance.GetCurrentStageScore();
        
        if (stageMiss < _missBonusPercents.Length)
        {
            _missBonusPercent = _missBonusPercents[stageMiss];
        }
        else
        {
            _missBonusPercent = 0;
        }
        
        m_GemGroundNumberText.SetText($"X {gemGroundCount}");
        m_GemGroundScoreText.SetText($"{gemGroundScore}");
        m_GemAirNumberText.SetText($"X {gemAirCount}");
        m_GemAirScoreText.SetText($"{gemAirScore}");
        m_RemainText.SetText($"{stageScore - gemGroundScore - gemAirScore}");
        m_TotalStageText.SetText($"{stageScore}");
        m_MissText.SetText($"{stageMiss}");
        m_MissBonusScaleText.SetText($"[ + {_missBonusPercent}% ]");

        FinalBonusScore = stageScore * _missBonusPercent / 100;

        /*
        ground_gem[0].text = "X " + m_SystemManager.GemsGround;
        ground_gem[1].text = "" + ground_gem_score;
        air_gem[0].text = "X " + m_SystemManager.GemsAir;
        air_gem[1].text = "" + air_gem_score;
        remains_score[1].text = "" + (stage_score - ground_gem_score - air_gem_score);
        total_stage_score[1].text = "" + stage_score;
        miss[0].text = stage_miss + " Miss";
        miss[1].text = bonus_scale;*/
    }
}

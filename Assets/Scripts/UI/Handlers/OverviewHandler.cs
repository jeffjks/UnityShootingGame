using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OverviewFlow
{
    public readonly float waitTime;
    public event UnityAction Action_OnExecute;

    public OverviewFlow(float waitTime)
    {
        this.waitTime = waitTime;
    }

    public void Execute()
    {
        Action_OnExecute?.Invoke();
    }
}

public class OverviewFlowBuilder
{
    private readonly OverviewFlow _overviewFlow;

    public OverviewFlowBuilder(float waitTime)
    {
        _overviewFlow = new OverviewFlow(waitTime);
    }

    public OverviewFlowBuilder AddAction(UnityAction action)
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
    public ItemInfoDatas m_GemGroundData;
    public ItemInfoDatas m_GemAirData;
    [SerializeField] private int[] _missBonusPercents;
    
    [SerializeField] private GameObject[] m_OverviewContents;

    [SerializeField] private TextMeshProUGUI m_GemGroundNumberText;
    [SerializeField] private TextMeshProUGUI m_GemGroundScoreText;
    [SerializeField] private TextMeshProUGUI m_GemAirNumberText;
    [SerializeField] private TextMeshProUGUI m_GemAirScoreText;
    [SerializeField] private TextMeshProUGUI m_RemainText;
    [SerializeField] private TextMeshProUGUI m_GemTotalScoreText;
    [SerializeField] private TextMeshProUGUI m_MissText;
    [SerializeField] private TextMeshProUGUI m_MissBonusScaleText;
    [SerializeField] private TextMeshProUGUI m_FinalBonusText;

    private readonly Queue<OverviewFlow> _overviewFlowQueue = new();
    private bool _inputSkip;
    private long _finalBonusScore;

    private long FinalBonusScore
    {
        get => _finalBonusScore;
        set
        {
            _finalBonusScore = value;
            m_FinalBonusText.SetText($"{_finalBonusScore}");
        }
    }

    private IEnumerator _calculateFinalBonusCoroutine;

    private void Awake()
    {
        InGameInputController.Action_OnFireInput += SkipOverviewPhase;
    }

    private void OnDestroy()
    {
        InGameInputController.Action_OnFireInput -= SkipOverviewPhase;
    }

    private void OnEnable()
    {
        StartOverview();
    }

    private void OnDisable()
    {
        _overviewFlowQueue.Clear();
        
        foreach (var overviewContent in m_OverviewContents)
        {
            overviewContent.SetActive(false);
        }
    }

    private void StartOverview() {
        SetOverviewText();

        EnqueueOverviewFlow(new OverviewFlowBuilder(2f) // Gem 점수
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
    }

    private void OverviewFlowAction2()
    {
        m_OverviewContents[3].SetActive(true);
    }

    private void OverviewFlowAction3()
    {
        m_OverviewContents[4].SetActive(true);
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
        InGameDataManager.Instance.AddScore(FinalBonusScore, false, false);
        FinalBonusScore = 0;
    }

    private void OverviewFlowAction6()
    {
        GoToNextStage();
    }

    private IEnumerator CalculateFinalBonus()
    {
        const long limitDelta = 25000;
        var target_score = FinalBonusScore <= limitDelta ? 
            (long) Mathf.Sqrt(FinalBonusScore) : FinalBonusScore / (long) Mathf.Sqrt(limitDelta);
        while (FinalBonusScore >= target_score)
        {
            FinalBonusScore -= target_score;
            InGameDataManager.Instance.AddScore(target_score, false, false);
            yield return new WaitForFrames(1);
        }
    }

    private void SkipOverviewPhase(bool isPressed)
    {
        _inputSkip = isPressed;
    }

    private IEnumerator OverviewFlow()
    {
        while (_overviewFlowQueue.Count > 0)
        {
            var current = _overviewFlowQueue.Dequeue();
            var startTime = Time.time;
            var waitTime = current.waitTime;

            while (true)
            {
                if (_inputSkip)
                {
                    _inputSkip = false;
                    break;
                }

                if (Time.time >= startTime + waitTime && waitTime >= 0f)
                    break;
                yield return new WaitForFrames(1);
            }
            current.Execute();
        }
    }

    /*
    void LateUpdate()
    {   
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
        }
    }

    private void GoToNextOverviewPhase() {
        //m_OverviewTimer = 0f;
        //m_OverviewPhase++;
    }
    */
    
    private void GoToNextStage() {
        SystemManager.Instance.StartNextStageCoroutine();
    }

    private void SetOverviewText()
    {
        var gemGroundCount = InGameDataManager.Instance.GetItemCount(ItemType.GemGround);
        var gemGroundScore = m_GemGroundData.itemScore * gemGroundCount;
        var gemAirCount = InGameDataManager.Instance.GetItemCount(ItemType.GemAir);
        var gemAirScore = m_GemAirData.itemScore * gemAirCount;
        var stageMiss = InGameDataManager.Instance.GetCurrentStageMiss();
        var gemTotalScore = gemGroundScore + gemAirScore;
        //var currentStageScore = InGameDataManager.Instance.GetCurrentStageScore();

        var missBonusPercent = (stageMiss < _missBonusPercents.Length) ? _missBonusPercents[stageMiss] : 100;
        var missBonusScale = missBonusPercent / 100;
        
        m_GemGroundNumberText.SetText($"X {gemGroundCount}");
        m_GemGroundScoreText.SetText($"{gemGroundScore}");
        m_GemAirNumberText.SetText($"X {gemAirCount}");
        m_GemAirScoreText.SetText($"{gemAirScore}");
        //m_RemainText.SetText($"{gemTotalScore - gemGroundScore - gemAirScore}");
        m_GemTotalScoreText.SetText($"{gemTotalScore}");
        m_MissText.SetText($"{stageMiss} Miss");
        m_MissBonusScaleText.SetText($"[ x {missBonusScale} ]");
        
        FinalBonusScore = gemTotalScore * missBonusScale;
    }
}

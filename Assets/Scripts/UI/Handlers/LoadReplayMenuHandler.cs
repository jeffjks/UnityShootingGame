using System;
using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadReplayMenuHandler : MenuHandler
{
    public GameObject m_ReplaySlotPanel;
    private readonly ReplayManager.ReplayInfo[] _replayInfos = new ReplayManager.ReplayInfo[MAX_REPLAY_NUMBER];
    private string _replayDirectory;
    private const int MAX_REPLAY_NUMBER = 5;
    private int _currentSelectedSlot;

    private CanvasGroup[] _canvasGroups;
    private ButtonStyling[] _buttonStylingArray;
    private Button[] _buttons;
    private TextMeshProUGUI[] _buttonTexts;

    private void Awake()
    {
        _replayDirectory = $"{Application.dataPath}/";
        _canvasGroups = m_ReplaySlotPanel.GetComponentsInChildren<CanvasGroup>();
        _buttonStylingArray = m_ReplaySlotPanel.GetComponentsInChildren<ButtonStyling>();
        _buttons = m_ReplaySlotPanel.GetComponentsInChildren<Button>();
        _buttonTexts = m_ReplaySlotPanel.GetComponentsInChildren<TextMeshProUGUI>();
        
#if UNITY_EDITOR
        // Test Code
        //_currentSelectedSlot = -1;
        //Confirm();
#endif
    }

    protected override void Init()
    {
        for (var i = 0; i < MAX_REPLAY_NUMBER; ++i)
        {
            var filePath = $"{_replayDirectory}replay{i}.rep";
            if (!File.Exists(filePath))
            {
                _buttonStylingArray[i].m_NativeText = "빈 슬롯";
                _buttonTexts[i].SetText("Empty Slot");
                _canvasGroups[i].interactable = false;
                _replayInfos[i] = null;
                continue;
            }
            var fileStream = new FileStream(filePath, FileMode.Open);
            _replayInfos[i] = ReplayManager.ReadBinaryHeader(fileStream);
            var dateTimeString = new DateTime(_replayInfos[i].m_DateTime).ToString("yyyy-MM-dd-HH:mm");
            _buttonStylingArray[i].m_NativeText = dateTimeString;
            _buttonTexts[i].SetText(dateTimeString);
            fileStream.Close();
        }
    }

    public void PlayReplaySlot1()
    {
        _currentSelectedSlot = 0;
        Confirm();
    }

    public void PlayReplaySlot2()
    {
        _currentSelectedSlot = 1;
        Confirm();
    }

    public void PlayReplaySlot3()
    {
        _currentSelectedSlot = 2;
        Confirm();
    }

    public void PlayReplaySlot4()
    {
        _currentSelectedSlot = 3;
        Confirm();
    }

    public void PlayReplaySlot5()
    {
        _currentSelectedSlot = 4;
        Confirm();
    }

    public void Confirm()
    {
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
        EventSystem.current.currentInputModule.enabled = false;
        FadeScreenService.ScreenFadeOut(2f, StartReplay);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
        CriticalStateSystem.SetCriticalState(120);
    }

    private void StartReplay()
    {
        var replayInfo = GetReplayInfo();

        if (replayInfo.m_Version != Application.version)
        {
            // TODO. 버전이 다름
        }
        
        PlayerManager.CurrentAttributes = replayInfo.m_Attributes;
        SystemManager.SetDifficulty(replayInfo.m_Difficulty);
        SystemManager.Instance.StartStage(replayInfo.m_Stage, replayInfo.m_Seed);
        
        _previousMenuStack.Clear();
    }

    private ReplayManager.ReplayInfo GetReplayInfo()
    {
        ReplayManager.ReplayInfo replayInfo;
        
        if (_currentSelectedSlot == -1)
        {
            ReplayManager.ReplayFilePath = $"{_replayDirectory}replayTemp.rep";
            var fileStream = new FileStream(ReplayManager.ReplayFilePath, FileMode.Open);
            replayInfo = ReplayManager.ReadBinaryHeader(fileStream);
            fileStream.Close();
        }
        else
        {
            ReplayManager.ReplayFilePath = $"{_replayDirectory}replay{_currentSelectedSlot}.rep";
            replayInfo = _replayInfos[_currentSelectedSlot];
        }

        return replayInfo;
    }
}
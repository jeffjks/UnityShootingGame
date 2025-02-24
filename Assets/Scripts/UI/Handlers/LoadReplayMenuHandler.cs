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
    public ReplayVersionDatas m_ReplayVersionData;
    public PopupMenuHandler m_PopupMenuHandler;
    
    private const int MAX_REPLAY_NUMBER = 5;
    private ReplayManager.ReplayInfo _tempReplayInfo = null;
    private readonly ReplayManager.ReplayInfo[] _replayInfos = new ReplayManager.ReplayInfo[MAX_REPLAY_NUMBER];
    private int _currentSelectedSlot;

    private CanvasGroup[] _canvasGroups;
    private ButtonStyling[] _buttonStylingArray;
    private ColorTintButton[] _buttons;
    private TextMeshProUGUI[] _buttonTexts;
    private readonly ColorBlock _colorRed = new ()
    {
        normalColor = Color.red,
        highlightedColor = Color.red,
        selectedColor = Color.red,
        pressedColor = Color.red,
        colorMultiplier = 1f,
        fadeDuration = 0.1f
    };

    private ReplayManager.ReplayInfo CurrentReplayInfo => (_currentSelectedSlot == -1) ? _tempReplayInfo : _replayInfos[_currentSelectedSlot];
    
    private void Awake()
    {
        _canvasGroups = m_ReplaySlotPanel.GetComponentsInChildren<CanvasGroup>();
        _buttonStylingArray = m_ReplaySlotPanel.GetComponentsInChildren<ButtonStyling>();
        _buttons = m_ReplaySlotPanel.GetComponentsInChildren<ColorTintButton>();
        _buttonTexts = m_ReplaySlotPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    protected override void Init()
    {
        _tempReplayInfo = ReplayFileController.ReadReplayHeader(-1, out var tempResult);
        
        for (var i = 0; i < MAX_REPLAY_NUMBER; ++i)
        {
            _buttons[i].ResetButtonTextColor();

            _replayInfos[i] = ReplayFileController.ReadReplayHeader(i, out var result);
            
            if (result == ReplayFileController.ErrorCode.Error)
            {
                _buttonStylingArray[i].m_NativeText = "파일 오류";
                _buttonTexts[i].SetText("File Error");
                _buttonStylingArray[i].SetText();
                _buttons[i].ButtonTextColor = _colorRed;
                continue;
            }
            if (result == ReplayFileController.ErrorCode.NoFile)
            {
                _buttonStylingArray[i].m_NativeText = "빈 슬롯";
                _buttonTexts[i].SetText("Empty Slot");
                _buttonStylingArray[i].SetText();
                _canvasGroups[i].interactable = false;
                continue;
            }
            
            var dateTimeString = new DateTime(_replayInfos[i].m_DateTime).ToString("yyyy-MM-dd-HH:mm");
            _buttonStylingArray[i].m_NativeText = dateTimeString;
            _buttonStylingArray[i].SetText();
            _buttonTexts[i].SetText(dateTimeString);
            if (_replayInfos[i].m_Version != m_ReplayVersionData.replayVersion)
            {
                _buttons[i].ButtonTextColor = _colorRed;
            }
        }

#if UNITY_EDITOR
        StartCoroutine(LoadTempReplayFile());
#endif
    }

#if UNITY_EDITOR
    private IEnumerator LoadTempReplayFile()
    {
        yield return null;
        if (DebugOption.LoadTempReplayFile)
            PlayReplaySlot(-1);
    }
#endif

    public void PlayReplaySlot(int slot)
    {
        _currentSelectedSlot = slot;
        Confirm();
    }

    public void Confirm()
    {
        if (!CheckReplayAvailable())
        {
            AudioService.PlaySound("ConfirmUI");
            return;
        }
        
        CriticalStateSystem.SetCriticalState(120);
        
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
        EventSystem.current.currentInputModule.enabled = false;
        FadeScreenService.ScreenFadeOut(2f, StartReplay);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    private bool CheckReplayAvailable()
    {
        try
        {
            if (CurrentReplayInfo.m_Version != m_ReplayVersionData.replayVersion)
            {
                PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
                    () => _isActive = true,
                    null,
                    "리플레이 버전이 맞지 않아 리플레이를 실행할 수 없습니다.",
                    "This replay can't be viewed as the replay version is different."
                ));
                m_PopupMenuHandler.m_ButtonNegative.gameObject.SetActive(false);
                return false;
            }
        }
        catch (Exception e)
        {
            PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
                () => _isActive = true,
                null,
                $"오류가 발생하여 리플레이를 실행할 수 없습니다.",
                $"Error has occured while reading replay."
            ));
            m_PopupMenuHandler.m_ButtonNegative.gameObject.SetActive(false);
            Debug.LogError(e);
            return false;
        }

        return true;
    }

    private void StartReplay()
    {
        var replayInfo = CurrentReplayInfo;

        ReplayManager.CurrentReplaySlot = _currentSelectedSlot;
        PlayerManager.CurrentAttributes = replayInfo.m_Attributes;
        SystemManager.TrainingInfo = replayInfo.m_TrainingInfo;
        SystemManager.SetGameMode(replayInfo.m_GameMode);
        SystemManager.SetDifficulty(replayInfo.m_Difficulty);
        SystemManager.StartStage(replayInfo.m_TrainingInfo.stage, replayInfo.m_Seed);
        
        _previousMenuStack.Clear();
    }
}
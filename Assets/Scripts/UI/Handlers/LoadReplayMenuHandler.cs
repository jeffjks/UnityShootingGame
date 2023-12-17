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
    
    private readonly ReplayManager.ReplayInfo[] _replayInfos = new ReplayManager.ReplayInfo[MAX_REPLAY_NUMBER];
    private const int MAX_REPLAY_NUMBER = 5;
    private int _currentSelectedSlot;

    private CanvasGroup[] _canvasGroups;
    private ButtonStyling[] _buttonStylingArray;
    private ColorTintButton[] _buttons;
    private TextMeshProUGUI[] _buttonTexts;

    private void Awake()
    {
        _canvasGroups = m_ReplaySlotPanel.GetComponentsInChildren<CanvasGroup>();
        _buttonStylingArray = m_ReplaySlotPanel.GetComponentsInChildren<ButtonStyling>();
        _buttons = m_ReplaySlotPanel.GetComponentsInChildren<ColorTintButton>();
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
#if UNITY_EDITOR
            if (i == 0)
                _replayInfos[0] = ReplayFileController.ReadReplayHeader(-1);
            else
                _replayInfos[i] = ReplayFileController.ReadReplayHeader(i);
#else
            _replayInfos[i] = ReplayFileController.ReadReplayHeader(i);
#endif
            if (_replayInfos[i].IsDefault())
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
                _buttons[i].ButtonTextColor = new ColorBlock()
                {
                    normalColor = Color.red,
                    highlightedColor = Color.red,
                    selectedColor = Color.red,
                    pressedColor = Color.red,
                    colorMultiplier = 1f,
                    fadeDuration = 0.1f
                };
            }
            else
            {
                _buttons[i].ResetButtonTextColor();
            }
        }
    }

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
        if (_replayInfos[_currentSelectedSlot].m_Version != m_ReplayVersionData.replayVersion)
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

        return true;
    }

    private void StartReplay()
    {
        var replayInfo = _replayInfos[_currentSelectedSlot];

        ReplayManager.CurrentReplaySlot = _currentSelectedSlot;
        PlayerManager.CurrentAttributes = replayInfo.m_Attributes;
        SystemManager.SetDifficulty(replayInfo.m_Difficulty);
        SystemManager.Instance.StartStage(replayInfo.m_Stage, replayInfo.m_Seed);
        
        _previousMenuStack.Clear();
    }
}
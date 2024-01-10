using System;
using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveReplayMenuHandler : MenuHandler
{
    public GameObject m_ReplaySlotPanel;
    public ReplayVersionDatas m_ReplayVersionData;
    public PopupMenuHandler m_PopupMenuHandler;
    public TextErrorMessage m_TextErrorMessage;
    public CanvasGroup m_CanvasGroup;
    
    private readonly ReplayManager.ReplayInfo[] _replayInfos = new ReplayManager.ReplayInfo[MAX_REPLAY_NUMBER];
    private const int MAX_REPLAY_NUMBER = 5;
    private string _currentReplayDateTime;

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

    private void Awake()
    {
        _canvasGroups = m_ReplaySlotPanel.GetComponentsInChildren<CanvasGroup>();
        _buttonStylingArray = m_ReplaySlotPanel.GetComponentsInChildren<ButtonStyling>();
        _buttons = m_ReplaySlotPanel.GetComponentsInChildren<ColorTintButton>();
        _buttonTexts = m_ReplaySlotPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    protected override void Init()
    {
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
    }

    public void SaveReplaySlot(int slot)
    {
        SaveReplay(slot);
    }

    public override void Back() { }

    public void Cancel()
    {
        AudioService.PlaySound("CancelUI");
        LeaveMenu();
    }

    private void SaveReplay(int slot)
    {
        var oldFilePath = ReplayFileController.GetReplayFilePath();
        var newFilePath = ReplayFileController.GetReplayFilePath(slot);
        
        if (!File.Exists(oldFilePath))
        {
            m_TextErrorMessage.DisplayText("FileLoadException");
            AudioService.PlaySound("CancelUI");
            return;
        }
        
        if (File.Exists(newFilePath))
        {
            PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
                () => CopyAndDelete(oldFilePath, newFilePath),
                null,
                "해당 슬롯에 리플레이 파일이 이미 존재합니다. 덮어쓰시겠습니까?",
                "Replay file already exists in this slot. Overwrite it?"
            ));
        }
        else
        {
            CopyAndDelete(oldFilePath, newFilePath);
        }
        
        _buttonTexts[slot].SetText(_currentReplayDateTime);
        
        Debug.Log($"리플레이 파일을 성공적으로 저장하였습니다: {newFilePath}");
    }

    private void CopyAndDelete(string oldFilePath, string newFilePath)
    {
        File.Copy(oldFilePath, newFilePath, true);
        File.Delete(oldFilePath);
        AudioService.PlaySound("SallyUI");
        LeaveMenu();
    }

    private void LeaveMenu()
    {
        EventSystem.current.sendNavigationEvents = false;
        m_CanvasGroup.interactable = false;
        
        StartCoroutine(ReturnToMainMenu());
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        FadeScreenService.ScreenFadeOut(2f);
        yield return new WaitForSeconds(3f);

        SystemManager.QuitGame(null);
    }
}
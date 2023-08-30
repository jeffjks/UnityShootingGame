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
    public PopupMenuHandler m_PopupMenuHandler;
    public TextErrorMessage m_TextErrorMessage;
    
    private readonly ReplayManager.ReplayInfo[] _replayInfos = new ReplayManager.ReplayInfo[MAX_REPLAY_NUMBER];
    private string _replayDirectory;
    private const int MAX_REPLAY_NUMBER = 5;
    private int _currentSelectedSlot;

    private CanvasGroup[] _canvasGroups = new CanvasGroup[MAX_REPLAY_NUMBER];
    private ButtonStyling[] _buttonStylingArray = new ButtonStyling[MAX_REPLAY_NUMBER];
    private TextMeshProUGUI[] _buttonTexts = new TextMeshProUGUI[MAX_REPLAY_NUMBER];

    private void Awake()
    {
        _replayDirectory = $"{Application.dataPath}/";
        _canvasGroups = m_ReplaySlotPanel.GetComponentsInChildren<CanvasGroup>();
        _buttonStylingArray = m_ReplaySlotPanel.GetComponentsInChildren<ButtonStyling>();
        _buttonTexts = m_ReplaySlotPanel.GetComponentsInChildren<TextMeshProUGUI>();
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
                //_canvasGroups[i].interactable = false;
                _replayInfos[i] = null;
                continue;
            }
            var fileStream = new FileStream(filePath, FileMode.Open);
            _replayInfos[i] = ReplayManager.ReadBinaryHeader(fileStream);
            var dateTimeString = new DateTime(_replayInfos[i].m_DateTime).ToString("yyyy-MM-dd-hh-mm");
            _buttonStylingArray[i].m_NativeText = dateTimeString;
            _buttonTexts[i].SetText(dateTimeString);
            fileStream.Close();
        }
    }

    public void SaveReplaySlot1()
    {
        _currentSelectedSlot = 0;
        SaveReplay();
    }

    public void SaveReplaySlot2()
    {
        _currentSelectedSlot = 1;
        SaveReplay();
    }

    public void SaveReplaySlot3()
    {
        _currentSelectedSlot = 2;
        SaveReplay();
    }

    public void SaveReplaySlot4()
    {
        _currentSelectedSlot = 3;
        SaveReplay();
    }

    public void SaveReplaySlot5()
    {
        _currentSelectedSlot = 4;
        SaveReplay();
    }

    public override void Back() { }

    public void Cancel()
    {
        AudioService.PlaySound("CancelUI");
        LeaveMenu();
    }

    private void SaveReplay()
    {
        var oldFilePath = $"{_replayDirectory}replayTemp.rep";
        var newFilePath = $"{_replayDirectory}replay{_currentSelectedSlot}.rep";
        
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
    }

    private void CopyAndDelete(string oldFilePath, string newFilePath)
    {
        File.Copy(oldFilePath, newFilePath, true);
        File.Delete(oldFilePath);
        LeaveMenu();
    }

    private void LeaveMenu()
    {
        EventSystem.current.sendNavigationEvents = false;
        AudioService.PlaySound("SallyUI");
        
        StartCoroutine(ReturnToMainMenu());
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        FadeScreenService.ScreenFadeOut(2f);
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}
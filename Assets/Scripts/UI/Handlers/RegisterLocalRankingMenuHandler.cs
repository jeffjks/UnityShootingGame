using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class RegisterLocalRankingMenuHandler : MenuHandler
{
    public TMP_InputField m_InputFieldID;
    public Button m_ConfirmButton;
    public TextErrorMessage m_TextErrorMessage;

    private long _totalScore;
    private ShipAttributes _shipAttributes;
    private int _totalMiss;
    private long _clearedTime;

    protected override void Init()
    {
        _totalScore = InGameDataManager.Instance.TotalScore;
        _shipAttributes = InGameDataManager.Instance.CurrentShipAttributes;
        _totalMiss = InGameDataManager.Instance.TotalMiss;
        _clearedTime = InGameDataManager.Instance.ElapsedTime;

        m_InputFieldID.text = PlayerPrefs.GetString("LastLocalRankingID", string.Empty);
        m_InputFieldID.ActivateInputField();
    }

    public void RegisterLocalRanking()
    {
        var id = m_InputFieldID.text;
        var localRankingData = new LocalRankingData(id, _totalScore, _shipAttributes, _totalMiss, _clearedTime);
        var difficulty = (int)SystemManager.Difficulty;
        
        var rankingData = Utility.LoadDataFile<List<LocalRankingData>>(Application.dataPath, $"ranking{difficulty}.dat").jsonData;
        if (rankingData == null)
        {
            m_TextErrorMessage.DisplayText("FileLoadException");
            //rankingData = new List<LocalRankingData>();
            AudioService.PlaySound("CancelUI");
            return;
        }
        rankingData.Add(localRankingData);
        
        Utility.SaveDataFile(Application.dataPath, $"ranking{difficulty}.dat", rankingData);

        AudioService.PlaySound("SallyUI");
        CriticalStateSystem.SetCriticalState(120);
        LeaveMenu();
    }

    public override void Back()
    {
        AudioService.PlaySound("CancelUI");
        LeaveMenu();
    }

    private void LeaveMenu()
    {
        EventSystem.current.sendNavigationEvents = false;
        m_InputFieldID.DeactivateInputField();

        StartCoroutine(ReturnToMainMenu());
    }

    public void SelectConfirm()
    {
        m_ConfirmButton.Select();
    }
    
    private IEnumerator ReturnToMainMenu()
    {
        
        FadeScreenService.ScreenFadeOut(2f);
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }
}

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

    private long m_TotalScore;
    private ShipAttributes m_ShipAttributes;
    private int m_TotalMiss;
    private long m_ClearedTime;

    void Start()
    {
        m_TotalScore = InGameDataManager.Instance.TotalScore;
        m_ShipAttributes = PlayerManager.CurrentAttributes;
        m_TotalMiss = InGameDataManager.Instance.TotalMiss;
        m_ClearedTime = InGameDataManager.Instance.GetElapsedTime();

        m_InputFieldID.text = PlayerPrefs.GetString("LastLocalRankingID", string.Empty);
        m_InputFieldID.ActivateInputField();
    }

    public void RegisterLocalRanking()
    {
        var id = m_InputFieldID.text;
        LocalRankingData localRankingData = new LocalRankingData(id, m_TotalScore, m_ShipAttributes, m_TotalMiss, m_ClearedTime);
        
        Utility.SaveDataFile(Application.dataPath, $"ranking{(int) SystemManager.Difficulty}", localRankingData);

        AudioService.PlaySound("ConfirmUI");
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

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MenuHandler
{
    public GameObject m_SelectDifficultyPanel;
    public GameObject m_TrainingPanel;
    public GameObject m_ReplayPanel;
    public GameObject m_SelectRankingDifficultyPanel;
    public GameObject m_SettingsPanel;
    public GameObject m_KeyConfigPanel;
    public GameObject m_CreditPanel;

    void Start()
    {
        Time.timeScale = 1;
    }

    void OnEnable()
    {
        AudioService.LoadMusics("Main");
        AudioService.PlayMusic("Main");
        
        //m_GameManager.SetOptions();
    }

    public void StartGame() {
        GoToTargetMenu(m_SelectDifficultyPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void Training() {
        GoToTargetMenu(m_TrainingPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void RankingMenu() {
        GoToTargetMenu(m_SelectRankingDifficultyPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void ReplayMenu() {
        GoToTargetMenu(m_ReplayPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void SettingMenu() {
        GoToTargetMenu(m_SettingsPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void KeyConfigMenu() {
        GoToTargetMenu(m_KeyConfigPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void CreditMenu()
    {
        GoToTargetMenu(m_CreditPanel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void ExitGame() {
        AudioService.PlaySound("CancelUI");
        Application.Quit(); // 에디터에서는 무시됨
    }
}
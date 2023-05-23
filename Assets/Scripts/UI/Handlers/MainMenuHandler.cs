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
        SystemManager.SetGameMode(GameMode.GAMEMODE_NORMAL);
        GoToTargetMenu(m_SelectDifficultyPanel);
    }

    public void Training()
    {
        SystemManager.SetGameMode(GameMode.GAMEMODE_TRAINING);
        GoToTargetMenu(m_TrainingPanel);
    }

    public void RankingMenu() {
        GoToTargetMenu(m_SelectRankingDifficultyPanel);
    }

    public void ReplayMenu() {
        GoToTargetMenu(m_ReplayPanel);
    }

    public void SettingMenu() {
        GoToTargetMenu(m_SettingsPanel);
    }

    public void KeyConfigMenu() {
        GoToTargetMenu(m_KeyConfigPanel);
    }

    public void CreditMenu()
    {
        GoToTargetMenu(m_CreditPanel);
    }

    public void ExitGame() {
        AudioService.PlaySound("CancelUI");
        Application.Quit(); // 에디터에서는 무시됨
    }
}
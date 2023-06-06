using UnityEngine;

public class MainMenuHandler : MenuHandler
{
    public MenuHandler m_DifficultyPanel;
    public MenuHandler m_TrainingPanel;
    public MenuHandler m_ReplayPanel;
    public MenuHandler m_RankingDifficultyPanel;
    public MenuHandler m_SettingsPanel;
    public MenuHandler m_KeyConfigPanel;
    public MenuHandler m_CreditPanel;

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
        SystemManager.SetGameMode(GameMode.Normal);
        GoToTargetMenu(m_DifficultyPanel);
    }

    public void Training()
    {
        SystemManager.SetGameMode(GameMode.Training);
        GoToTargetMenu(m_TrainingPanel);
    }

    public void ReplayMenu() {
        SystemManager.SetGameMode(GameMode.Replay);
        GoToTargetMenu(m_ReplayPanel);
    }

    public void RankingMenu() {
        GoToTargetMenu(m_RankingDifficultyPanel);
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
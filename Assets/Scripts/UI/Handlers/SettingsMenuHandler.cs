using UnityEngine;

public class SettingsMenuHandler : MenuHandler
{
    public MenuHandler m_GraphicsPanel;
    public MenuHandler m_SoundPanel;
    public MenuHandler m_LanguagePanel;
    
    protected override void Init() { }

    public void GraphicsSettings() {
        GoToTargetMenu(m_GraphicsPanel);
    }

    public void SoundSettings() {
        GoToTargetMenu(m_SoundPanel);
    }

    public void LanguageSettings() {
        GoToTargetMenu(m_LanguagePanel);
    }
}
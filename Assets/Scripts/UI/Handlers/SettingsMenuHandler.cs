using UnityEngine;

public class SettingsMenuHandler : MenuHandler
{
    public MenuHandler m_GraphicsPanel;
    public MenuHandler m_SoundPanel;
    public MenuHandler m_LanguagePanel;

    public void Graphics() {
        GoToTargetMenu(m_GraphicsPanel);
    }

    public void Sound() {
        GoToTargetMenu(m_SoundPanel);
    }

    public void Language() {
        GoToTargetMenu(m_LanguagePanel);
    }
}
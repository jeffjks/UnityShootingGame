using UnityEngine;

public class SettingsMenuHandler : MenuHandler
{
    public GameObject m_GraphicsPanel;
    public GameObject m_SoundPanel;
    public GameObject m_LanguagePanel;

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
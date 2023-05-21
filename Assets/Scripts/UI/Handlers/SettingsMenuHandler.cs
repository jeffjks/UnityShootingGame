using UnityEngine;

public class SettingsMenuHandler : MenuHandler
{
    public GameObject m_GraphicsPannel;
    public GameObject m_SoundPannel;
    public GameObject m_LanguagePannel;

    public void Graphics() {
        GoToTargetMenu(m_GraphicsPannel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void Sound() {
        GoToTargetMenu(m_SoundPannel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public void Language() {
        GoToTargetMenu(m_LanguagePannel);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }
}
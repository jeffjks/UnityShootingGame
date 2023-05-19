using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_GraphicsMenu;
    public GameObject m_SoundMenu;
    public GameObject m_LanguageMenu;

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    Graphics();
                    break;
                case 1:
                    Sound();
                    break;
                case 2:
                    Language();
                    break;
                default:
                    Back();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();
        
        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void Graphics() {
        m_GraphicsMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void Sound() {
        m_SoundMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void Language() {
        m_LanguageMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void Back() {
        m_PreviousMenu.SetActive(true);
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }
}
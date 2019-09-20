using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_GraphicsPanel;
    public GameObject m_SoundPanel;
    public GameObject m_LanguagePanel;

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

        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();
        
        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void Graphics() {
        m_GraphicsPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Sound() {
        m_SoundPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Language() {
        m_LanguagePanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Back() {
        m_PreviousPanel.SetActive(true);
        CancelSound();
        gameObject.SetActive(false);
    }
}
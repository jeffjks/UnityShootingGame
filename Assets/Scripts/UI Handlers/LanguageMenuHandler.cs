using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LanguageMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_LanguagePanel;

    private int m_LanguageOptions;
    private int m_MaxLanguageOptions;

    void OnEnable() {
        UpdateValues();
        SetText();
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
        
        if (moveRawHorizontal != 0) { // 좌우 방향키
            switch(m_Selection) {
                case 0:
                    if (!m_isHorizontalAxisInUse)
                        m_LanguageOptions += moveRawHorizontal;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 1:
                    Apply();
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
        else if (Input.GetButtonDown("Fire2"))
            Cancel();
        
        if (m_LanguageOptions < 0)
            m_LanguageOptions = m_MaxLanguageOptions - 1;
        else if (m_LanguageOptions >= m_MaxLanguageOptions)
            m_LanguageOptions = 0;

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {
        m_LanguageOptions = (int) m_GameManager.m_Language;
        m_MaxLanguageOptions = m_GameManager.m_MaxLanguageOptions;
    }

    private void SetText() {
        m_Text[0].text = m_GameManager.GetLanguage(m_LanguageOptions);
    }

    private void Apply() {
        m_GameManager.SetLanguage(m_LanguageOptions);
        PlayerPrefs.Save();
        ConfirmSound();

        m_PreviousPanel.SetActive(true);
        m_LanguagePanel.SetActive(false);
    }

    private void Cancel() {
        UpdateValues();
        SetText();
        CancelSound();
        m_PreviousPanel.SetActive(true);
        m_LanguagePanel.SetActive(false);
    }
}
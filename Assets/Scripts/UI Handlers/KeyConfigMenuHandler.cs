using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyConfigMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_KeyConfigPanel;

    void OnEnable() {
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 6:
                    Apply();
                    break;
                case 7:
                    Cancel();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        SetText();

        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {

    }

    private void SetText() {
    }

    private void Apply() {
        PlayerPrefs.Save();
        ConfirmSound();

        m_PreviousPanel.SetActive(true);
        m_KeyConfigPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Cancel() {
        UpdateValues();
        SetText();
        CancelSound();
        m_PreviousPanel.SetActive(true);
        m_KeyConfigPanel.SetActive(false);
        gameObject.SetActive(false);
    }
}
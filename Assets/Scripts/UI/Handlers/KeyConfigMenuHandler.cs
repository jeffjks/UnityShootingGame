using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyConfigMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_KeyConfigMenu;

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
        else if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
        else if (Input.GetButtonDown("Fire2"))
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
        AudioService.PlaySound("ConfirmUI");

        m_PreviousMenu.SetActive(true);
        m_KeyConfigMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Cancel() {
        UpdateValues();
        SetText();
        AudioService.PlaySound("CancelUI");
        m_PreviousMenu.SetActive(true);
        m_KeyConfigMenu.SetActive(false);
        gameObject.SetActive(false);
    }
}
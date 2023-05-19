using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : GameUI
{
    public GameObject m_SelectDifficultyMenu;
    public GameObject m_TrainingMenu;
    public GameObject m_ReplayMenu;
    public GameObject m_SelectRankingDifficultyMenu;
    public GameObject m_SettingsMenu;
    public GameObject m_KeyConfigMenu;
    public GameObject m_CreditMenu;

    void Start()
    {
        Time.timeScale = 1;
    }

    void OnEnable()
    {
        AudioService.LoadMusics("Main");
        AudioService.PlayMusic("Main");
        
        m_GameManager.SetOptions();

        if (m_GameManager.m_NetworkAvailable) {
            m_IsEnabled[2] = m_GameManager.m_IsOnline;
        }
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    SelectDifficulty();
                    break;
                case 1:
                    Training();
                    break;
                case 2:
                    SelectRankingDifficulty();
                    break;
                case 3:
                    Option();
                    break;
                case 4:
                    Credit();
                    break;
                case 5:
                    ExitGame();
                    break;
                default:
                    break;
            }
        }

        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void SelectDifficulty() {
        m_SelectDifficultyMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void Training() {
        m_TrainingMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void SelectRankingDifficulty() {
        if (m_IsEnabled[2]) {
            m_SelectRankingDifficultyMenu.SetActive(true);
            AudioService.PlaySound("ConfirmUI");
            gameObject.SetActive(false);
        }
        else {
            AudioService.PlaySound("CancelUI");
        }
    }

    private void Replay() {
        AudioService.PlaySound("CancelUI");
        //m_ReplayMenu.SetActive(true);
        //ConfirmSound();
        //gameObject.SetActive(false);
    }

    private void Option() {
        m_SettingsMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void KeyConfig() {
        AudioService.PlaySound("CancelUI");
        // m_KeyConfigMenu.SetActive(true);
        // ConfirmSound();
        // gameObject.SetActive(false);
    }

    private void Credit() {
        m_CreditMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void ExitGame() {
        AudioService.PlaySound("CancelUI");
        Application.Quit(); // 에디터에서는 무시됨
    }
}
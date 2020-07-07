using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : GameUI
{
    public GameObject m_SelectDifficulty;
    public GameObject m_TrainingPanel;
    public GameObject m_ReplayPanel;
    public GameObject m_RankingPanel;
    public GameObject m_SettingsPanel;
    public GameObject m_KeyConfigPanel;
    public GameObject m_CreditPanel;

    [SerializeField] private MainMenuMusicController m_MainMenuMusicController = null;

    void OnEnable()
    {
        m_MainMenuMusicController.PlayMainMusic();
        m_GameManager.SetOptions();

        m_IsEnabled[2] = m_GameManager.m_IsOnline;
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
                    Ranking();
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
        m_SelectDifficulty.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Training() {
        m_TrainingPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Ranking() {
        if (m_IsEnabled[2]) {
            m_RankingPanel.SetActive(true);
            ConfirmSound();
            gameObject.SetActive(false);
        }
        else {
            CancelSound();
        }
    }

    private void Replay() {
        CancelSound();
        //m_ReplayPanel.SetActive(true);
        //ConfirmSound();
        //gameObject.SetActive(false);
    }

    private void Option() {
        m_SettingsPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void KeyConfig() {
        CancelSound();
        // m_KeyConfigPanel.SetActive(true);
        // ConfirmSound();
        // gameObject.SetActive(false);
    }

    private void Credit() {
        m_CreditPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void ExitGame() {
        CancelSound();
        Application.Quit(); // 에디터에서는 무시됨
    }
}
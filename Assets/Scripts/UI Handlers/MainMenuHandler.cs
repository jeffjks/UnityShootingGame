using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : GameUI
{
    public GameObject m_SelectDifficulty;
    public GameObject m_SettingsPanel;
    public GameObject m_KeyConfigPanel;

    [SerializeField] private MainMenuMusicController m_MainMenuMusicController = null;

    void OnEnable()
    {
        m_MainMenuMusicController.PlayMainMusic();
    }

    void Start()
    {
        m_IsEnabled[2] = false;
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
                    Option();
                    break;
                case 2:
                    KeyConfig();
                    break;
                case 3:
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

    private void Option() {
        m_SettingsPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void KeyConfig() {
        // m_KeyConfigPanel.SetActive(true);
        // ConfirmSound();
        // gameObject.SetActive(false);
    }

    private void ExitGame() {
        CancelSound();
        Application.Quit(); // 에디터에서는 무시됨
    }
}
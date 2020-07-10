using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : GameUI
{
    public GameObject m_SoundMenu;

    public PauseManager m_PauseManager;
    
    private SystemManager m_SystemManager = null;
    private PoolingManager m_PoolingManager = null;
    private int m_TempSelection;

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    m_PauseManager.Resume();
                    break;
                case 1:
                    Option();
                    break;
                case 2:
                    QuitGame();
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            m_PauseManager.Resume();

        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    void OnEnable() {
        m_SystemManager = SystemManager.instance_sm;
        m_PoolingManager = PoolingManager.instance_op;
        m_Selection = m_InitialSelection;
    }

    private void Option() {
        m_InitialSelection = 1;
        // ConfirmSound();
        m_SoundMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void QuitGame() {
        m_SystemManager.QuitGame();
    }
}

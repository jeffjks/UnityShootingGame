using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
    public float m_PauseDelay;
    public GameObject m_CanvasPause;
    public PauseMenuHandler m_PauseMenuHandler;

    private PlayerManager m_PlayerManager = null;
    private SystemManager m_SystemManager = null;
    private bool m_CanPause = true, m_PauseKeyPress = false;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
        m_CanvasPause.SetActive(false);
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            m_PauseKeyPress = true;
        
        if (m_PauseKeyPress) {
            m_PauseKeyPress = false;
            Pause();
        }
    }

    private void Pause() {
        if (Time.timeScale == 1) {
            if (m_SystemManager.m_PlayState != 3) {
                if (m_CanPause) {
                    Time.timeScale = 0;
                    AudioListener.pause = true;
                    m_CanvasPause.SetActive(true);
                    m_CanPause = false;
                }
            }
        }
        else if (m_PauseMenuHandler.gameObject.activeSelf) {
            Resume();
        }
    }

    public void Resume() {
        EscapePause();
        Invoke("PauseEnabled", m_PauseDelay);
    }

    private void PauseEnabled() {
        m_CanPause = true;
    }

    public void QuitGame() {
        //EscapePause();
        m_CanPause = true;
        m_PauseMenuHandler.m_InitialSelection = 0;
        m_CanvasPause.SetActive(false);

        m_SystemManager.QuitGame();
    }

    private void EscapePause() {
        Time.timeScale = 1;
        AudioListener.pause = false;
        
        m_CanPause = true;
        m_PauseMenuHandler.m_InitialSelection = 0;
        m_CanvasPause.SetActive(false);
    }
}
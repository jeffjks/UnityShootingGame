using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
    public float m_PauseDelay;

    private PauseMenuHandler m_PauseMenuHandler;
    private PlayerManager m_PlayerManager = null;
    private SystemManager m_SystemManager = null;
    private bool m_CanPause = true;

    [SerializeField]
    private GameObject m_PausePanel = null;

    void Awake() {
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
        m_PauseMenuHandler = m_PausePanel.GetComponent<PauseMenuHandler>();
    }

    void Start() {
        m_PausePanel.SetActive(false);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    private void Pause() {
        if (Time.timeScale == 1) {
            if (m_SystemManager.m_PlayState != 3) {
                if (m_CanPause) {
                    Time.timeScale = 0;
                    AudioListener.pause = true;
                    m_PausePanel.SetActive(true);
                    m_CanPause = false;
                }
            }
        }
        else if (m_PauseMenuHandler.gameObject.activeSelf) {
            Resume();
        }
    }

    public void Resume() {
        Time.timeScale = 1;
        m_PauseMenuHandler.m_InitialSelection = 0;
        AudioListener.pause = false;
        m_PausePanel.SetActive(false);
        Invoke("PauseEnabled", m_PauseDelay);
    }

    private void PauseEnabled() {
        m_CanPause = true;
    }
}
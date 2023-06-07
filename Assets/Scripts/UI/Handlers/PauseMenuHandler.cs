using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MenuHandler
{
    public MenuHandler m_SoundPanel;
    public IngameInputController m_InGameInputController;
    public PauseManager m_PauseManager;

    private int _activateTimer;

    private void OnEnable()
    {
        _activateTimer = 0;
        m_InGameInputController.Action_OnPause += Resume;
    }

    private void OnDisable()
    {
        m_InGameInputController.Action_OnPause -= Resume;
    }

    private void Update()
    {
        _activateTimer++;
    }

    public override void Back()
    {
    }

    public void SoundSettings() {
        GoToTargetMenu(m_SoundPanel);
    }
    
    public void Resume() {
        if (_activateTimer < 10)
        {
            return;
        }
        AudioService.PlaySound("CancelUI");
        m_PauseManager.Resume();
    }

    public void QuitGame() {
        m_PauseManager.QuitGame();
    }
}

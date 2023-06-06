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
        m_InGameInputController.Action_OnPause += Back;
    }

    private void OnDisable()
    {
        m_InGameInputController.Action_OnPause -= Back;
    }

    private void Update()
    {
        _activateTimer++;
    }

    public override void Back()
    {
        if (_activateTimer < 10)
        {
            return;
        }
        
        m_PauseManager.Resume();
        base.Back();
    }

    public void SoundSettings() {
        GoToTargetMenu(m_SoundPanel);
    }
    
    public void Resume() {
        Back();
    }

    public void QuitGame() {
        m_PauseManager.QuitGame();
    }
}

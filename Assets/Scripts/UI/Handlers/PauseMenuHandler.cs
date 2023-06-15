using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MenuHandler
{
    public MenuHandler m_SoundMenuPanel;
    public MenuHandler m_QuitMenuPanel;
    public IngameInputController m_InGameInputController;
    public PauseManager m_PauseManager;

    private void OnEnable()
    {
        m_InGameInputController.Action_OnPause += Resume;
    }

    private void OnDisable()
    {
        m_InGameInputController.Action_OnPause -= Resume;
    }

    public override void Back()
    {
    }

    public void SoundSettings() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        GoToTargetMenu(m_SoundMenuPanel);
    }

    public void QuitMenu() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        GoToTargetMenu(m_QuitMenuPanel);
    }
    
    public void Resume() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        AudioService.PlaySound("CancelUI");
        m_PauseManager.Resume();
    }
}

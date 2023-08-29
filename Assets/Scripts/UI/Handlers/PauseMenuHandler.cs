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
    public PauseManager m_PauseManager;
    
    protected override void Init() { }

    private void OnEnable()
    {
        InGameInputController.Instance.Action_OnPauseInput += Resume;
        InGameInputController.Instance.Action_OnEscapeInput += Resume;
    }

    private void OnDisable()
    {
        InGameInputController.Instance.Action_OnPauseInput -= Resume;
        InGameInputController.Instance.Action_OnEscapeInput -= Resume;
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
        
        _isActive = false;
    }
}

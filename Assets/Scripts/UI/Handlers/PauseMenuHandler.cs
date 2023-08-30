using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MenuHandler
{
    public MenuHandler m_SoundMenuPanel;
    public PopupMenuHandler m_PopupMenuHandler;
    public PauseManager m_PauseManager;
    
    protected override void Init() { }

    private void OnEnable()
    {
        InGameInputController.Instance.Action_OnPauseInput += Resume;
        InGameInputController.Instance.Action_OnEscapeInput += Resume;
    }

    private void OnDisable()
    {
        if (!InGameInputController.Instance)
            return;
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

    public void QuitMenu()
    {
        PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
            QuitGame,
            null,
            "게임을 종료하시겠습니까?",
            "Are you really want to exit?"
            ));
    }

    private void QuitGame() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        m_PauseManager.QuitGame();
    }
    
    public void Resume() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        AudioService.PlaySound("CancelUI");
        m_PauseManager.Resume();
        
        _isActive = false;
    }
}

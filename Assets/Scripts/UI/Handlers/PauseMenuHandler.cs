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
        _isActive = true;
        InGameInputController.Action_OnPauseInput += Resume;
        InGameInputController.Action_OnEscapeInput += Resume;
    }

    private void OnDisable()
    {
        InGameInputController.Action_OnPauseInput -= Resume;
        InGameInputController.Action_OnEscapeInput -= Resume;
    }

    public override void Back()
    {
    }

    public void SoundSettings() {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (!_isActive)
            return;
        
        GoToTargetMenu(m_SoundMenuPanel);
    }

    public void QuitMenu()
    {
        if (!_isActive)
            return;
        
        PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
            QuitGame,
            () => _isActive = true,
            "게임을 종료하시겠습니까?",
            "Are you really want to exit?"
            ));
    }

    private void QuitGame() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        _previousMenuStack.Clear();
        m_PauseManager.QuitGame();
    }
    
    public void Resume(bool isPressed) {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (!_isActive)
            return;
        if (!isPressed)
            return;
        
        AudioService.PlaySound("CancelUI");
        m_PauseManager.ClosePauseMenu();
        
        _isActive = false;
    }
}

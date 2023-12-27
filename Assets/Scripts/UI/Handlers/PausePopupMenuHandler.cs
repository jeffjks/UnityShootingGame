using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePopupMenuHandler : MenuHandler
{
    public PopupMenuHandler m_PopupMenuHandler;
    public PauseManager m_PauseManager;
    
    protected override void Init()
    {
        _isActive = true;
    }

    public override void Back()
    {
    }

    public void ShowPopupMessage(string messageNative, string message)
    {
        PopupMessageMenu(m_PopupMenuHandler, new PopupMenuContext(
            QuitGame,
            null,
            messageNative,
            message
        ));
    }

    private void QuitGame() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        _previousMenuStack.Clear();
        m_PauseManager.QuitGame();
    }
}

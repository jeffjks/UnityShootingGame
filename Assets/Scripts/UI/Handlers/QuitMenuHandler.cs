using UnityEngine;
using System.Collections.Generic;

public class QuitMenuHandler : MenuHandler
{
    public PauseManager m_PauseManager;

    public void QuitGame() {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        m_PauseManager.QuitGame();
    }
}
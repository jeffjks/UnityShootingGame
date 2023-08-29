using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenuHandler : MenuHandler
{
    public MenuHandler m_PausePanel;

    protected override void Init()
    {
    }

    public void PauseMenu() {
        GoToTargetMenu(m_PausePanel);
    }
}

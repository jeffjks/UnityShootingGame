using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDifficultyMenuHandler : MenuHandler
{
    public GameObject m_SelectAttributesMenu;
    
    public void SelectDifficultyNormal()
    {
        SystemManager.SetDifficulty(GameDifficulty.Normal);
        GoToTargetMenu(m_SelectAttributesMenu);
    }

    public void SelectDifficultyExpert()
    {
        SystemManager.SetDifficulty(GameDifficulty.Expert);
        GoToTargetMenu(m_SelectAttributesMenu);
    }

    public void SelectDifficultyHell()
    {
        SystemManager.SetDifficulty(GameDifficulty.Hell);
        GoToTargetMenu(m_SelectAttributesMenu);
    }
}

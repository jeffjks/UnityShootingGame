using UnityEngine;

public class DifficultyMenuHandler : MenuHandler
{
    public MenuHandler m_SelectAttributesMenu;
    
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

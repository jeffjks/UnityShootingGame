using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingMenuHandler : MenuHandler
{
    public MenuHandler m_SelectAttributesMenu;
    private TrainingInfo _oldTrainingInfo;
    private GameDifficulty _oldDifficulty;
    
    void OnEnable()
    {
        _oldTrainingInfo = SystemManager.TrainingInfo;
        _oldDifficulty = SystemManager.Difficulty;
    }

    public void StartTraining()
    {
        SystemManager.SetGameMode(GameMode.Training);
        GoToTargetMenu(m_SelectAttributesMenu);
    }
    
    public override void Back() {
        SystemManager.TrainingInfo = _oldTrainingInfo;
        SystemManager.SetDifficulty(_oldDifficulty);
        
        base.Back();
    }
}
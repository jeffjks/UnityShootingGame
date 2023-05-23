using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingMenuHandler : MenuHandler
{
    public GameObject m_SelectAttributesMenu;
    private TrainingInfo _oldTrainingInfo;
    private GameDifficulty _difficulty;
    
    void OnEnable()
    {
        _oldTrainingInfo = SystemManager.TrainingInfo;
    }

    public void StartTraining()
    {
        SystemManager.SetGameMode(GameMode.GAMEMODE_TRAINING);
        GoToTargetMenu(m_SelectAttributesMenu);
    }
    
    public override void Back() {
        SystemManager.TrainingInfo = _oldTrainingInfo;
        
        base.Back();
    }
}
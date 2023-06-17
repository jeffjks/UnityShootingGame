using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AttributesConfirmMenuHandler : MenuHandler
{
    public Button m_ConfirmButton;
    
    private Action _action_startStage;

    private void Start()
    {
        _action_startStage += StartStage;
    }
    
    public void Confirm()
    {
        EventSystem.current.currentInputModule.enabled = false;
        m_ConfirmButton.interactable = false;
        FadeScreenService.ScreenFadeOut(2f, _action_startStage);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    public override void Back()
    {
        base.Back(false);
    }

    private void StartStage()
    {
        _previousMenuStack.Clear();
        if (SystemManager.GameMode == GameMode.Normal)
        {
            SceneManager.LoadScene("Stage1");
        }
        else if (SystemManager.GameMode == GameMode.Training)
        {
            int stageNum = SystemManager.TrainingInfo.stage + 1;
            SceneManager.LoadScene($"Stage{stageNum}");
        }
    }
}

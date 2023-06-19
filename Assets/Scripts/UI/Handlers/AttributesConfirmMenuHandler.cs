using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttributesConfirmMenuHandler : MenuHandler
{
    public Button m_ConfirmButton;
    
    public void Confirm()
    {
        EventSystem.current.currentInputModule.enabled = false;
        m_ConfirmButton.interactable = false;
        FadeScreenService.ScreenFadeOut(2f, StartInGame);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    public override void Back()
    {
        base.Back(false);
    }

    private void StartInGame()
    {
        _previousMenuStack.Clear();
        
        var stage = (SystemManager.GameMode == GameMode.Training) ? SystemManager.TrainingInfo.stage : 0;
        SystemManager.Instance.StartStage(stage);
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttributesConfirmMenuHandler : MenuHandler
{
    public CanvasGroup m_CanvasGroup;
    public ArrowSet m_ArrowSet;
    
    protected override void Init() { }

    public void Confirm()
    {
        EventSystem.current.currentInputModule.enabled = false;
        m_CanvasGroup.interactable = false;
        FadeScreenService.ScreenFadeOut(2f, StartInGame);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
        CriticalStateSystem.SetCriticalState(120);
    }

    public override void Back()
    {
        base.Back(false);
        m_ArrowSet.SetInteractable(true);
    }

    private void StartInGame()
    {
        _previousMenuStack.Clear();
        
        var stage = (SystemManager.GameMode == GameMode.Training) ? SystemManager.TrainingInfo.stage : 0;
        SystemManager.StartStage(stage, Environment.TickCount);
    }
}

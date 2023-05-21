using UnityEngine;
using System.Collections.Generic;

public class GraphicsMenuHandler : MenuHandler
{
    private readonly Dictionary<GraphicsOption, int> m_OldGraphicSettings = new();

    void OnEnable() {
        foreach (var keyValuePair in GameSetting.m_GraphicOptions)
        {
            m_OldGraphicSettings[keyValuePair.Key] = keyValuePair.Value;
        }
    }

    public void Apply()
    {
        GameSetting.SaveGraphicSettings();
        PlayerPrefs.Save();
        
        base.Back();
    }
    
    public override void Back() {
        foreach (var keyValuePair in m_OldGraphicSettings)
        {
            GameSetting.m_GraphicOptions[keyValuePair.Key] = keyValuePair.Value;
        }
        
        base.Back();
    }
}
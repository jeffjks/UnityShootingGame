using UnityEngine;
using System.Collections.Generic;

public class GraphicsMenuHandler : MenuHandler
{
    private readonly Dictionary<GraphicsOption, int> _oldGraphicSettings = new();

    void OnEnable() {
        foreach (var keyValuePair in GameSetting.m_GraphicOptions)
        {
            _oldGraphicSettings[keyValuePair.Key] = keyValuePair.Value;
        }
    }

    public override void Apply()
    {
        GameSetting.SaveGraphicSettings();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back() {
        foreach (var keyValuePair in _oldGraphicSettings)
        {
            GameSetting.m_GraphicOptions[keyValuePair.Key] = keyValuePair.Value;
        }
        
        base.Back();
    }
}
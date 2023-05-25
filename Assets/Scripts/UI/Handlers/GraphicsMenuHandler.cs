using UnityEngine;
using System.Collections.Generic;

public class GraphicsMenuHandler : MenuHandler
{
    private GraphicsSettings _oldGraphicsSettings;

    void OnEnable() {
        _oldGraphicsSettings = GameSetting.m_GraphicsSettings;
    }

    public override void Apply()
    {
        GameSetting.SaveGraphicSettings();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back() {
        GameSetting.m_GraphicsSettings = _oldGraphicsSettings;
        
        base.Back();
    }
}
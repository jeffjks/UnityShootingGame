using UnityEngine;
using System.Collections.Generic;

public class GraphicsMenuHandler : MenuHandler
{
    private int _oldGraphicsResolution;
    private ScreenModeSetting _oldGraphicsScreenMode;
    private QualitySetting _oldGraphicsQuality;
    private AntiAliasingSetting _oldGraphicsAntiAliasing;

    protected override void Init()
    {
        _oldGraphicsResolution = GameSetting.GraphicsResolution;
        _oldGraphicsScreenMode = GameSetting.GraphicsScreenMode;
        _oldGraphicsQuality = GameSetting.GraphicsQuality;
        _oldGraphicsAntiAliasing = GameSetting.GraphicsAntiAliasing;
    }

    public override void Apply()
    {
        GameSetting.SaveGraphicSettings();
        GameSetting.SetGraphicSettings();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back() {
        GameSetting.GraphicsResolution = _oldGraphicsResolution;
        GameSetting.GraphicsScreenMode = _oldGraphicsScreenMode;
        GameSetting.GraphicsQuality = _oldGraphicsQuality;
        GameSetting.GraphicsAntiAliasing = _oldGraphicsAntiAliasing;
        
        base.Back();
    }
}
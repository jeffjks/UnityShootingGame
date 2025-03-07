﻿using UnityEngine;

public class LanguageMenuHandler : MenuHandler
{
    private Language _oldLanguage;

    protected override void Init()
    {
        _oldLanguage = GameSetting.CurrentLanguage;
    }

    public override void Apply()
    {
        GameSetting.SaveLanguageSetting();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back()
    {
        GameSetting.CurrentLanguage = _oldLanguage;
        
        base.Back();
    }
}
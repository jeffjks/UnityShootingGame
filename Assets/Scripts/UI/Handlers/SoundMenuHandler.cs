using UnityEngine;
using System.Collections.Generic;

public class SoundMenuHandler : MenuHandler
{
    private SoundSettings _oldSoundSettings;

    void OnEnable()
    {
        _oldSoundSettings = GameSetting.m_SoundSettings;
    }

    public override void Apply()
    {
        GameSetting.SaveSoundSettings();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back() {
        GameSetting.m_SoundSettings = _oldSoundSettings;
        
        base.Back();
    }
}
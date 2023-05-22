using UnityEngine;
using System.Collections.Generic;

public class SoundMenuHandler : MenuHandler
{
    private readonly Dictionary<SoundOption, int> _oldSoundSettings = new();

    void OnEnable() {
        foreach (var keyValuePair in GameSetting.m_SoundOptions)
        {
            _oldSoundSettings[keyValuePair.Key] = keyValuePair.Value;
        }
    }

    public void Apply()
    {
        GameSetting.SaveSoundSettings();
        PlayerPrefs.Save();
        
        base.Back();
    }
    
    public override void Back() {
        foreach (var keyValuePair in _oldSoundSettings)
        {
            GameSetting.m_SoundOptions[keyValuePair.Key] = keyValuePair.Value;
        }
        
        base.Back();
    }
}
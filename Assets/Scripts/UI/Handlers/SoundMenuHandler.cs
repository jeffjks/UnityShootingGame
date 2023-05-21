using UnityEngine;
using System.Collections.Generic;

public class SoundMenuHandler : MenuHandler
{
    private readonly Dictionary<SoundOption, int> m_OldSoundSettings = new();

    void OnEnable() {
        foreach (var keyValuePair in GameSetting.m_SoundOptions)
        {
            m_OldSoundSettings[keyValuePair.Key] = keyValuePair.Value;
        }
    }

    public void Apply()
    {
        GameSetting.SaveSoundSettings();
        PlayerPrefs.Save();
        
        base.Back();
    }
    
    public override void Back() {
        foreach (var keyValuePair in m_OldSoundSettings)
        {
            GameSetting.m_SoundOptions[keyValuePair.Key] = keyValuePair.Value;
        }
        
        base.Back();
    }
}
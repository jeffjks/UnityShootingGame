using UnityEngine;
using System.Collections.Generic;

public class SoundMenuHandler : MenuHandler
{
    private int _oldMusicVolume;
    private int _oldSoundEffectVolume;

    protected override void Init()
    {
        _oldMusicVolume = GameSetting.MusicVolume;
        _oldSoundEffectVolume = GameSetting.SoundEffectVolume;
    }

    public override void Apply()
    {
        GameSetting.SaveSoundSettings();
        GameSetting.SetSoundSettings();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back() {
        GameSetting.MusicVolume = _oldMusicVolume;
        GameSetting.SoundEffectVolume = _oldSoundEffectVolume;
        
        base.Back();
    }
}
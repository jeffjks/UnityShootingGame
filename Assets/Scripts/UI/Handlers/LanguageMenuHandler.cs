using UnityEngine;

public class LanguageMenuHandler : MenuHandler
{
    private Language _oldLanguage;

    void OnEnable()
    {
        _oldLanguage = GameSetting.m_Language;
    }

    public override void Apply()
    {
        GameSetting.SaveLanguageSetting();
        PlayerPrefs.Save();
        
        base.Apply();
    }
    
    public override void Back()
    {
        GameSetting.m_Language = _oldLanguage;
        
        base.Back();
    }
}
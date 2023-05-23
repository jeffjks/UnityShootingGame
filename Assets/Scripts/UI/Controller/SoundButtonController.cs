using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundButtonController : MonoBehaviour, IMoveHandler
{
    public SoundOption m_SoundOption;
    
    private TextMeshProUGUI _textUI;

    private void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        SetText();
    }

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;
        GameSetting.m_SoundOptions[m_SoundOption] += moveInputX;

        GameSetting.m_SoundOptions[m_SoundOption] = Mathf.Clamp(GameSetting.m_SoundOptions[m_SoundOption], 0, 100);
        
        if (m_SoundOption == SoundOption.MusicVolume)
            GameSetting.SaveMusicVolume();
        else if (m_SoundOption == SoundOption.SoundEffectVolume)
            GameSetting.SaveSoundEffectVolume();
        
        SetText();
    }

    private void SetText()
    {
        _textUI.text = GameSetting.m_SoundOptions[m_SoundOption].ToString();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEffectVolumeButtonController : MonoBehaviour, IMoveHandler
{
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
        GameSetting.SoundEffectVolume += moveInputX;

        GameSetting.SoundEffectVolume = Mathf.Clamp(GameSetting.SoundEffectVolume, 0, GameSetting.MAX_VOLUME);
        GameSetting.SetSoundEffectVolume();
        
        SetText();
    }

    private void SetText()
    {
        _textUI.text = GameSetting.SoundEffectVolume.ToString();
    }
}

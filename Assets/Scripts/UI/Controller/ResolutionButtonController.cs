using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionButtonController : MonoBehaviour, IMoveHandler
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
        if (axisEventData.moveDir is MoveDirection.Up or MoveDirection.Down)
            return;

        var moveInputX = (int) axisEventData.moveVector.x;
        GameSetting.GraphicsResolution += moveInputX;
        
        if (GameSetting.GraphicsResolution < 0)
        {
            GameSetting.GraphicsResolution = GameSetting.RESOLUTION_SETTING_COUNT - 1;
        }
        else if (GameSetting.GraphicsResolution >= GameSetting.RESOLUTION_SETTING_COUNT)
        {
            GameSetting.GraphicsResolution = 0;
        }

        SetText();
    }

    private void SetText()
    {
        try
        {
            Resolution resolution = GameSetting.GetCurrentResolution();
            _textUI.text = $"{resolution.width} x {resolution.height}";
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.SetText("Unknown");
        }
    }
}

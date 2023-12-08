using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MusicVolumeButtonController : MonoBehaviour, IMoveHandler
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
        GameSetting.MusicVolume += moveInputX;

        GameSetting.MusicVolume = Mathf.Clamp(GameSetting.MusicVolume, 0, GameSetting.MAX_VOLUME);
        GameSetting.SetMusicVolume();
        
        SetText();
    }

    private void SetText()
    {
        _textUI.text = GameSetting.MusicVolume.ToString();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(ColorTintButtonHelper))]
public class ColorTintButton : Button
{
    private TextMeshProUGUI _buttonText;
    private DefaultButtonTextColorDatas _defaultButtonTextColor;
    private ColorBlock _buttonTextColor;

    public ColorBlock ButtonTextColor
    {
        get => _buttonTextColor;
        set
        {
            _buttonTextColor = value;
            DoStateTransition(currentSelectionState, true);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        var helper = GetComponent<ColorTintButtonHelper>();
        _buttonText = helper.m_ButtonText;
        _defaultButtonTextColor = helper.m_DefaultButtonTextColor;
        
        ResetButtonTextColor();
    }

    public void ResetButtonTextColor()
    {
        ButtonTextColor = _defaultButtonTextColor.colorBlock;
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        Color color;
        Color textColor;
        switch (state)
        {
            case SelectionState.Normal:
                color = colors.normalColor;
                textColor = ButtonTextColor.normalColor;
                break;
            case SelectionState.Highlighted:
                color = colors.highlightedColor;
                textColor = ButtonTextColor.highlightedColor;
                break;
            case SelectionState.Pressed:
                color = colors.pressedColor;
                textColor = ButtonTextColor.pressedColor;
                break;
            case SelectionState.Selected:
                color = colors.selectedColor;
                textColor = ButtonTextColor.selectedColor;
                break;
            case SelectionState.Disabled:
                color = colors.disabledColor;
                textColor = ButtonTextColor.disabledColor;
                break;
            default:
                color = Color.black;
                textColor = Color.black;
                break;
        }

        if (gameObject.activeInHierarchy)
        {
            switch (transition)
            {
                case Transition.ColorTint: //Color Tint
                    ColorTween(targetGraphic, color * colors.colorMultiplier, colors.fadeDuration, instant);
                    ColorTween(_buttonText, textColor * ButtonTextColor.colorMultiplier, ButtonTextColor.fadeDuration, instant);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    private void ColorTween(Graphic target, Color targetColor, float fadeDuration, bool instant)
    {
        if (target == null)
            return;
        target.CrossFadeColor(targetColor, (!instant) ? fadeDuration : 0f, true, true);
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributesDetailsWindowController : MonoBehaviour
{
    public Image m_DetailsIconImage;
    public TextMeshProUGUI m_DetailsWindowText;
    public Animator m_WindowAnimator;

    private readonly int _animationTransition = Animator.StringToHash("Transition");
    
    public void SetWindow(DetailsWindowElement data)
    {
        m_DetailsIconImage.sprite = data.sprite;
        string text;
        if (GameSetting.CurrentLanguage == Language.Korean)
        {
            text = $"{data.nativeName}\n[ 비용: {data.cost} ]\n\n{data.nativeDescription}";
        }
        else
        {
            text = $"{data.name}\n[ 비용: {data.cost} ]\n\n{data.description}";
        }
        m_DetailsWindowText.SetText(text);
    }

    public void PlayTransitionAnimation()
    {
        m_WindowAnimator.SetTrigger(_animationTransition);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_HitCount : MonoBehaviour
{
    public HitCountController.HitCountType m_HitCountType;
    public CanvasGroup m_CanvasGroup;
    public TextMeshProUGUI m_HitNumText;
    public Animator m_Animator;
    
    private int _currentHitCount;
    private IEnumerator _hitCountEndCoroutine;
    private HitCountController.HitCountType _currentHitCountType;
    private bool _isActive;

    private bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            m_CanvasGroup.alpha = _isActive ? 1f : 0f;
        }
    }

    private readonly int _animationIdle = Animator.StringToHash("Idle");
    private readonly int _animationBreakDown = Animator.StringToHash("BreakDown");
    private readonly int _animationCompleted = Animator.StringToHash("Completed");
    private readonly int _animationDecreasing = Animator.StringToHash("Decreasing");

    private void Awake()
    {
        HitCountController.Action_OnUpdateHitCount += UpdateHitCount;
        HitCountController.Action_OnChangeHitCountState += SetHitCountColor;
        HitCountController.Action_OnChangeHitCountType += SetHitCountType;
    }

    private void OnDestroy()
    {
        HitCountController.Action_OnUpdateHitCount -= UpdateHitCount;
        HitCountController.Action_OnChangeHitCountState -= SetHitCountColor;
        HitCountController.Action_OnChangeHitCountType -= SetHitCountType;
    }

    private void UpdateHitCount(int value)
    {
        _currentHitCount = value;
        m_HitNumText.SetText($"<mspace=0.48em>{_currentHitCount}</mspace>\t");

        UpdateActiveState();
    }

    private void SetHitCountColor(HitCountController.HitCountState hitCountState)
    {
        switch (hitCountState)
        {
            case HitCountController.HitCountState.Default:
                m_Animator.SetTrigger(_animationIdle);
                break;
            case HitCountController.HitCountState.Decreasing:
                m_Animator.SetTrigger(_animationDecreasing);
                break;
            case HitCountController.HitCountState.BreakDown:
                m_Animator.SetTrigger(_animationBreakDown);
                break;
            case HitCountController.HitCountState.Completed:
                m_Animator.SetTrigger(_animationCompleted);
                break;
            default:
                m_Animator.SetTrigger(_animationIdle);
                break;
        }
    }

    private void SetHitCountType(HitCountController.HitCountType hitCountType)
    {
        _currentHitCountType = hitCountType;
        UpdateActiveState();
    }

    private void UpdateActiveState()
    {
        IsActive = m_HitCountType == _currentHitCountType && _currentHitCount >= 10;
    }
}

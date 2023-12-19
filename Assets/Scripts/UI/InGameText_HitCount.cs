using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_HitCount : MonoBehaviour
{
    public HitCountController.HitCountType m_HitCountType;
    public GameObject m_HitText;
    public TextMeshProUGUI m_HitNumText;
    public TextMeshProUGUI m_HitWordText;
    public Color m_DecreasingColor;
    public Color m_BreakDownColor;
    public Color m_EndFieldColor;
    
    private int _currentHitCount;
    private Color _defaultColor;
    private IEnumerator _hitCountEndCoroutine;
    private bool _isActive;

    private void Awake()
    {
        _defaultColor = m_HitNumText.color;
        m_HitText.SetActive(_isActive);
        
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

    private void UpdateHitCount(int value, bool isIncreasing)
    {
        if (!_isActive)
            return;
        
        _currentHitCount = value;
        m_HitNumText.SetText($"<mspace=0.48em>{_currentHitCount}</mspace>\t");

        m_HitText.SetActive(_currentHitCount >= 10);

        if (PlayerBombHandler.IsBombInUse)
            return;
        if (isIncreasing)
            SetHitCountColor(HitCountController.HitCountState.Default);
    }

    private void SetHitCountColor(HitCountController.HitCountState hitCountState)
    {
        if (!_isActive)
            return;
        if (_hitCountEndCoroutine != null)
            StopCoroutine(_hitCountEndCoroutine);
        
        switch (hitCountState)
        {
            case HitCountController.HitCountState.Default:
                m_HitNumText.color = _defaultColor;
                m_HitWordText.color = _defaultColor;
                break;
            case HitCountController.HitCountState.Decreasing:
                m_HitNumText.color = m_DecreasingColor;
                m_HitWordText.color = m_DecreasingColor;
                break;
            case HitCountController.HitCountState.BreakDown:
                m_HitNumText.color = m_BreakDownColor;
                m_HitWordText.color = m_BreakDownColor;
                _hitCountEndCoroutine = HideHitCountText();
                StartCoroutine(_hitCountEndCoroutine);
                break;
            case HitCountController.HitCountState.EndField:
                m_HitNumText.color = m_EndFieldColor;
                m_HitWordText.color = m_EndFieldColor;
                _hitCountEndCoroutine = HideHitCountText();
                StartCoroutine(_hitCountEndCoroutine);
                break;
            default:
                m_HitNumText.color = _defaultColor;
                m_HitWordText.color = _defaultColor;
                break;
        }
    }

    private IEnumerator HideHitCountText()
    {
        yield return new WaitForMillisecondFrames(2500);
        if (_hitCountEndCoroutine == null)
            yield break;
        m_HitText.SetActive(false);
        _hitCountEndCoroutine = null;
    }

    private void SetHitCountType(HitCountController.HitCountType hitCountType)
    {
        _isActive = m_HitCountType == hitCountType;
        m_HitText.SetActive(_isActive);
    }
}

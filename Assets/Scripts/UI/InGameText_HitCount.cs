using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_HitCount : MonoBehaviour
{
    public enum HitCountState
    {
        Default,
        Decreasing,
        BreakDown
    }
    
    public TextMeshProUGUI m_HitCountText;
    public Color m_FadeColor;
    public Color m_EndColor;

    private InGameDataManager _inGameDataManager;
    private int _currentHitCount;
    private Color _defaultColor;
    private HitCountState _hitCountState = HitCountState.Default;
    private IEnumerator _hitCountEndCoroutine;

    private void Awake()
    {
        _defaultColor = m_HitCountText.color;
        _inGameDataManager = FindObjectOfType<InGameDataManager>();
        
        _inGameDataManager.Action_OnUpdateHitCount += UpdateHitCount;
        _inGameDataManager.Action_OnChangeHitCountState += SetHitCountState;
    }

    private void OnDestroy()
    {
        _inGameDataManager.Action_OnUpdateHitCount -= UpdateHitCount;
        _inGameDataManager.Action_OnChangeHitCountState -= SetHitCountState;
    }

    private void UpdateHitCount(int value, bool isIncreasing)
    {
        _currentHitCount = value;
        m_HitCountText.SetText($"<mspace=0.48em>{_currentHitCount}</mspace><size=80>\tHit</size>");

        m_HitCountText.gameObject.SetActive(_currentHitCount >= 10);

        if (PlayerBombHandler.IsBombInUse)
            return;
        if (isIncreasing)
            SetHitCountState(HitCountState.Default);
    }

    private void SetHitCountState(HitCountState hitCountState)
    {
        if (_hitCountState == hitCountState)
            return;
        
        if (_hitCountEndCoroutine != null)
            StopCoroutine(_hitCountEndCoroutine);
        _hitCountState = hitCountState;
        Debug.Log(_hitCountState);
        
        switch (_hitCountState)
        {
            case HitCountState.Default:
                m_HitCountText.color = _defaultColor;
                break;
            case HitCountState.Decreasing:
                m_HitCountText.color = m_FadeColor;
                break;
            case HitCountState.BreakDown:
                m_HitCountText.color = m_EndColor;
                _hitCountEndCoroutine = HideHitCountText();
                StartCoroutine(_hitCountEndCoroutine);
                break;
            default:
                m_HitCountText.color = _defaultColor;
                break;
        }
    }

    private IEnumerator HideHitCountText()
    {
        yield return new WaitForMillisecondFrames(3000);
        Debug.Log("ABC 2");
        if (_hitCountState == HitCountState.BreakDown)
            m_HitCountText.gameObject.SetActive(false);
        _hitCountEndCoroutine = null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_HitCount : MonoBehaviour
{
    public TextMeshProUGUI m_HitCountText;

    private InGameDataManager _inGameDataManager;
    private int _currentHitCount;
    private bool _isFade;
    private bool IsFade
    {
        set
        {
            if (_isFade == value)
                return;
            _isFade = value;

            SetTextAlpha(_isFade ? 0.5f : 1f);
        }
    }

    private void Awake()
    {
        _inGameDataManager = FindObjectOfType<InGameDataManager>();
        _inGameDataManager.Action_OnUpdateHitCount += UpdateHitCount;
        _inGameDataManager.Action_OnFadeHitCount += OnFadeHitCount;
        PlayerManager.Action_OnPlayerDead += OnFadeHitCount;
        PlayerBombHandler.Action_OnBombUse += OnFadeHitCount;
    }

    private void OnDestroy()
    {
        _inGameDataManager.Action_OnUpdateHitCount -= UpdateHitCount;
        _inGameDataManager.Action_OnFadeHitCount -= OnFadeHitCount;
        PlayerManager.Action_OnPlayerDead -= OnFadeHitCount;
        PlayerBombHandler.Action_OnBombUse -= OnFadeHitCount;
    }

    private void UpdateHitCount(int value, bool isIncreasing)
    {
        _currentHitCount = value;
        m_HitCountText.SetText($"<mspace=0.48em>{_currentHitCount}</mspace><size=80>\tHit</size>");

        m_HitCountText.gameObject.SetActive(_currentHitCount >= 10);

        if (PlayerBombHandler.IsBombInUse)
            return;
        if (isIncreasing)
            IsFade = false;
    }

    private void OnFadeHitCount()
    {
        IsFade = true;
    }

    private void SetTextAlpha(float alpha)
    {
        var oldColor = m_HitCountText.color;
        oldColor.a = alpha;
        m_HitCountText.color = oldColor;
    }
}

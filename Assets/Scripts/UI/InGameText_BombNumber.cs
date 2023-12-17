using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_BombNumber : MonoBehaviour
{
    public TextMeshProUGUI m_BombNumberText;

    private InGameDataManager _inGameDataManager;

    private void Awake()
    {
        _inGameDataManager = FindObjectOfType<InGameDataManager>();
        _inGameDataManager.Action_OnUpdateBombNumber += UpdateBombNumber;
    }

    private void OnDestroy()
    {
        _inGameDataManager.Action_OnUpdateBombNumber -= UpdateBombNumber;
    }

    private void UpdateBombNumber(int currentValue, int maxValue)
    {
        m_BombNumberText.SetText($"{currentValue} / {maxValue}");
    }
}

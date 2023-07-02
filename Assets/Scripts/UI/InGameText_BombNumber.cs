using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_BombNumber : MonoBehaviour
{
    public TextMeshProUGUI m_BombNumberText;

    private void Awake()
    {
        FindObjectOfType<InGameDataManager>().Action_OnUpdateBombNumber += UpdateBombNumber;
    }

    private void UpdateBombNumber(int currentValue, int maxValue)
    {
        m_BombNumberText.SetText($"{currentValue} / {maxValue}");
    }
}

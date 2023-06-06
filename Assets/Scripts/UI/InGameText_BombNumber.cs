using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_BombNumber : MonoBehaviour
{
    public TextMeshProUGUI m_BombNumberText;

    private void Start()
    {
        SystemManager.instance_sm.Action_OnUpdateBombNumber += UpdateBombNumber;
    }

    private void UpdateBombNumber(int value)
    {
        m_BombNumberText.SetText(value.ToString());
    }
}

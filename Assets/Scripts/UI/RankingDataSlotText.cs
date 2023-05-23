using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class RankingDataSlotText : RankingDataSlot
{
    public TMP_Text m_Text;
    public bool m_IsRankText;

    public override void InitRankingData()
    {
        SetRankingData(String.Empty);
    }

    public override void SetRankingData(string text) {
        m_Text.text = text;

        if (m_IsRankText)
        {
            SetRankTextColor();
        }
    }

    public override void SetRankingData(ShipAttributes shipAttributes) {
        Debug.LogWarning("Ranking slot warning: Type unmatched.");
    }
    
    private void SetRankTextColor()
    {
        int rank;
        try {
            rank = int.Parse(m_Text.text);
        }
        catch (System.FormatException) {
            rank = 0;
        }

        switch (rank) {
            case 1:
                m_Text.color = new Color32(225, 223, 0, 255);
                break;
            case 2:
                m_Text.color = new Color32(192, 192, 192, 255);
                break;
            case 3:
                m_Text.color = new Color32(205, 127, 50, 255);
                break;
            default:
                m_Text.color = new Color32(83, 221, 233, 255);
                break;
        }
    }
}

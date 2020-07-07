using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingRankColor : MonoBehaviour
{
    public Text m_DisplayTextElement;
    
    void Update()
    {
        int rank;
        try {
            rank = int.Parse(m_DisplayTextElement.text);
        }
        catch (System.FormatException) {
            return;
        }

        switch (rank) {
            case 1:
                m_DisplayTextElement.color = new Color32(225, 223, 0, 255);
                break;
            case 2:
                m_DisplayTextElement.color = new Color32(192, 192, 192, 255);
                break;
            case 3:
                m_DisplayTextElement.color = new Color32(205, 127, 50, 255);
                break;
            default:
                m_DisplayTextElement.color = new Color32(83, 221, 233, 255);
                break;
        }
    }
}

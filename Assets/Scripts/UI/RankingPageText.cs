using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingPageText : MonoBehaviour
{
    public TextMeshProUGUI _textUI;

    public void SetText(int currentPage, int maxPage)
    {
        _textUI.SetText($"<mspace=0.5em>{currentPage + 1} / {maxPage + 1}</mspace>");
    }
}

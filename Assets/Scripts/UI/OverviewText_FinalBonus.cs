using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverviewText_FinalBonus : MonoBehaviour
{
    public OverviewHandler m_OverviewHandler;
    public TextMeshProUGUI m_FinalBonusText;

    private void Start()
    {
        //m_OverviewHandler.Action_OnUpdateFinalBonus += UpdateFinalBonusText;
    }

    private void UpdateFinalBonusText(long value)
    {
        m_FinalBonusText.SetText(value.ToString());
    }
}

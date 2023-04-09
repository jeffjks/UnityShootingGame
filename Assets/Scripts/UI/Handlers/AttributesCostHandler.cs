using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttributesCostHandler : MonoBehaviour
{
    public AttributesSelectHandler m_AttributesSelectHandler;
    public Text m_Text;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
    }

    void OnEnable()
    {
        string available = m_AttributesSelectHandler.m_AvailableCost.ToString();
        string used = m_SystemManager.m_UsedCost.ToString();
        if (m_GameManager.m_Language == 0)
            m_Text.text = "COST\n"+used+" / "+available;
        else if (m_GameManager.m_Language == 1)
            m_Text.text = "비용\n"+used+" / "+available;
    }

    void Update()
    {
        string available = m_AttributesSelectHandler.m_AvailableCost.ToString();
        string used = m_SystemManager.m_UsedCost.ToString();
        if (m_GameManager.m_Language == 0)
            m_Text.text = "COST\n"+used+" / "+available;
        else if (m_GameManager.m_Language == 1)
            m_Text.text = "비용\n"+used+" / "+available;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TextUI_Account : MonoBehaviour
{
    public Text m_Text;

    private GameManager m_GameManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;
        
        if (m_GameManager.GetAccountID() != null) {
            m_Text.text = m_GameManager.GetAccountID();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class InGameText_AccountName : MonoBehaviour
{
    public Text m_Text;

    private GameManager m_GameManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;
        SetAccountText();
    }

    void Update()
    {
        SetAccountText();
    }

    private void SetAccountText() {
        if (!m_GameManager.m_NetworkAvailable) {
            return;
        }
        if (m_GameManager.m_IsOnline) {
            m_Text.text = m_GameManager.GetAccountID();
        }
        else {
            m_Text.text = "(offline)";
        }
    }
}

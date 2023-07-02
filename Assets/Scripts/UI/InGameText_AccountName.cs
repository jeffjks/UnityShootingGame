using UnityEngine;
using UnityEngine.UI;

public class InGameText_AccountName : MonoBehaviour
{
    public Text m_Text;

    void Start()
    {
        SetAccountText();
    }

    void Update()
    {
        SetAccountText();
    }

    private void SetAccountText() {
        if (!DebugOption.NetworkAvailable) {
            return;
        }
        if (GameManager.isOnline) {
            m_Text.text = GameManager.GetAccountID();
        }
        else {
            m_Text.text = "(offline)";
        }
    }
}

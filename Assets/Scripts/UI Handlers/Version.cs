using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{
    private Text m_Text;

    private GameManager m_GameManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;

        m_Text = GetComponent<Text>();
        m_Text.text = "ver "+Application.version; // + "   Debug: "+ m_test1.ToString(); // + m_Language.ToString();
    }
}

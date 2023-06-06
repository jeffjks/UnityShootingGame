using UnityEngine;
using UnityEngine.UI;

public class InGameText_Version : MonoBehaviour
{
    public Text m_Text;

    void Start()
    {
        m_Text.text = "ver " + Application.version; // + "   Debug: "+ m_test1.ToString(); // + m_Language.ToString();
    }
}

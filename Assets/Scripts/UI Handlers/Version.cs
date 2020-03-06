using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{
    public Text m_Text;

    void Start()
    {
        m_Text.text = "ver " + Application.version; // + "   Debug: "+ m_test1.ToString(); // + m_Language.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguage : MonoBehaviour
{
    public Text m_Text;
    public Font[] m_Font;
    [TextArea(4, 6)]
    public string[] m_String;

    void OnEnable()
    {
        try {
            m_Text.font = m_Font[(int) GameSetting.m_Language];
            if (m_String.Length > 0)
                m_Text.text = m_String[(int) GameSetting.m_Language];
        }
        catch (System.NullReferenceException) {
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguage : MonoBehaviour
{
    public Text m_Text;
    public Font[] m_Font;
    public string[] m_String;
    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
    }

    void OnEnable()
    {
        try {
            m_Text.font = m_Font[m_GameManager.m_Language];
            if (m_String.Length > 0)
                m_Text.text = m_String[m_GameManager.m_Language];
        }
        catch (System.NullReferenceException) {
        }
    }
}

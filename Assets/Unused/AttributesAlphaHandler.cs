using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// UNUSED SCRIPT

public class AttributesAlphaHandler : MonoBehaviour
{
    public RectTransform m_RectTransform;
    public int m_SelectionNumber;

    private Image m_Image;
    private Text m_Text;
    //private AttributesHandler m_AttributesHandler;
    
    void Start()
    {
        m_Image = GetComponent<Image>();
        m_Text = GetComponentInChildren<Text>();
        //m_AttributesHandler = GetComponentInParent<AttributesHandler>();
    }

    void LateUpdate()
    {
        //float alpha = 1 - Mathf.Abs((m_RectTransform.localPosition[1] - m_AttributesHandler.m_DefaultY - 60*m_SelectionNumber)/120);
        float alpha = 1;
        if (alpha < 0.98f) {
            m_Image.color = new Color(m_Image.color[0], m_Image.color[1], m_Image.color[2], alpha);
            m_Text.color = new Color(m_Text.color[0], m_Text.color[1], m_Text.color[2], alpha);
        }
    }
}

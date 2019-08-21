using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ================ 게임 UI ================ //

public abstract class GameUI : MonoBehaviour {
    public int m_InitialSelection;

    protected int m_Selection;
    protected int m_Total;
    protected bool m_isVerticalAxisInUse = false;
    protected bool m_isHorizontalAxisInUse = false;
    protected float m_Alpha = 0f;
    protected Color m_ColorSelectedImage_0 = new Color32(255, 255, 0, 83);
    protected Color m_ColorSelectedImage_1 = new Color32(255, 255, 0, 200);
    protected Color m_ColorSelectedText = new Color32(54, 219, 54, 255); // green
    protected Color m_ColorDeselectedImage = new Color32(255, 255, 255, 200);
    protected Color m_ColorDeselectedText = new Color32(83, 221, 233, 255); // cyan
    protected Color m_ColorDisabled_0 = new Color32(163, 163, 163, 83); // grey
    protected Color m_ColorDisabled_1 = new Color32(163, 163, 163, 200); // grey
    protected Image[] m_Image;
    protected Text[] m_Text;
    protected bool[] m_IsEnabled;
    protected AudioSource[] m_AudioSource = new AudioSource[2];

    protected PlayerManager m_PlayerManager = null;
    protected GameManager m_GameManager = null;

    void Awake()
    {
        m_Image = GetComponentsInChildren<Image>();
        m_Text = GetComponentsInChildren<Text>();
        m_Total = m_Text.Length;
        m_IsEnabled = new bool[m_Total];

        for (int i=0; i<m_Total; i++)
            m_IsEnabled[i] = true;
        m_Selection = m_InitialSelection;

        FindAudioSource();

        m_GameManager = GameManager.instance_gm;
        m_PlayerManager = PlayerManager.instance_pm;
    }

    protected void FindAudioSource() {
        GameObject obj = transform.root.gameObject;
        m_AudioSource[0] = obj.GetComponents<AudioSource>()[0];
        m_AudioSource[1] = obj.GetComponents<AudioSource>()[1];
    }

    protected void MoveCursorVertical(int move) {
        if (move != 0) {
            if (m_isVerticalAxisInUse == false) {
                m_Selection -= move;
                m_isVerticalAxisInUse = true;
            }
        }
        else {
            m_isVerticalAxisInUse = false;
        }
    }

    protected bool MoveCursorHorizontal(int move, bool selection) {
        if (move != 0) {
            if (m_isHorizontalAxisInUse == false) {
                m_isHorizontalAxisInUse = true;
                if (selection) {
                    m_Selection -= move;
                    return true;
                }
            }
        }
        else {
            m_isHorizontalAxisInUse = false;
        }
        return false;
    }
    
    protected int EndToStart(int selection, int end) {
        if (selection >= end) {
            selection = 0;
        }
        else if (selection < 0) {
            selection = end - 1;
        }
        return selection;
    }

    protected void SetColor() {
        for (int i = 0; i < m_Total; i++) {
            m_Image[i+1].color = m_ColorDeselectedImage;
            m_Text[i].color = m_ColorDeselectedText;
        }

        for (int i = 0; i < m_Total; i++) {
            if (m_IsEnabled[i] == false) {
                m_Image[i+1].color = m_ColorDisabled_0;
                m_Text[i].color = m_ColorDisabled_0;
            }
        }
        
        m_Alpha += 0.036f;

        if (m_IsEnabled[m_Selection]) {
            m_Image[m_Selection+1].color = Color.Lerp(m_ColorSelectedImage_0, m_ColorSelectedImage_1, m_Alpha);
            m_Text[m_Selection].color = m_ColorSelectedText;
        }
        else {
            m_Image[m_Selection+1].color = Color.Lerp(m_ColorDisabled_0, m_ColorDisabled_1, m_Alpha);
            m_Text[m_Selection].color = Color.Lerp(m_ColorDisabled_0, m_ColorDisabled_1, m_Alpha);
        }

        if (m_Alpha > 1) {
            m_Alpha = 0;
        }
    }

    protected void ConfirmSound() {
        if (!AudioListener.pause) {
            m_AudioSource[0].Stop();
            m_AudioSource[0].Play();
        }
    }

    protected void CancelSound() {
        if (!AudioListener.pause) {
            m_AudioSource[1].Stop();
            m_AudioSource[1].Play();
        }
    }
}

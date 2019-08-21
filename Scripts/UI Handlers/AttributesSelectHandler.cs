using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class AttributeSelectButtonUI : GameUI
{
    protected bool m_Enable = false;
    
    protected int EndAndStart(int selection, int end) {
        if (selection >= end) {
            selection = end - 1;
        }
        else if (selection < 0) {
            selection = 0;
        }
        return selection;
    }

    protected void SetDeselectedColor() {
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
    }
}

public class AttributesSelectHandler : MonoBehaviour
{
    [SerializeField] private MainMenuMusicController m_MainMenuMusicController = null;
    [HideInInspector] public int m_State = 1;
    public int m_AvailableCost;

    void OnEnable()
    {
        m_MainMenuMusicController.PlaySelectMusic();
    }
}

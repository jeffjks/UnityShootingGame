using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPreviewManager : MonoBehaviour
{
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;
    
    protected PlayerManager m_PlayerManager = null;
    protected abstract void SetPlayerPreviewColors();
    
    void Awake()
    {
        SpeedPart();
    }

    public virtual void SetPreviewDesign() {
        for (int i = 0; i < m_SpeedPart.Length; i++) { // Init
            m_SpeedPart[i].SetActive(false);
        }
        m_ModulePart.SetActive(false);

        SpeedPart();
        
        if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Module) != 0) // Module
            m_ModulePart.SetActive(true);
        
        SetPlayerPreviewColors();
    }

    private void SpeedPart() {
        m_SpeedPart[PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed)].SetActive(true); // Speed
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPreviewManager : MonoBehaviour
{
    public PreviewScreen m_PreviewScreen;
    public PlayerColorDatas m_PlayerColorData;
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;

    protected MeshRenderer[] _meshRenderers;

    protected virtual void SetPreviewDesign(ShipAttributes shipAttributes) {
        for (int i = 0; i < m_SpeedPart.Length; i++) { // Init
            m_SpeedPart[i].SetActive(false);
        }
        m_ModulePart.SetActive(false);

        m_SpeedPart[shipAttributes.GetAttributes(AttributeType.Speed)].SetActive(true);
        
        if (shipAttributes.GetAttributes(AttributeType.ModuleIndex) != 0) // Module
            m_ModulePart.SetActive(true);
        
        SetPlayerPreviewColors(shipAttributes.GetAttributes(AttributeType.Color));
    }

    private void SetPlayerPreviewColors(int attributeColor) {
        for (int i = 0; i < m_PlayerColorData.playerColorMaterial.Length; i++) {
            if (i == attributeColor)
            {
                for (int j = 0; j < _meshRenderers.Length; j++)
                {
                    _meshRenderers[j].material = m_PlayerColorData.playerColorMaterial[i];
                }

                break;
            }
        }
    }
}

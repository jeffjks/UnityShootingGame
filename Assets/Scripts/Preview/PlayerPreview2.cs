using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview2 : PlayerPreview
{
    public PlayerPreviewDrone[] m_PlayerPreviewDrone = new PlayerPreviewDrone[4];
    public GameObject m_DronePart; // Shot Spawner
    public GameObject m_PreviewPoolingManager;

    public override void SetPreviewDesign() {
        base.SetPreviewDesign();
        
        SetPreviewWeapons();
    }

    private void SetPreviewWeapons() {
        for (int i = 0; i < m_PlayerPreviewDrone.Length; i++)
            m_PlayerPreviewDrone[i].SetPreviewDrones();
    }

    protected override void SetPlayerPreviewColors() {
        m_DronePart.SetActive(false);
        m_PreviewPoolingManager.SetActive(false);
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        PlayerColors playerColors = GetComponent<PlayerColors>();
        int max_meshRenderer = meshRenderer.Length;
        
        // Color
        for (int i=0; i<3; i++) {
            if (m_GameManager.m_CurrentAttributes.m_Color == i)
                for (int j=0; j<max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
        m_DronePart.SetActive(true);
        m_PreviewPoolingManager.SetActive(true);
    }
}

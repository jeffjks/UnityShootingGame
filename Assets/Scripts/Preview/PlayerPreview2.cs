using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview2 : PlayerPreviewManager
{
    public PlayerDrone[] m_PlayerDrone = new PlayerDrone[4];
    public PlayerPreviewLaserShooter m_PlayerPreviewLaserShooter;
    public PlayerPreviewShooter m_PlayerPreviewShooter;
    public GameObject m_DronePart; // Shot Spawner

    public override void SetPreviewDesign() {
        base.SetPreviewDesign();
        
        SetPreviewWeapons();
    }

    private void SetPreviewWeapons() {
        for (int i = 0; i < m_PlayerDrone.Length; i++)
            m_PlayerDrone[i].SetPreviewDrones();
        m_PlayerPreviewLaserShooter.SetLaserType();
        m_PlayerPreviewShooter.SetPreviewShooter();
    }

    protected override void SetPlayerPreviewColors() {
        m_DronePart.SetActive(false);
        // m_PreviewPoolingManager.SetActive(false);
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        PlayerColors playerColors = GetComponent<PlayerColors>();
        int max_meshRenderer = meshRenderer.Length;
        
        // Color
        for (int i = 0; i < 3; i++) {
            if (m_PlayerManager.m_CurrentAttributes.GetAttributes(AttributeType.Color) == i)
                for (int j = 0; j < max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
        m_DronePart.SetActive(true);
        // m_PreviewPoolingManager.SetActive(true);
    }
}

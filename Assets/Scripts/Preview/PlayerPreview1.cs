using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview1 : PlayerPreviewManager
{
    void Update()
    {
        RotateSelf();
    }

    private void RotateSelf() {
        transform.Rotate(Vector3.up * 48f * Time.deltaTime);
    }

    protected override void SetPlayerPreviewColors() {
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
    }
}

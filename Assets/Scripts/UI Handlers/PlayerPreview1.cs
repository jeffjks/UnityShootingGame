using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview1 : MonoBehaviour
{
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;

    private GameManager m_GameManager = null;
    private PlayerManager m_PlayerManager = null;

    void OnEnable()
    {
        SetPreviewDesign();
    }

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_PlayerManager = PlayerManager.instance_pm;
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.up * 48f * Time.deltaTime);
    }

    public void SetPreviewDesign() {
        for (int i = 0; i < m_SpeedPart.Length; i++) { // Init
            m_SpeedPart[i].SetActive(false);
        }
        m_ModulePart.SetActive(false);

        m_SpeedPart[m_GameManager.m_CurrentAttributes.m_Speed].SetActive(true); // Speed
        
        if (m_GameManager.m_CurrentAttributes.m_Module != 0) // Module
            m_ModulePart.SetActive(true);
        
        SetPlayerPreviewColors();
    }

    private void SetPlayerPreviewColors() {
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
    }
}

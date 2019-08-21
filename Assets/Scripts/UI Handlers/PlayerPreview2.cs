using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview2 : MonoBehaviour
{
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
        for (int i=1; i<=8; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        // Speed
        if (m_GameManager.m_CurrentAttributes[1] == 0)
            transform.GetChild(1).gameObject.SetActive(true);
        else if (m_GameManager.m_CurrentAttributes[1] == 1)
            transform.GetChild(2).gameObject.SetActive(true);
        else if (m_GameManager.m_CurrentAttributes[1] == 2)
            transform.GetChild(3).gameObject.SetActive(true);

        // Shot Damage
        if (m_GameManager.m_CurrentAttributes[3] == 2)
            transform.GetChild(4).gameObject.SetActive(true);
        
        // Laser Damage
        if (m_GameManager.m_CurrentAttributes[4] == 2)
            transform.GetChild(5).gameObject.SetActive(true);
        
        // Module
        if (m_GameManager.m_CurrentAttributes[5] == 1)
            transform.GetChild(6).gameObject.SetActive(true);
        else if (m_GameManager.m_CurrentAttributes[5] == 2)
            transform.GetChild(7).gameObject.SetActive(true);
        else if (m_GameManager.m_CurrentAttributes[5] == 3)
            transform.GetChild(8).gameObject.SetActive(true);
        
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        PlayerColors playerColors = GetComponent<PlayerColors>();
        int max_meshRenderer = meshRenderer.Length - 4;
        
        // Color
        for (int i=0; i<3; i++) {
            if (m_GameManager.m_CurrentAttributes[0] == i)
                for (int j=0; j<max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
    }
}

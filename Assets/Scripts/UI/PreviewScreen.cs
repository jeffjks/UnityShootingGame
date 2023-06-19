using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewScreen : MonoBehaviour
{
    public GameObject m_PlayerPreviewCamera;
    
    private void OnEnable()
    {
        if (m_PlayerPreviewCamera != null)
            m_PlayerPreviewCamera.SetActive(true);
    }

    private void OnDisable()
    {
        if (m_PlayerPreviewCamera != null)
            m_PlayerPreviewCamera.SetActive(false);
    }
}

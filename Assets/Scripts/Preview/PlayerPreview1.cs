using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview1 : PlayerPreviewManager
{
    private void Awake()
    {
        m_PreviewScreen.Action_OnChangedTempAttributes += SetPreviewDesign;
        
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }
    
    void Update()
    {
        RotateSelf();
    }

    private void RotateSelf() {
        transform.Rotate(Vector3.up * (48f * Time.deltaTime));
    }
}

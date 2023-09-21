using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreview1 : PlayerPreviewManager
{
    private void Awake()
    {
        _previewScreen = FindObjectOfType<PreviewScreen>();
        _previewScreen.Action_OnTempAttributesChanged += SetPreviewDesign;
        
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
    }
    
    void Update()
    {
        RotateSelf();
    }

    private void RotateSelf() {
        transform.Rotate(Vector3.up * (48f * Time.deltaTime));
    }
}

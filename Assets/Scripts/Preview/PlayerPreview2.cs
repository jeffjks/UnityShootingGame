using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerPreview2 : PlayerPreviewManager
{
    public PlayerLaserHandler m_PlayerLaserHandler;
    public PlayerShotHandler m_PlayerShotHandler;
    public GameObject m_DronePart; // Shot Spawner
    
    private void Awake()
    {
        _previewScreen = FindObjectOfType<PreviewScreen>();
        if (_previewScreen != null)
            _previewScreen.Action_OnTempAttributesChanged += SetPreviewDesign;
        
        m_DronePart.SetActive(false);
        _meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        m_DronePart.SetActive(true);
    }
    
    protected override void SetPreviewDesign(ShipAttributes shipAttributes) {
        base.SetPreviewDesign(shipAttributes);
        
        SetPreviewWeapons(shipAttributes);
    }

    private void SetPreviewWeapons(ShipAttributes shipAttributes)
    {
        m_PlayerLaserHandler.LaserIndex = shipAttributes.GetAttributes(AttributeType.LaserIndex);
        m_PlayerShotHandler.ShotIndex = shipAttributes.GetAttributes(AttributeType.ShotIndex);
        m_PlayerShotHandler.SubWeaponIndex = shipAttributes.GetAttributes(AttributeType.SubWeaponIndex);
    }
}

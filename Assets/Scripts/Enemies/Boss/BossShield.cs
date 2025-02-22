using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : MonoBehaviour {
    public GameObject ShieldBody;
    public GameObject Ring1;
    public GameObject Ring2;
    public float RotateSpeed;
    public float ShieldOutShineTimeSpace;

    private float m_EffectTimer;
    private bool m_IsShining = false;
    private Material m_ShieldMaterial;
    private float m_OffsetY = 1;
    private Quaternion m_DefaultQuaternion;
    private readonly int _scanningOffsetYPropId = Shader.PropertyToID("_ScanningOffsetY");

	private void Start ()
    {
        m_ShieldMaterial = ShieldBody.GetComponent<MeshRenderer>().material;
        m_DefaultQuaternion = transform.localRotation;
    }
    
    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        transform.localRotation = m_DefaultQuaternion;
        
        Ring1.transform.Rotate(Vector3.right, Time.deltaTime * RotateSpeed);
        Ring2.transform.Rotate(Vector3.forward, Time.deltaTime * RotateSpeed);

        if (m_IsShining) {
            m_OffsetY += 0.025f * Time.timeScale;
            m_ShieldMaterial.SetFloat(_scanningOffsetYPropId, m_OffsetY);
            if (m_OffsetY > 0.63) {
                m_OffsetY = -0.64f;
                m_IsShining = false;
                m_EffectTimer = 0;
            }
        }
        else {
            m_EffectTimer += Time.deltaTime * Time.timeScale;
            if (m_EffectTimer >= ShieldOutShineTimeSpace) {
                m_IsShining = true;
            }
        }
    }
}

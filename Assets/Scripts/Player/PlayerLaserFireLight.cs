using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserFireLight : MonoBehaviour {
    
    public Light m_Light;
    public Color[] m_Color = new Color[3];

    /*
    private void Start()
    {
        if (GameSetting.GraphicsQuality > QualitySetting.VeryHigh)
        {
            m_Light.enabled = false;
        }
    }
    */

    private void Update()
    {
        m_Light.intensity = new System.Random().Next(8, 16);
    }

    public void SetLightColor(int index) {
        m_Light.color = m_Color[index];
    }
}
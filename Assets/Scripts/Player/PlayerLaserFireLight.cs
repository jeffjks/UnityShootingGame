using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserFireLight : MonoBehaviour {
    
    public Light m_Light;
    public Color[] m_Color = new Color[3];

    void Start()
    {
        GameManager gameManager = GameManager.instance_gm;

        if (gameManager != null) {
            if (GameSetting.GraphicsQuality > QualitySetting.VeryHigh) {
                m_Light.enabled = false;
            }
        }
    }

    void Update()
    {
        m_Light.intensity = Random.Range(8f, 16f);
    }

    public void SetLightColor(int index) {
        m_Light.color = m_Color[index];
    }
}
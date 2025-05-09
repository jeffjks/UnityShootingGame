﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebrisEffect : MonoBehaviour, IObjectPooling
{
    public GameObject[] m_DebrisLarge;
    public GameObject[] m_DebrisMedium;
    public GameObject[] m_DebrisSmall;
    public string m_ObjectName;
    
    [SerializeField] private Transform _debrisTransform;

    private Vector2 m_Position2D;
    //private Material[] m_Materials;
    private int m_DebrisIndex = -1;
    private readonly Dictionary<DebrisType, GameObject[]> _debrisDict = new();
    private const float BoundaryPadding = 3f;

    //private IEnumerator m_FadeOutAnimation;

    void Awake()
    {
        // MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
        // m_Materials = new Material[meshRenderers.Length];
        //
        // for (int i = 0; i < meshRenderers.Length; i++) {
        //     m_Materials[i] = meshRenderers[i].material;
        // }

        _debrisDict[DebrisType.Small] = m_DebrisSmall;
        _debrisDict[DebrisType.Medium] = m_DebrisMedium;
        _debrisDict[DebrisType.Large] = m_DebrisLarge;
    }
    
    private void Update()
    {
        GetCoordinates();
        CheckOutside();
    }

    private void GetCoordinates() {
        m_Position2D = BackgroundCamera.GetScreenPosition(transform.position);
    }

    private void CheckOutside() // 화면 바깥으로 나갈시 파괴
    {
        if (InnerGameBoundary.IsOutOfCamera(m_Position2D, BoundaryPadding))
            ReturnToPool();
    }
    
    public void OnStart(DebrisType debrisType, float debrisScale)
    {
        DeactivateAllChildren();

        m_DebrisIndex = new System.Random().Next(0, _debrisDict[debrisType].Length);
        var rotationRandom = new System.Random().Next(0, 360);
        var debrisObject = _debrisDict[debrisType][m_DebrisIndex];
        _debrisTransform.rotation = Quaternion.Euler(0f, rotationRandom, 0f);
        _debrisTransform.localScale = new Vector3(debrisScale, debrisScale, debrisScale);
        debrisObject.SetActive(true);
        
        //m_Materials[m_DebrisIndex].color = Color.white;
        
        //m_FadeOutAnimation = FadeOutAnimation();
        //StartCoroutine(m_FadeOutAnimation);
    }

    /*private IEnumerator FadeOutAnimation() {
        float init_alpha = m_Materials[m_DebrisIndex].color.a;
        int frame = m_LifeTime * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_fade = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float alpha = Mathf.Lerp(init_alpha, 0f, t_fade);
            Color color_tmp = m_Materials[m_DebrisIndex].color;
            m_Materials[m_DebrisIndex].color = new Color(color_tmp.r, color_tmp.g, color_tmp.b, alpha);
            yield return new WaitForMillisecondFrames(0);
        }
        
        ReturnToPool();
    }*/

    public void ReturnToPool()
    {
        // if (m_FadeOutAnimation != null) {
        //     StopCoroutine(m_FadeOutAnimation);
        // }
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.Debris);
    }

    private void DeactivateAllChildren() {
        foreach (var keyValuePair in _debrisDict)
        {
            foreach (var debrisObj in keyValuePair.Value)
            {
                debrisObj.SetActive(false);
            }
        }
    }
}

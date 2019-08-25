using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DebrisEffect : MonoBehaviour
{
    public string m_ObjectName;
    public GameObject[] m_DebrisObject;
    public byte m_LifeTime;
    public Collider2D m_Collider2D; // 지상 아이템 콜라이더 보정 및 충돌 체크

    private Vector2 m_Position2D;
    private Vector2 m_BackgroundCameraSize;
    private Material[] m_Materials;
    private int m_DebrisType;
    private bool m_OnEnable = false;

    [HideInInspector] public int m_DebrisSize;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_BackgroundCameraSize = m_SystemManager.m_BackgroundCameraSize;

        
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        m_Materials = new Material[meshRenderers.Length];
        
        for (int i = 0; i < meshRenderers.Length; i++) {
            m_Materials[i] = meshRenderers[i].material;
        }
        DeactivateAllChildren();
    }
    
    void Update()
    {
        GetCoordinates();
    }

    private void GetCoordinates() {
        m_Position2D = GetScreenPosition(transform.position);
        m_Collider2D.transform.position = m_Position2D;
    }

    private Vector2 GetScreenPosition(Vector3 pos) {
        float main_camera_xpos = m_PlayerManager.m_MainCamera.transform.position.x;
        Vector3 screen_pos = m_SystemManager.m_BackgroundCamera.WorldToScreenPoint(pos);
        Vector2 modified_pos = new Vector2(
            screen_pos[0]*m_BackgroundCameraSize.x/Screen.width - m_BackgroundCameraSize.x/2 + main_camera_xpos,
            screen_pos[1]*m_BackgroundCameraSize.y/Screen.height - m_BackgroundCameraSize.y);
        return modified_pos;
    }
    
    void OnEnable()
    {
        if (!m_OnEnable) {
            m_OnEnable = true;
            return;
        }
        
        DeactivateAllChildren();

        switch(m_DebrisSize) {
            case 1: // Small
                m_DebrisType = Random.Range(0, 3);
                break;
            case 2: // Large
                m_DebrisType = Random.Range(3, 5);
                break;
            default:
                m_DebrisType = -1;
                OnDeath();
                return;
        }

        m_DebrisObject[m_DebrisType].SetActive(true);
        m_Materials[m_DebrisType].DOFade(0f, "_Color", m_LifeTime);
        Invoke("OnDeath", m_LifeTime);
    }

    public void OnDeath() {
        CancelInvoke("OnDeath");
        DOTween.Kill(m_Materials[m_DebrisType]);
        m_Materials[m_DebrisType].SetColor("_Color", Color.white);
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.DEBRIS);
    }

    void DeactivateAllChildren() {
        for (int i = 0; i < m_DebrisObject.Length; i++) {
            m_DebrisObject[i].SetActive(false);
        }
    }
}

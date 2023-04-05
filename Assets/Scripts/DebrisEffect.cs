using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebrisEffect : MonoBehaviour
{
    public string m_ObjectName;
    public GameObject[] m_DebrisObject;
    public int m_LifeTime;
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
    private IEnumerator m_FadeOutAnimation;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_BackgroundCameraSize = m_SystemManager.m_BackgroundCameraSize;

        
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
        m_Materials = new Material[meshRenderers.Length];
        
        for (int i = 0; i < meshRenderers.Length; i++) {
            m_Materials[i] = meshRenderers[i].material;
        }
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
        float main_camera_xpos = m_SystemManager.m_MainCamera.transform.position.x;
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
                m_DebrisType = System.Environment.TickCount % 3; // 0, 1, 2
                break;
            case 2: // Medium
                m_DebrisType = (System.Environment.TickCount % 2) + 3; // 3, 4
                break;
            case 3: // Large
                m_DebrisType = (System.Environment.TickCount % 2) + 5; // 5, 6
                break;
            default:
                m_DebrisType = -1;
                OnDeath();
                return;
        }
        m_DebrisObject[m_DebrisType].SetActive(true);
        
        m_FadeOutAnimation = FadeOutAnimation();
        StartCoroutine(m_FadeOutAnimation);
    }

    private IEnumerator FadeOutAnimation() {
        float init_alpha = m_Materials[m_DebrisType].color.a;
        int frame = m_LifeTime * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_fade = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float alpha = Mathf.Lerp(init_alpha, 0f, t_fade);
            Color color_tmp = m_Materials[m_DebrisType].color;
            m_Materials[m_DebrisType].color = new Color(color_tmp.r, color_tmp.g, color_tmp.b, alpha);
            yield return new WaitForMillisecondFrames(0);
        }
        
        OnDeath();
        yield break;
    }

    public void OnDeath() {
        m_Materials[m_DebrisType].SetColor("_Color", Color.white);
        
        if (m_FadeOutAnimation != null) {
            StopCoroutine(m_FadeOutAnimation);
        }
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.DEBRIS);
    }

    private void DeactivateAllChildren() {
        for (int i = 0; i < m_DebrisObject.Length; i++) {
            m_DebrisObject[i].SetActive(false);
        }
    }
}

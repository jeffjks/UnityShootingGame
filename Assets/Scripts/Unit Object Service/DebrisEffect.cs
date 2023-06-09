using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DebrisEffect : MonoBehaviour, IObjectPooling
{
    public GameObject[] m_DebrisObject;
    public string m_ObjectName;
    public int m_LifeTime;
    public Collider2D m_Collider2D; // 지상 아이템 콜라이더 보정 및 충돌 체크

    private Vector2 m_Position2D;
    private Material[] m_Materials;
    private int m_DebrisIndex = -1;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;
    private IEnumerator m_FadeOutAnimation;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        
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
        m_Position2D = BackgroundCamera.GetScreenPosition(transform.position);
        m_Collider2D.transform.position = m_Position2D;
    }
    
    public void OnStart(DebrisType debrisType)
    {
        DeactivateAllChildren();
        
        m_DebrisIndex = GetDebrisIndex(debrisType);
        m_DebrisObject[m_DebrisIndex].SetActive(true);
        m_Materials[m_DebrisIndex].color = Color.white;
        
        m_FadeOutAnimation = FadeOutAnimation();
        StartCoroutine(m_FadeOutAnimation);
    }

    private int GetDebrisIndex(DebrisType debrisType) {
        System.Random rand = new System.Random();
        switch(debrisType) {
            case DebrisType.Small:
                return rand.Next(3); // 0, 1, 2
            case DebrisType.Medium:
                return rand.Next(2) + 2; // 3, 4
            case DebrisType.Large:
                return rand.Next(2) + 5; // 5, 6
            default:
                return -1;
        }
    }

    private IEnumerator FadeOutAnimation() {
        float init_alpha = m_Materials[m_DebrisIndex].color.a;
        int frame = m_LifeTime * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_fade = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float alpha = Mathf.Lerp(init_alpha, 0f, t_fade);
            Color color_tmp = m_Materials[m_DebrisIndex].color;
            m_Materials[m_DebrisIndex].color = new Color(color_tmp.r, color_tmp.g, color_tmp.b, alpha);
            yield return new WaitForMillisecondFrames(0);
        }
        
        ReturnToPool();
        yield break;
    }

    public void ReturnToPool() {
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

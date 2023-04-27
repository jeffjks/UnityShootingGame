using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyColorBlender : MonoBehaviour
{
    //public EnemyUnit m_EnemyUnit;
    public EnemyHealth m_EnemyHealth;

    private Material[] m_Materials;
    private Material[] m_MaterialsAll;
    private int m_TakingDamageTimer;
    private int m_LowHealthBlinkTimer;
    private Color[] m_DefaultAlbedo;
    private Color m_DamagingAlbedo = new Color(0.64f, 0.64f, 1f, 1f); // blue
    private bool m_IsDamaging = false;
    private IEnumerator m_LowHealthBlendEffect;
    private IEnumerator m_DamagingBlendEffect;

    private const int DAMAGING_BLEND_TIMER = 67;

/*
    private void Awake()
    {
        m_MaterialsAll = GetAllMetrials();
        m_Materials = GetMaterials();
    }

    private void OnEnable()
    {
        if (m_EnemyUnit.m_EnemyType != EnemyType.Zako || (1 << gameObject.layer & Layer.AIR) != 0) { // 지상 자코가 아닐 경우
            m_EnemyHealth.Action_StartInteractable += StartInteractableEffect;
        }
        m_EnemyHealth.Action_LowHealthState += StartLowHealthBlendEffect;
        m_EnemyHealth.Action_DamagingBlend += StartDamagingBlendEffect;
        m_EnemyHealth.Action_OnDying += DyingImageBlend;
    }

    private void OnDisable()
    {
        if (m_EnemyUnit.m_EnemyType != EnemyType.Zako || (1 << gameObject.layer & Layer.AIR) != 0) { // 지상 자코가 아닐 경우
            m_EnemyHealth.Action_StartInteractable -= StartInteractableEffect;
        }
        m_EnemyHealth.Action_LowHealthState -= StartLowHealthBlendEffect;
        m_EnemyHealth.Action_DamagingBlend -= StartDamagingBlendEffect;
        m_EnemyHealth.Action_OnDying -= DyingImageBlend;

        StopAllCoroutines();
    }

    private Material[] GetAllMetrials() { // 무적 해제, 사망 이펙트 용 (전체 Materials)
        if (m_EnemyUnit.m_Collider2D.Length == 0)
            return new Material[0];

        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        Material[] mat = new Material[meshRenderers.Length];
        m_DefaultAlbedo = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++) {
            mat[i] = meshRenderers[i].material;
            m_DefaultAlbedo[i] = meshRenderers[i].material.color;
        }
        return mat;
    }

    private Material[] GetMaterials() {
        if (m_EnemyUnit.m_Collider2D.Length == 0) {
            if (m_EnemyUnit.m_ChildEnemies.Length == 0)
                return new Material[0];
        }
        if (m_EnemyHealth.m_HealthType == HealthType.Share)
            return new Material[0];

        for (int i = 0; i < m_EnemyUnit.m_ChildEnemies.Length; i++) {
            m_EnemyUnit.m_ChildEnemies[i].gameObject.SetActive(false);
        }
        
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        Material[] mat = new Material[meshRenderers.Length];
        m_DefaultAlbedo = new Color[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++) {
            mat[i] = meshRenderers[i].material;
            m_DefaultAlbedo[i] = meshRenderers[i].material.color;
        }

        for (int i = 0; i < m_EnemyUnit.m_ChildEnemies.Length; i++) {
            m_EnemyUnit.m_ChildEnemies[i].gameObject.SetActive(true);
        }
        return mat;
    }*/

    private void StartInteractableEffect() {
        StartCoroutine(InteractableEffect());
    }

    private IEnumerator InteractableEffect() { // 무적 해제 이펙트
        float color = 0.82f; // white
        while (color > 0f) {
            color -= 0.04f*Time.deltaTime*60f;
            for (int i = 0; i < m_MaterialsAll.Length; i++) {
                m_MaterialsAll[i].SetColor("_EmissionColor", new Color(color, color, color, 1f));
                m_MaterialsAll[i].EnableKeyword("_EMISSION");
            }
            yield return new WaitForFrames(0);
        }
        yield break;
    }


    private void StartDamagingBlendEffect() {
        if (m_DamagingBlendEffect != null)
            StopCoroutine(m_DamagingBlendEffect);
        m_DamagingBlendEffect = DamagingBlendEffect();
        StartCoroutine(DamagingBlendEffect());
    }

    private IEnumerator DamagingBlendEffect() {
        m_IsDamaging = true;
        ImageBlend(m_DamagingAlbedo);
        yield return new WaitForFrames(2);
        m_IsDamaging = false;
        ImageBlend(m_DefaultAlbedo);
        yield break;
    }

    private void StartLowHealthBlendEffect() {
        m_LowHealthBlendEffect = LowHealthBlendEffect();
        StartCoroutine(m_LowHealthBlendEffect);
    }

    private IEnumerator LowHealthBlendEffect() {
        while (true) {
            ImageBlend(Color.red);
            yield return new WaitForMillisecondFrames(120);
            if (m_IsDamaging) {
                ImageBlend(m_DamagingAlbedo);
            }
            else {
                ImageBlend(m_DefaultAlbedo);
            }
        }
    }
    

    private void DyingImageBlend() {
        ImageBlend(Color.red);
    }

    private void ImageBlend(Color target_color) {
        for (int i = 0; i < m_Materials.Length; i++) {
            if (m_Materials[i] != null)
                m_Materials[i].color = target_color;
        }
    }

    private void ImageBlend(Color[] target_color) { // Overload
        for (int i = 0; i < m_Materials.Length; i++) {
            if (m_Materials[i] != null)
                m_Materials[i].color = target_color[i];
        }
    }
}

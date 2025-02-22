using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ColorChannelLevel {
    LowHealthState,
    DamagingState
}

[RequireComponent(typeof(EnemyHealth))]
public class EnemyColorBlender : MonoBehaviour
{
    private EnemyHealth m_EnemyHealth;
    private EnemyDeath m_EnemyDeath;

    private Material[] m_Materials;
    private Material[] m_MaterialsAll;
    private int m_DamagingBlendTimer;
    private Color[] m_DefaultAlbedo;
    private readonly Color m_DamagingAlbedo = new (0.64f, 0.64f, 1f, 1f); // blue
    private readonly Color m_LowHealthAlbedo = Color.red;
    private IEnumerator m_LowHealthBlendEffect;
    private readonly BitArray m_ColorBitArray = new (2);

    private const int DAMAGING_BLEND_FRAME = 3;


    private void Awake()
    {
        m_EnemyHealth = GetComponent<EnemyHealth>();
        m_EnemyDeath = GetComponent<EnemyDeath>();
        InitMaterials();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        CountdownDamagingBlendEffectTimer();
    }

    private void OnEnable()
    {
        m_EnemyHealth.Action_LowHealthState += StartLowHealthBlendEffect;
        m_EnemyHealth.Action_DamagingBlend += SetDamagingBlendEffectTimer;
        m_EnemyDeath.Action_OnKilled += DyingImageBlend;
    }

    private void OnDisable()
    {
        m_EnemyHealth.Action_LowHealthState -= StartLowHealthBlendEffect;
        m_EnemyHealth.Action_DamagingBlend -= SetDamagingBlendEffectTimer;
        m_EnemyDeath.Action_OnKilled -= DyingImageBlend;

        StopAllCoroutines();
    }

    private void InitMaterials() {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_MaterialsAll = new Material[meshRenderers.Length];
        List<GameObject> inactivatedGameObjects = new List<GameObject>();

        for (int i = 0; i < meshRenderers.Length; i++) {
            m_MaterialsAll[i] = meshRenderers[i].material;
        }

        EnemyHealth[] enemyHealths = GetComponentsInChildren<EnemyHealth>();
        for (int i = 0; i < enemyHealths.Length; ++i) {
            if (enemyHealths[i].transform == transform) { // 자기자신 제외
                continue;
            }
            if (enemyHealths[i].m_HealthType != HealthType.Share) { // ShareHealth가 아닐 경우 미포함
                enemyHealths[i].gameObject.SetActive(false);
                inactivatedGameObjects.Add(enemyHealths[i].gameObject); // 비활성화 오브젝트 목록 저장
            }
        }
        
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        m_Materials = new Material[meshRenderers.Length];
        m_DefaultAlbedo = new Color[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++) {
            m_Materials[i] = meshRenderers[i].material;
            
            if (m_Materials[i].HasProperty("_Color")) {
                m_DefaultAlbedo[i] = m_Materials[i].color;
            }
            else {
                m_DefaultAlbedo[i] = Color.white;
            }
        }
        
        foreach (GameObject value in inactivatedGameObjects) { // 복구
            value.SetActive(true);
        }
    }

    public void StartInteractableEffect() {
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
            yield return new WaitForFrames(1);
        }
        yield break;
    }


    private void SetDamagingBlendEffectTimer() {
        m_DamagingBlendTimer = DAMAGING_BLEND_FRAME;
        SetColorChannel(ColorChannelLevel.DamagingState, true);
    }

    private void CountdownDamagingBlendEffectTimer() {
        if (m_DamagingBlendTimer > 0) {
            m_DamagingBlendTimer--;
        }
        else {
            return;
        }

        if (m_DamagingBlendTimer == 0) {
            SetColorChannel(ColorChannelLevel.DamagingState, false);
        }
    }

    private void StartLowHealthBlendEffect() {
        m_LowHealthBlendEffect = LowHealthBlendEffect();
        StartCoroutine(m_LowHealthBlendEffect);
    }

    private IEnumerator LowHealthBlendEffect() {
        while (true) {
            SetColorChannel(ColorChannelLevel.LowHealthState, true);
            yield return new WaitForSeconds(0.12f);
            SetColorChannel(ColorChannelLevel.LowHealthState, false);
            yield return new WaitForSeconds(0.38f);
        }
    }
    

    private void DyingImageBlend() {
        if (m_LowHealthBlendEffect != null) {
            StopCoroutine(m_LowHealthBlendEffect);
        }
        SetColorChannel(ColorChannelLevel.LowHealthState, true);
    }

    private void SetColorChannel(ColorChannelLevel level, bool state) {
        m_ColorBitArray.Set((int) level, state);

        if (m_ColorBitArray.Get((int) ColorChannelLevel.LowHealthState)) {
            ImageBlend(m_LowHealthAlbedo);
            return;
        }

        if (m_ColorBitArray.Get((int) ColorChannelLevel.DamagingState)) {
            ImageBlend(m_DamagingAlbedo);
            return;
        }
        ImageBlend(m_DefaultAlbedo);
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

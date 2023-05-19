using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffecter : MonoBehaviour // 흰색 점멸 이펙트
{
    public GameObject m_WE;
    private SpriteRenderer m_WESpriteRenderer;
    private Transform m_WETransform;
    private Vector2 m_MaxSize;
    private const float WIDTH_SCALER = 1.5625f; // WIDTH

    void Awake()
    {
        Vector3 pos = m_WE.transform.position;
        m_WE.transform.position = new Vector3(pos.x, pos.y, Depth.WHITE_EFFECT);
        m_WESpriteRenderer = m_WE.GetComponent<SpriteRenderer>();
        m_WETransform = m_WE.transform;
        m_MaxSize = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT) * WIDTH_SCALER;
    }
    
    void OnEnable()
    {
        m_WE.SetActive(false);
    }

    void OnDisable() {
        StopAllCoroutines();
    }

    public void PlayWhiteEffect(bool isLarge) {
        if (isLarge) {
            StartCoroutine(WhiteEffectLarge());
        }
        else {
            StartCoroutine(WhiteEffectSmall());
        }
    }
    
    private void SetAlpha(float alpha) {
        m_WESpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
    }
    
    private void SetXscale(float xscale) {
        m_WETransform.localScale = new Vector2(xscale, m_MaxSize.y);
    }

    private IEnumerator WhiteEffectLarge() {
        m_WETransform.localScale = new Vector2(0f, m_MaxSize.y);
        m_WE.SetActive(true);
        float alpha = 1f, xscale = 0f;
        SetAlpha(alpha);
        while (m_WETransform.localScale.x < m_MaxSize.x) {
            SetXscale(Mathf.Lerp(0f, m_MaxSize.x, xscale += 0.08f));
            yield return new WaitForFrames(0);
        }
        while (alpha > 0f) {
            SetAlpha(alpha);
            alpha -= 0.03f;
            yield return new WaitForFrames(0);
        }
        alpha = 0f;
        SetAlpha(alpha);
        m_WE.SetActive(false);
        yield break;
    }

    private IEnumerator WhiteEffectSmall() {
        m_WETransform.localScale = m_MaxSize;
        m_WE.SetActive(true);
        float alpha = 1f;
        while (alpha > 0f) {
            SetAlpha(alpha);
            alpha -= 0.03f;
            yield return new WaitForFrames(0);
        }
        alpha = 0f;
        SetAlpha(alpha);
        m_WE.SetActive(false);
        yield break;
    }
}
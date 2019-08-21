using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffecter : MonoBehaviour
{
    public GameObject m_WE;
    private SpriteRenderer m_WESpriteRenderer;
    private Transform m_WETransform;
    private Vector2 m_MaxSize;
    
    void Start()
    {
        m_WESpriteRenderer = m_WE.GetComponent<SpriteRenderer>();
        m_WETransform = m_WE.transform;

        m_MaxSize = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT) * 1.5625f;
    }
    
    private void SetAlpha(float alpha) {
        m_WESpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
    }
    
    private void SetXscale(float xscale) {
        m_WETransform.localScale = new Vector2(xscale, m_MaxSize.y);
    }


    public IEnumerator WhiteEffectSmall() {
        m_WETransform.localScale = m_MaxSize;
        m_WE.SetActive(true);
        float alpha = 1f;
        while (alpha > 0f) {
            SetAlpha(alpha);
            alpha -= 0.03f;
            yield return null;
        }
        alpha = 0f;
        SetAlpha(alpha);
        m_WE.SetActive(false);
        yield break;
    }

    public IEnumerator WhiteEffectBig() {
        m_WETransform.localScale = new Vector2(0f, m_MaxSize.y);
        m_WE.SetActive(true);
        float alpha = 1f, xscale = 0f;
        SetAlpha(alpha);
        while (m_WETransform.localScale.x < m_MaxSize.x) {
            SetXscale(Mathf.Lerp(0f, m_MaxSize.x, xscale += 0.08f));
            yield return null;
        }
        while (alpha > 0f) {
            SetAlpha(alpha);
            alpha -= 0.03f;
            yield return null;
        }
        alpha = 0f;
        SetAlpha(alpha);
        m_WE.SetActive(false);
        yield break;
    }
}
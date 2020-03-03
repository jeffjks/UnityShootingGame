using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour, CanDeath
{
    public TextMesh m_TextMesh;
    
    private IEnumerator m_BlinkEffect, m_FadeOutEffect;
    private PoolingManager m_PoolingManager = null;
    private float m_Hspeed;

    void Awake()
    {
        m_PoolingManager = PoolingManager.instance_op;
    }

    void OnEnable()
    {
        m_Hspeed = 0.6f;
        m_TextMesh.color = new Color(0.3254902f, 0.8666667f, 0.9137255f, 1f);

        m_BlinkEffect = BlinkEffect();
        m_FadeOutEffect = FadeOutEffect();
        StartCoroutine(m_BlinkEffect);
        StartCoroutine(m_FadeOutEffect);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x + m_Hspeed*Time.deltaTime, pos.y, Depth.SCORE_TEXT);
            m_Hspeed += 4f*Time.deltaTime;
    }

    private IEnumerator BlinkEffect() {
        while (true) {
            m_TextMesh.color = new Color(0.3254902f, 0.8666667f, 0.9137255f, m_TextMesh.color.a);
            yield return new WaitForSeconds(0.12f);
            m_TextMesh.color = new Color(1f, 1f, 1f, m_TextMesh.color.a);
            yield return new WaitForSeconds(0.08f);
        }
    }

    private IEnumerator FadeOutEffect() {
        yield return new WaitForSeconds(0.5f);
        float alpha = 1f;
        while (alpha > 0f) {
            m_TextMesh.color = new Color(m_TextMesh.color.r, m_TextMesh.color.g, m_TextMesh.color.b, alpha);
            alpha -= 3f*Time.deltaTime;
            yield return null;
        }
        OnDeath();
        yield break;
    }

    public void OnDeath() {
        if (m_BlinkEffect != null)
            StopCoroutine(m_BlinkEffect);
        if (m_FadeOutEffect != null)
            StopCoroutine(m_FadeOutEffect);
        m_PoolingManager.PushToPool("ScoreText", gameObject, PoolingParent.SCORE_TEXT);
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour, IObjectPooling
{
    private TextMeshProUGUI _textUI;
    private Animator _animator;
    
    //private IEnumerator m_BlinkEffect, m_FadeOutEffect;
    //private float _hSpeed;
    //private const float SPEED_START = 0.6f;
    //private const float SPEED_ACCEL = 4f;
    private readonly int _textLeft = Animator.StringToHash("Text_Left");
    private readonly int _textRight = Animator.StringToHash("Text_Right");

    void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    public void OnStart(Vector3 pos, string text, bool dir)
    {
        transform.position = pos;
        _textUI.text = text;
        //_hSpeed = SPEED_START;
        
        if (dir) {
            _animator.SetTrigger(_textLeft);
        }
        else {
            _animator.SetTrigger(_textRight);
        }
        
        //m_TextMesh.color = new Color(0.3254902f, 0.8666667f, 0.9137255f, 1f);

        //m_BlinkEffect = BlinkEffect();
        //m_FadeOutEffect = FadeOutEffect();
        //StartCoroutine(m_BlinkEffect);
        //StartCoroutine(m_FadeOutEffect);
    }
/*
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
            yield return new WaitForFrames(0);
        }
        ReturnToPool();
        yield break;
    }*/

    public void ReturnToPool() {
        //if (m_BlinkEffect != null)
        //    StopCoroutine(m_BlinkEffect);
        //if (m_FadeOutEffect != null)
        //    StopCoroutine(m_FadeOutEffect);
        PoolingManager.PushToPool("ScoreText", gameObject, PoolingParent.ScoreText);
    }
}

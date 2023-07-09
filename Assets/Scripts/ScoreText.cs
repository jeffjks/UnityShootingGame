using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour, IObjectPooling
{
    private TextMeshProUGUI _textUI;
    private Animator _animator;
    private Color _defaultTextColor;
    private const int BLINK_FRAME_WAIT = 25;
    
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
        _defaultTextColor = _textUI.color;
    }

    public void OnStart(Vector3 pos, string text, float timeScale, bool dir)
    {
        transform.position = pos;
        transform.localScale = new Vector3(1f, 1f, 1f);
        _textUI.text = text;
        
        _animator.SetTrigger(dir ? _textLeft : _textRight);
        _animator.speed = timeScale;
        StartCoroutine(BlinkEffect());
    }
    private IEnumerator BlinkEffect() {
        yield return new WaitForFrames(BLINK_FRAME_WAIT);
        while (true)
        {
            _textUI.color = _defaultTextColor;
            yield return new WaitForSeconds(0.12f);
            _textUI.color = Color.white;
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void ReturnToPool() {
        StopAllCoroutines();
        PoolingManager.PushToPool("ScoreText", gameObject, PoolingParent.ScoreText);
    }
}

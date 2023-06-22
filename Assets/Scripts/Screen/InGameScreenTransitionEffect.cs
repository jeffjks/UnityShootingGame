using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameScreenTransitionEffect : MonoBehaviour
{
    public Image m_TransitionImage;
    public int Delay { get; set; }
    
    private RectTransform _rectTransform;
    private const int WIDTH = 135;
    private const int HEIGHT = 135;

    private float _alpha;
    private float Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            SetTransitionsAlpha(_alpha);
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(WIDTH, HEIGHT);
    }

    private void OnEnable()
    {
        InitFadeOut();
    }

    public void PlayFadeOut(float duration, Action callback) {
        InitFadeOut();
        StartCoroutine(FadeOut(duration, callback));
    }

    public void PlayTransitionIn(Action callback) {
        InitTransitionIn();
        StartCoroutine(TransitionIn(callback));
    }

    private void InitFadeOut()
    {
        StopAllCoroutines();
        m_TransitionImage.color = Color.clear;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void InitTransitionIn()
    {
        StopAllCoroutines();
        m_TransitionImage.color = Color.black;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private IEnumerator FadeOut(float duration, Action callback)
    {
        if (Mathf.Approximately(duration, 0f))
        {
            Alpha = 0f;
        }

        for (float i = Alpha; (i - Time.deltaTime / duration) > 0; i -= Time.deltaTime / duration)
        {
            Alpha = i;
            yield return null;
        }
        callback?.Invoke();
    }

    private IEnumerator TransitionIn(Action callback) {
        int duration = 660;
        //int delay = 1000 - (int) (transform.position.y*1000f/12f) + Random.Range(1000, 300);
        yield return new WaitForMillisecondFrames(Delay + Random.Range(0, 500));

        float init_scale_x = transform.localScale.x;
        int frame = duration * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_scale = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            float localScale_x = Mathf.Lerp(init_scale_x, 0f, t_scale);
            Vector3 tempScale = transform.localScale;
            tempScale.x = localScale_x;
            transform.localScale = tempScale;
            yield return new WaitForMillisecondFrames(0);
        }
        callback?.Invoke();
        //gameObject.SetActive(false);
    }

    private void SetTransitionsAlpha(float alpha)
    {
        Color newColor = m_TransitionImage.color;
        newColor.a = alpha;
        m_TransitionImage.color = newColor;
    }

    public void SetRotationState()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    public DebrisEffect m_DebrisEffect;
    public SpriteRenderer[] m_SpriteRenderers;

    private IEnumerator _fadeOutCoroutine;
    private const int LIFE_TIME = 24000;

    private void OnEnable()
    {
        _fadeOutCoroutine = FadeOutAnimation();
        StartCoroutine(_fadeOutCoroutine);
    }

    private IEnumerator FadeOutAnimation() {
        var initAlpha = 1f;
        var frame = LIFE_TIME * Application.targetFrameRate / 1000;
        for (var i = 0; i < frame; ++i) {
            float t_fade = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float alpha = Mathf.Lerp(initAlpha, 0f, t_fade);
            foreach (var sprite in m_SpriteRenderers)
            {
                var colorTmp = sprite.color;
                colorTmp.a = alpha;
                sprite.color = colorTmp;
            }
            yield return new WaitForMillisecondFrames(0);
        }
        m_DebrisEffect.ReturnToPool();
    }
}

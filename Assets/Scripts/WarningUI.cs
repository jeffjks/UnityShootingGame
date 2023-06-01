using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    public GameObject m_WarningSign;
    public Image m_WarningText;
    public Material m_WarningSignMat;

    private const int ANIMATION_TIME_START = 250;
    private const int ANIMATION_TIME_WHITE = 333;
    private const int ANIMATION_TIME_END = 250;
    private const int ANIMATION_TIME_BLINK = 250;
    private const float BLINK_ALPHA_GAP = 0.4f;

    private void Awake()
    {
        StageManager.Action_BossWarningSign += PlayWarningSign;
    }
    
    private void PlayWarningSign() {
        AudioService.PlaySound("BossAlert1");
        m_WarningSign.SetActive(true);
        StartCoroutine(WarningSignAnimation());
        StartCoroutine(WarningTextAnimation());
    }

    private IEnumerator WarningSignAnimation()
    {
        var frame = 0;
        //AudioService.PlaySound("BossAlert1");
        
        SetEmissionColor(0f);
        SetLocalScaleY(0f);
        
        frame = ANIMATION_TIME_START * Application.targetFrameRate / 1000; // Increase yScale
        for (int i = 0; i < frame; ++i) {
            float inter = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            SetLocalScaleY(inter);
            yield return new WaitForMillisecondFrames(0);
        }
        
        frame = ANIMATION_TIME_WHITE * Application.targetFrameRate / 1000; // White Blink Effect
        for (int i = 0; i < frame; ++i) {
            float inter = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            SetEmissionColor(1f - inter);
            yield return new WaitForMillisecondFrames(0);
        }
        
        frame = ANIMATION_TIME_END * Application.targetFrameRate / 1000; // Increase yScale
        for (int i = 0; i < frame; ++i) {
            float inter = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            SetLocalScaleY(1f - inter);
            yield return new WaitForMillisecondFrames(0);
        }

        StopAllCoroutines();
        m_WarningSign.SetActive(false);
    }

    private IEnumerator WarningTextAnimation()
    {
        while (true)
        {
            int frame = ANIMATION_TIME_BLINK * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float inter = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
                SetTextAlpha(1f - inter * BLINK_ALPHA_GAP);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }

    private void SetEmissionColor(float value)
    {
        m_WarningSignMat.SetColor("_Emission", new Color(value, value, value, 1f));
    }

    private void SetLocalScaleY(float newValue)
    {
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.y = newValue;
        transform.localScale = newLocalScale;
    }

    private void SetTextAlpha(float alpha)
    {
        Color textColor = m_WarningText.color;
        textColor.a = alpha;
        m_WarningText.color = textColor;
    }
}

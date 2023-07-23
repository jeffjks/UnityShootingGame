using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningSign : MonoBehaviour
{
    public Material m_WarningSignMat;

    public void StartBlinkAnimation(int duration)
    {
        StartCoroutine(BlinkAnimation(duration));
    }

    private IEnumerator BlinkAnimation(int duration)
    {
        int frame = duration * Application.targetFrameRate / 1000; // White Blink Effect
        for (int i = 0; i < frame; ++i) {
            float inter = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            SetEmissionColor(1f - inter);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private void SetEmissionColor(float value)
    {
        m_WarningSignMat.SetColor("_Emission", new Color(value, value, value, 1f));
    }

    private void StopWarningSign()
    {
        gameObject.SetActive(false);
    }
}

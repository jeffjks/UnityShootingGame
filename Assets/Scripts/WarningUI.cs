using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningUI : MonoBehaviour
{
    public GameObject m_WarningSign;
    public Image m_WarningText;
    public Material m_WarningSignMat;

    private void Awake()
    {
        StageManager.Action_BossWarningSign += PlayWarningSign;
    }
    
    private void PlayWarningSign() {
        AudioService.PlaySound("BossAlert1");
        m_WarningSign.SetActive(true);
        //StartCoroutine(WarningSignAnimation());
        //StartCoroutine(WarningTextAnimation());
    }

    public void StartBlinkAnimation(int duration)
    {
        StartCoroutine(BlinkAnimation(duration));
    }

    private IEnumerator BlinkAnimation(int duration)
    {
        int frame = duration * Application.targetFrameRate / 1000; // White Blink Effect
        for (int i = 0; i < frame; ++i) {
            float inter = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            SetEmissionColor(1f - inter);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private void SetEmissionColor(float value)
    {
        m_WarningSignMat.SetColor("_Emission", new Color(value, value, value, 1f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenEffectFadeOut : MonoBehaviour // 게임 바깥에서의 효과
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer = null;

    private float m_Delta;
    
    void Update()
    {
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, m_Delta);
        if (m_Delta < 1f) {
            m_Delta += 0.5f * Time.deltaTime;
        }
        else {
            if (SystemManager.GameMode == GameMode.Training) {
                SceneManager.LoadScene("Stage" + (SystemManager.TrainingInfo.stage + 1));
            }
            else {
                SceneManager.LoadScene("Stage1");
            }
        }
    }
}

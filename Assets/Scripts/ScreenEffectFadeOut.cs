using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenEffectFadeOut : MonoBehaviour
{
    [SerializeField] private MainMenuMusicController m_MainMenuMusicController = null;
    [SerializeField] private SpriteRenderer m_SpriteRenderer = null;

    private float m_Delta;
    
    void Update()
    {
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, m_Delta);
        if (m_Delta < 1f) {
            m_Delta += 0.5f * Time.deltaTime;
            m_MainMenuMusicController.SetSelectMusicVolume(m_Delta);
        }
        else {
            m_MainMenuMusicController.StopAllMusic();
            SceneManager.LoadScene("Stage1");
        }
    }
}

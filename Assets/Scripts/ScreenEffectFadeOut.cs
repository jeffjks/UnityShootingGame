using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenEffectFadeOut : MonoBehaviour // 게임 바깥에서의 효과
{
    [SerializeField] private MainMenuMusicController m_MainMenuMusicController = null;
    [SerializeField] private SpriteRenderer m_SpriteRenderer = null;

    private GameManager m_GameManager = null;

    private float m_Delta;

    void Start() {
        m_GameManager = GameManager.instance_gm;
    }
    
    void Update()
    {
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, m_Delta);
        if (m_Delta < 1f) {
            m_Delta += 0.5f * Time.deltaTime;
            m_MainMenuMusicController.SetSelectMusicVolume(m_Delta);
        }
        else {
            m_MainMenuMusicController.StopAllMusic();
            if (m_GameManager.m_PracticeState) {
                SceneManager.LoadScene("Stage" + (m_GameManager.m_PracticeInfo.m_Stage + 1));
            }
            else {
                SceneManager.LoadScene("Stage1");
            }
        }
    }
}

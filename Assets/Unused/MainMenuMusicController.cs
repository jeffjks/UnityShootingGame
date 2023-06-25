using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioMain = null;
    [SerializeField] private AudioSource m_AudioSelect = null;

    private sbyte m_MusicState = -1;

    private void PlayMainMusic() {
        if (m_MusicState != 0) {
            m_MusicState = 0;
            m_AudioSelect.Stop();
            m_AudioMain.Play();
        }
    }

    private void PlaySelectMusic() {
        if (m_MusicState != 1) {
            m_MusicState = 1;
            m_AudioSelect.volume = 1f;
            m_AudioMain.Stop();
            m_AudioSelect.Play();
        }
    }

    private void SetSelectMusicVolume(float value) {
        if (m_AudioMain.isPlaying)
            m_AudioMain.volume = 1 - value;
        else if (m_AudioSelect.isPlaying)
            m_AudioSelect.volume = 1 - value;
    }

    private void StopAllMusic() {
        m_MusicState = -1;
        m_AudioSelect.Stop();
        m_AudioMain.Stop();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED SCRIPT

public class SoundManager : MonoBehaviour
{
    private AudioSource[] m_AudioSource;
    
    void Awake()
    {
        m_AudioSource = gameObject.GetComponents<AudioSource>();
    }

    public void PlayAudio(AudioClip clip) {
        if (clip == null)
            return;
            
        for (int i = 0; i < m_AudioSource.Length; i++) {
            if (clip == m_AudioSource[i].clip) {
                m_AudioSource[i].Play();
                return;
            }
        }
    }
}

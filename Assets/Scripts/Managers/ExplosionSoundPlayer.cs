using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class ExplosionSoundPlayer : MonoBehaviour
{
    public AudioMixerGroup m_AudioMixerGroup;
    public AudioClip[] m_AudioClip;
    private Dictionary<ExplAudioType, AudioSource> m_AudioMatcher = new Dictionary<ExplAudioType, AudioSource>();
    
    void Awake()
    {
        AudioSource[] audioSource = new AudioSource[m_AudioClip.Length];
        for (int i = 0; i < m_AudioClip.Length; ++i) {
            audioSource[i] = gameObject.AddComponent<AudioSource>();
            audioSource[i].clip = m_AudioClip[i];
            audioSource[i].playOnAwake = false;
            audioSource[i].outputAudioMixerGroup = m_AudioMixerGroup;
        }

        m_AudioMatcher[ExplAudioType.AirMedium_1] = audioSource[0];
        m_AudioMatcher[ExplAudioType.AirMedium_2] = audioSource[1];
        m_AudioMatcher[ExplAudioType.AirSmall] = audioSource[2];
        m_AudioMatcher[ExplAudioType.GroundMedium] = audioSource[3];
        m_AudioMatcher[ExplAudioType.GroundSmall] = audioSource[4];
        m_AudioMatcher[ExplAudioType.Huge_1] = audioSource[5];
        m_AudioMatcher[ExplAudioType.Huge_2] = audioSource[6];
        m_AudioMatcher[ExplAudioType.Large] = audioSource[7];
        m_AudioMatcher[ExplAudioType.Player] = audioSource[8];
    }

    public void PlayAudio(ExplAudioType audioType) {
        if (audioType == ExplAudioType.None)
            return;
        
        if (m_AudioMatcher.ContainsKey(audioType)) {
            m_AudioMatcher[audioType].Play();
        }
        else {
            Debug.LogError("There is no matching audio type");
            return;
        }
    }
}

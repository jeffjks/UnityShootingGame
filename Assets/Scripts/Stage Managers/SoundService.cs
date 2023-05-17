using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class SoundService : MonoBehaviour
{
    public string[] m_SceneString;
    public MusicDatas[] m_MusicDatas;
    public MusicInfoDatas m_MusicInfoDatas;
    public AudioMixerGroup m_AudioMixerGroup;
    
    private Dictionary<string, MusicInfo> m_MusicInfoDict = new Dictionary<string, MusicInfo>();
    private Dictionary<string, MusicDatas> m_MusicDataDict = new Dictionary<string, MusicDatas>();
    private string m_CurrentScene;
    private Dictionary<string, AudioSource> m_AudioSourceDict = new Dictionary<string, AudioSource>();
    private string m_CurrentMusic = String.Empty;

    private float _loopStartPoint;
    private float _loopEndPoint;

    private static SoundService instance_ss;
    
    private void Start()
    {
        if (instance_ss != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_ss = this;

        for (int i = 0; i < m_SceneString.Length; ++i)
        {
            m_MusicDataDict[m_SceneString[i]] = m_MusicDatas[i];
        }

        for (int i = 0; i < m_MusicInfoDatas.musicInfos.Length; ++i)
        {
            string musicName = m_MusicInfoDatas.musicInfos[i].musicName;
            m_MusicInfoDict[musicName] = m_MusicInfoDatas.musicInfos[i];
        }

        LoadMusics("Main");
        
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        LoopMusic();
    }

    public void LoadMusics(string sceneString)
    {
        if (sceneString == m_CurrentScene)
        {
            return;
        }
        
        foreach (KeyValuePair<string, AudioSource> keyValue in m_AudioSourceDict)
        {
            Destroy(keyValue.Value);
        }
        m_AudioSourceDict.Clear();
        
        MusicDatas musicDatas = m_MusicDataDict[sceneString];
        m_CurrentScene = sceneString;
        for (int i = 0; i < musicDatas.musicInfoNames.Length; ++i)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            string audioName = musicDatas.musicInfoNames[i];
            audioSource.outputAudioMixerGroup = m_AudioMixerGroup;
            audioSource.clip = m_MusicInfoDict[audioName].stageMusicAudio;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            m_AudioSourceDict[audioName] = audioSource;
        }
        Debug.Log("Loaded");
    }

    public void PlayMusic(string musicName)
    {
        if (m_CurrentMusic != string.Empty)
        {
            m_AudioSourceDict[m_CurrentMusic].Stop();
        }
        if (m_AudioSourceDict.TryGetValue(musicName, out AudioSource audioSource))
        {
            audioSource.Play();
            m_CurrentMusic = musicName;
        
            _loopStartPoint = m_MusicInfoDict[m_CurrentMusic].loopStartPoint;
            _loopEndPoint = m_MusicInfoDict[m_CurrentMusic].loopEndPoint;
        }
    }

    private void LoopMusic()
    {
        if (_loopEndPoint == 0)
        {
            return;
        }
        if (m_AudioSourceDict[m_CurrentMusic].time > _loopEndPoint) {
            m_AudioSourceDict[m_CurrentMusic].time = _loopStartPoint;
        }
    }

    private void StopMusic()
    {
        m_AudioSourceDict[m_CurrentMusic].Stop();
        m_CurrentMusic = string.Empty;
    }

    private void FadeOutMusic(float duration)
    {
        StartCoroutine(FadingOut(duration));
    }

    private IEnumerator FadingOut(float duration = 3.3f) {
        m_AudioSourceDict[m_CurrentMusic].DOFade(0f, duration);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_AudioSourceDict[m_CurrentMusic]);
        m_AudioSourceDict[m_CurrentMusic].Stop();
    }
}

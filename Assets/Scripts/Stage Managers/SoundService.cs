using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private static Dictionary<string, HashSet<string>> m_SceneMusicDict = new Dictionary<string, HashSet<string>>();
    private static string m_CurrentScene = String.Empty;
    private static Dictionary<string, AudioSource> m_AudioSourceDict = new Dictionary<string, AudioSource>();
    private static string m_CurrentMusic = String.Empty;

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

        for (int i = 0; i < m_MusicDatas.Length; ++i)
        {
            string sceneName = m_SceneString[i];
            MusicDatas musicDatas = m_MusicDatas[i];
            m_SceneMusicDict[sceneName] = new HashSet<string>();
            
            foreach (var musicName in musicDatas.musicInfoNames)
            {
                m_SceneMusicDict[sceneName].Add(musicName);
            }
        }
        m_SceneMusicDict[String.Empty] = new HashSet<string>();

        foreach (var musicInfo in m_MusicInfoDatas.musicInfos)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            string audioName = musicInfo.musicName;
            audioSource.outputAudioMixerGroup = m_AudioMixerGroup;
            audioSource.clip = musicInfo.stageMusicAudio;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            
            m_AudioSourceDict[audioName] = audioSource;
            m_MusicInfoDict[audioName] = musicInfo;
        }

        LoadMusics("Main");
        
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        LoopMusic();
    }

    public static void LoadMusics(string sceneString)
    {
        if (sceneString == m_CurrentScene)
        {
            return;
        }
        // 현재 : m_SceneMusicDict[m_CurrentScene]
        // 신규 : m_SceneMusicDict[sceneString]
        // 겹치는건 유지 현재에만 있는건 Unload, 신규에만 있는건 Load
        HashSet<string> currentMusicNames = m_SceneMusicDict[m_CurrentScene];

        foreach (var musicName in m_SceneMusicDict[sceneString])
        {
            if (!currentMusicNames.Remove(musicName))
            {
                m_AudioSourceDict[musicName].clip.LoadAudioData();
            }
        }

        foreach (var musicName in currentMusicNames)
        {
            m_AudioSourceDict[musicName].clip.UnloadAudioData();
        }
        
        m_CurrentScene = sceneString;
    }

    public static void PlayMusic(string musicName)
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
        if (m_CurrentMusic == String.Empty)
        {
            return;
        }
        if (_loopEndPoint == 0)
        {
            return;
        }
        if (m_AudioSourceDict[m_CurrentMusic].time > _loopEndPoint) {
            m_AudioSourceDict[m_CurrentMusic].time = _loopStartPoint;
        }
    }

    private static void StopMusic()
    {
        if (m_AudioSourceDict.TryGetValue(m_CurrentMusic, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
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

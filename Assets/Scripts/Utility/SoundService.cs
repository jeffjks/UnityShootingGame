using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class SoundService : MonoBehaviour
{
    public GameObject m_GameObjectBGM;
    public GameObject m_GameObjectSFX;
    public string[] m_SceneString;
    public MusicDatas[] m_SO_Musics;
    
    public MusicInfoDatas m_SO_MusicInfo;
    public AudioMixerGroup m_AudioMixerGroupBGM;
    
    public ExplosionSoundEffectDatas m_SO_ExplosionSoundEffect;
    public AudioMixerGroup m_AudioMixerGroupExplosion;
    
    public SoundEffectDatas[] m_SO_SoundEffects;
    public AudioMixerGroup[] m_AudioMixerGroupSFX;
    
    private static Dictionary<string, MusicInfo> m_MusicInfoDict = new Dictionary<string, MusicInfo>();
    private static Dictionary<string, HashSet<string>> m_SceneMusicDict = new Dictionary<string, HashSet<string>>();
    private static string currentScene = String.Empty;
    private static Dictionary<string, AudioSource> m_AudioSourceMusicDict = new Dictionary<string, AudioSource>();
    private static string currentMusic = String.Empty;
    
    private static Dictionary<string, AudioSource> m_AudioSourceSFXDict = new Dictionary<string, AudioSource>();
    private static Dictionary<ExplAudioType, AudioSource> m_AudioSourceExplosionDict = new Dictionary<ExplAudioType, AudioSource>();

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

        InitBGM();
        InitSFX();
        InitExplosionSFX();
        
        DontDestroyOnLoad(gameObject);
    }

    private void InitBGM()
    {
        for (int i = 0; i < m_SO_Musics.Length; ++i)
        {
            string sceneName = m_SceneString[i];
            MusicDatas musicDatas = m_SO_Musics[i];
            m_SceneMusicDict[sceneName] = new HashSet<string>();
            
            foreach (var musicName in musicDatas.musicInfoNames)
            {
                m_SceneMusicDict[sceneName].Add(musicName);
            }
        }
        m_SceneMusicDict[String.Empty] = new HashSet<string>();

        foreach (var musicInfo in m_SO_MusicInfo.musicInfos)
        {
            AudioSource audioSource = m_GameObjectBGM.AddComponent<AudioSource>();
            string audioName = musicInfo.musicName;
            audioSource.outputAudioMixerGroup = m_AudioMixerGroupBGM;
            audioSource.clip = musicInfo.musicAudio;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            
            m_AudioSourceMusicDict[audioName] = audioSource;
            m_MusicInfoDict[audioName] = musicInfo;
        }
    }

    private void InitSFX()
    {
        for (int i = 0; i < m_SO_SoundEffects.Length; ++i)
        {
            foreach (var soundEffectInfo in m_SO_SoundEffects[i].soundEffectInfos)
            {
                AudioSource audioSource = m_GameObjectSFX.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = m_AudioMixerGroupSFX[i];
                audioSource.clip = soundEffectInfo.soundEffectAudio;
                audioSource.playOnAwake = false;

                if (m_AudioSourceMusicDict.ContainsKey(soundEffectInfo.soundEffectName))
                {
                    Debug.LogError("There are duplicated sound effect name!");
                }
                else
                {
                    m_AudioSourceSFXDict[soundEffectInfo.soundEffectName] = audioSource;
                }
            }
        }
    }

    private void InitExplosionSFX()
    {
        foreach (var soundEffectInfo in m_SO_ExplosionSoundEffect.soundEffectInfos)
        {
            AudioSource audioSource = m_GameObjectSFX.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = m_AudioMixerGroupExplosion;
            audioSource.clip = soundEffectInfo.soundEffectAudio;
            audioSource.playOnAwake = false;

            if (m_AudioSourceExplosionDict.ContainsKey(soundEffectInfo.explosionAudioType))
            {
                Debug.LogError("There are duplicated sound effect name!");
            }
            else
            {
                m_AudioSourceExplosionDict[soundEffectInfo.explosionAudioType] = audioSource;
            }
        }
    }
    
    private void Update()
    {
        LoopMusic();
    }

    public static void LoadMusics(string sceneString)
    {
        if (sceneString == currentScene)
        {
            return;
        }
        // 현재 : m_SceneMusicDict[currentScene]
        // 신규 : m_SceneMusicDict[sceneString]
        // 겹치는건 유지 현재에만 있는건 Unload, 신규에만 있는건 Load
        HashSet<string> currentMusicNames = m_SceneMusicDict[currentScene];

        foreach (var musicName in m_SceneMusicDict[sceneString])
        {
            if (!currentMusicNames.Remove(musicName))
            {
                m_AudioSourceMusicDict[musicName].clip.LoadAudioData();
            }
        }

        foreach (var musicName in currentMusicNames)
        {
            m_AudioSourceMusicDict[musicName].clip.UnloadAudioData();
        }
        
        currentScene = sceneString;
    }

    public static void PlayMusic(string musicName)
    {
        if (!m_SceneMusicDict[currentScene].Contains(musicName))
        {
            Debug.LogError($"'{musicName}' is not loaded music in this scene.");
            return;
        }
        
        if (currentMusic != string.Empty)
        {
            m_AudioSourceMusicDict[currentMusic].Stop();
        }
        if (m_AudioSourceMusicDict.TryGetValue(musicName, out AudioSource audioSource))
        {
            audioSource.Play();
            currentMusic = musicName;
        
            instance_ss._loopStartPoint = m_MusicInfoDict[currentMusic].loopStartPoint;
            instance_ss._loopEndPoint = m_MusicInfoDict[currentMusic].loopEndPoint;
        }
    }

    private void LoopMusic()
    {
        if (currentMusic == String.Empty)
        {
            return;
        }
        if (_loopEndPoint == 0)
        {
            return;
        }
        if (m_AudioSourceMusicDict[currentMusic].time > _loopEndPoint) {
            m_AudioSourceMusicDict[currentMusic].time = _loopStartPoint;
        }
    }

    public static void StopMusic()
    {
        if (m_AudioSourceMusicDict.TryGetValue(currentMusic, out AudioSource audioSource))
        {
            audioSource.Stop();
        }
        currentMusic = string.Empty;
    }

    public static void FadeOutMusic(float seconds = 3.3f)
    {
        instance_ss.StartCoroutine(instance_ss.FadingOut(seconds));
    }

    private IEnumerator FadingOut(float seconds) {
        m_AudioSourceMusicDict[currentMusic].DOFade(0f, seconds);

        yield return new WaitForSeconds(seconds);
        DOTween.Kill(m_AudioSourceMusicDict[currentMusic]);
        m_AudioSourceMusicDict[currentMusic].Stop();
    }

    public static void PlaySFX(string soundEffectName)
    {
        if (m_AudioSourceMusicDict.TryGetValue(soundEffectName, out AudioSource audioSource))
        {
            audioSource.Play();
        }
    }

    public static void PlayExplosionSFX(ExplAudioType explAudioType)
    {
        if (m_AudioSourceExplosionDict.TryGetValue(explAudioType, out AudioSource audioSource))
        {
            audioSource.Play();
        }
    }
}

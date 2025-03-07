using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class AudioService : MonoBehaviour
{
    public GameObject m_GameObjectMusic;
    public GameObject m_GameObjectSound;
    public string[] m_SceneString;
    public MusicDatas[] m_SO_Musics;
    
    public MusicInfoDatas m_SO_MusicInfo;
    public AudioMixerGroup m_AudioMixerGroupMusic;
    
    public ExplosionSoundDatas m_SO_ExplosionSound;
    public AudioMixerGroup m_AudioMixerGroupExplosion;
    
    public SoundDatas[] m_SO_Sounds;
    public AudioMixerGroup[] m_AudioMixerGroupSound;
    
    private static Dictionary<string, MusicInfo> m_MusicInfoDict = new ();
    private static Dictionary<string, HashSet<string>> m_SceneMusicDict = new ();
    private static string currentScene = String.Empty;
    private static Dictionary<string, AudioSource> m_AudioSourceMusicDict = new ();
    private static string currentMusic = String.Empty;
    private static bool IsMusicPaused;
    private static bool IsMusicPlaying;
    
    private static Dictionary<string, AudioSource> m_AudioSourceSoundDict = new ();
    private static Dictionary<ExplAudioType, AudioSource> m_AudioSourceExplosionDict = new ();

    private float _loopStartPoint;
    private float _loopEndPoint;

    private static AudioService Instance;
    
    private void Start()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitMusic();
        InitSound();
        InitExplosionSound();
        
        DontDestroyOnLoad(gameObject);
    }

    private void InitMusic()
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
            AudioSource audioSource = m_GameObjectMusic.AddComponent<AudioSource>();
            string audioName = musicInfo.musicName;
            audioSource.outputAudioMixerGroup = m_AudioMixerGroupMusic;
            audioSource.clip = musicInfo.musicAudio;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            
            m_AudioSourceMusicDict[audioName] = audioSource;
            m_MusicInfoDict[audioName] = musicInfo;
        }
    }

    private void InitSound()
    {
        for (int i = 0; i < m_SO_Sounds.Length; ++i)
        {
            foreach (var soundInfo in m_SO_Sounds[i].soundInfos)
            {
                AudioSource audioSource = m_GameObjectSound.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = m_AudioMixerGroupSound[i];
                audioSource.clip = soundInfo.soundAudio;
                audioSource.playOnAwake = false;

                if (m_AudioSourceMusicDict.ContainsKey(soundInfo.soundName))
                {
                    Debug.LogError("There are duplicated sound effect name!");
                }
                else
                {
                    m_AudioSourceSoundDict[soundInfo.soundName] = audioSource;
                }
            }
        }
    }

    private void InitExplosionSound()
    {
        foreach (var soundInfo in m_SO_ExplosionSound.soundInfos)
        {
            AudioSource audioSource = m_GameObjectSound.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = m_AudioMixerGroupExplosion;
            audioSource.clip = soundInfo.soundAudio;
            audioSource.playOnAwake = false;

            if (m_AudioSourceExplosionDict.ContainsKey(soundInfo.explosionAudioType))
            {
                Debug.LogError("There are duplicated sound effect name!");
            }
            else
            {
                m_AudioSourceExplosionDict[soundInfo.explosionAudioType] = audioSource;
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
            Debug.Log("Current scene's musics are already loaded.");
            return;
        }
        // 현재 : m_SceneMusicDict[currentScene]
        // 신규 : m_SceneMusicDict[sceneString]
        // 겹치는건 유지 현재에만 있는건 Unload, 신규에만 있는건 Load
        HashSet<string> currentMusicNames = new (m_SceneMusicDict[currentScene]);

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
            Debug.LogError($"'{musicName}' can not be played in scene '{currentScene}'.");
            return;
        }
        if (currentMusic == musicName)
        {
            Debug.Log($"'{musicName}' is already playing.");
            return;
        }

        if (IsMusicPaused)
            return;
        
        Instance.StopAllCoroutines();
        StopMusic();
        
        if (m_AudioSourceMusicDict.TryGetValue(musicName, out AudioSource audioSource))
        {
            audioSource.Play();
            audioSource.volume = 1f;
            currentMusic = musicName;
        
            Instance._loopStartPoint = m_MusicInfoDict[currentMusic].loopStartPoint;
            Instance._loopEndPoint = m_MusicInfoDict[currentMusic].loopEndPoint;
        }
        else
        {
            Debug.LogError($"There is no music name such as '{musicName}' in dictionary.");
        }

        IsMusicPlaying = true;
    }

    public static void PauseAudio()
    {
        PauseMusic();
        PauseSound();
    }

    public static void UnpauseAudio()
    {
        UnpauseMusic();
        UnpauseSound();
    }

    private static void PauseMusic()
    {
        if (!IsMusicPlaying)
            return;
        
        IsMusicPaused = true;
        Instance.DOPause();
        var audioSource = GetCurrentMusic();
        if (!audioSource)
            return;
        audioSource.Pause();
    }

    private static void UnpauseMusic()
    {
        if (!IsMusicPlaying)
            return;
        
        IsMusicPaused = false;
        Instance.DOPlay();
        var audioSource = GetCurrentMusic();
        if (!audioSource)
            return;
        audioSource.UnPause();
    }

    private static void PauseSound()
    {
        foreach (var keyValue in m_AudioSourceSoundDict)
        {
            AudioSource audioSource = keyValue.Value;
            audioSource.Pause();
        }
    }

    private static void UnpauseSound()
    {
        foreach (var keyValue in m_AudioSourceSoundDict)
        {
            AudioSource audioSource = keyValue.Value;
            audioSource.UnPause();
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
        if (!IsMusicPlaying)
            return;
        if (!IsMusicPaused)
            UnpauseMusic();
        
        Instance.StopAllCoroutines();
        IsMusicPaused = false;
        var audioSource = GetCurrentMusic();
        if (!audioSource)
            return;
        audioSource.Stop();
        currentMusic = string.Empty;
        IsMusicPlaying = false;
    }

    public static void FadeOutMusic(float seconds = 3.3f)
    {
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.FadingOut(seconds));
    }

    private IEnumerator FadingOut(float seconds) {
        var audioSource = GetCurrentMusic();
        if (!audioSource)
            yield break;
        audioSource.volume = 1f;
        audioSource.DOFade(0f, seconds);

        yield return new WaitForSeconds(seconds);
        DOTween.Kill(audioSource);
        StopMusic();
    }
    
    

    public static void PlaySound(string soundName, bool loop = false)
    {
        if (m_AudioSourceSoundDict.TryGetValue(soundName, out AudioSource audioSource))
        {
            audioSource.Play();
            audioSource.loop = loop;
        }
        else
        {
            Debug.LogError($"There is no sound name such as '{soundName}' in dictionary.");
        }
    }

    public static void PlaySound(ExplAudioType explAudioType)
    {
        if (m_AudioSourceExplosionDict.TryGetValue(explAudioType, out AudioSource audioSource))
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError($"There is no explosion type sound such as '{explAudioType}' in dictionary.");
        }
    }

    public static void StopSound(string soundName)
    {
        if (m_AudioSourceSoundDict.TryGetValue(soundName, out AudioSource audioSource))
        {
            if (!audioSource)
                return;
            if (!audioSource.isPlaying)
                return;
            audioSource.Stop();
        }
    }

    public static void StopSound(ExplAudioType explAudioType)
    {
        if (m_AudioSourceExplosionDict.TryGetValue(explAudioType, out AudioSource audioSource))
        {
            if (!audioSource.isPlaying)
            {
                return;
            }
            audioSource.Stop();
        }
    }

    public static void StopAllSound()
    {
        foreach (var keyValuePair in m_AudioSourceSoundDict)
        {
            var audioSource = keyValuePair.Value;
            
            if (!audioSource)
                return;
            if (!audioSource.isPlaying)
                return;
            audioSource.Stop();
        }
        foreach (var keyValuePair in m_AudioSourceExplosionDict)
        {
            var audioSource = keyValuePair.Value;
            
            if (!audioSource)
                return;
            if (!audioSource.isPlaying)
                return;
            audioSource.Stop();
        }
    }

    private static AudioSource GetCurrentMusic()
    {
        if (m_AudioSourceMusicDict.TryGetValue(currentMusic, out AudioSource audioSource))
        {
            return audioSource;
        }
        Debug.LogError($"Can't find current music (Current Music : {currentMusic})");
        return null;
    }
}

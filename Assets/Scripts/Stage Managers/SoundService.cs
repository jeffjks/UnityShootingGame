using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundService : MonoBehaviour
{
    public string[] m_SceneString;
    public MusicDatas[] m_MusicDatas;
    
    private Dictionary<string, MusicDatas> m_MusicDataDict = default;
    private string m_CurrentScene;
    private Dictionary<string, AudioSource> m_MusicDict = default;
    private string m_CurrentMusic;

    public static SoundService instance_ss;

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
        
        MusicDatas musicDatas = m_MusicDataDict[sceneString];
        foreach (KeyValuePair<string, AudioSource> keyValue in m_MusicDict)
        {
            Destroy(keyValue.Value);
        }
        m_MusicDict.Clear();
        
        for (int i = 0; i < musicDatas.backgroundMusics.Length; ++i)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            m_MusicDict[audioSource.name] = audioSource;
        }
    }

    public void PlayMusic(string musicName)
    {
        m_MusicDict[m_CurrentMusic].Stop();
        m_MusicDict[musicName].Play();
    }

    private void LoopMusic()
    {
    }

    private void StopMusic()
    {
        m_MusicDict[m_CurrentMusic].Stop();
    }

    private void FadeOutMusic(float duration)
    {
        StartCoroutine(FadingOut(duration));
    }

    private IEnumerator FadingOut(float duration = 3.3f) {
        m_MusicDict[m_CurrentMusic].DOFade(0f, duration);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_MusicDict[m_CurrentMusic]);
        m_MusicDict[m_CurrentMusic].Stop();
    }
}

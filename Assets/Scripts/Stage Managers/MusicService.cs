using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicService : MonoBehaviour
{
    public StageManager m_StageManager;
    [SerializeField] private StageMusicInfo[] m_StageMusicInfos;

    private AudioSource m_CurrentMusicAudio;
    private int m_CurrentIndex;

    private void Start()
    {
        m_StageManager.Action_PlayMusic += PlayMusic;
        m_StageManager.Action_StopMusic += StopMusic;
        m_StageManager.Action_FadeOutMusic += FadeOutMusic;
        
        PlayMusic(m_CurrentIndex);
    }
    
    void Update()
    {
        LoopMusic();
    }

    private void PlayMusic(int index)
    {
        m_CurrentMusicAudio.Stop();
        m_CurrentIndex = index;
        m_CurrentMusicAudio = m_StageMusicInfos[index].stageMusicAudio;
        m_CurrentMusicAudio.Play();
    }

    private void LoopMusic()
    {
        if (m_CurrentMusicAudio.time > m_StageMusicInfos[m_CurrentIndex].loopEndPoint) {
            m_CurrentMusicAudio.time = m_StageMusicInfos[m_CurrentIndex].loopStartPoint;
        }
    }

    private void StopMusic()
    {
        m_CurrentMusicAudio.Stop();
    }

    private void FadeOutMusic(float duration)
    {
        StartCoroutine(FadingOut(duration));
    }

    private IEnumerator FadingOut(float duration = 3.3f) {
        m_CurrentMusicAudio.DOFade(0f, duration);

        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_CurrentMusicAudio);
        m_CurrentMusicAudio.Stop();
    }
}


using UnityEngine;
using System.Collections;

public class ScreenEffectAnimation : MonoBehaviour { // 화면 전환 효과

    public SpriteRenderer m_SpriteRenderer;
    
    public void PlayTransition() {
        StartCoroutine(Transition());
    }
    
    public void PlayFadeIn() {
        StartCoroutine(FadeIn());
    }
    
    public void PlayFadeOut() {
        StartCoroutine(FadeOut());
    }
    

    private IEnumerator Transition() {
        int duration = 660;
        int duration_frame = duration * Application.targetFrameRate / 1000;

        int delay = 1000 - (int) (transform.position.y*1000f/12f) + Random.Range(1000, 300);

        yield return new WaitForMillisecondFrames(delay);

        for (int i = 0; i < duration_frame; ++i) {
            transform.localScale = new Vector3(1f - 1f/duration_frame*(i+1), 1f, 1f);
            yield return new WaitForMillisecondFrames(0);
        }

        //transform.DOScaleX(0f, duration);
        //yield return new WaitForMillisecondFrames(duration);
        //DOTween.Kill(transform);
        transform.localScale = new Vector3(1f, 1f, 1f);
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
        gameObject.SetActive(false);
        yield break;
    }

    private IEnumerator FadeIn() {
        int duration = 1000;
        int duration_frame = duration * Application.targetFrameRate / 1000;
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);

        for (int i = 0; i < duration_frame; ++i) { // 점점 알파값 1로
            m_SpriteRenderer.color = new Color(0f, 0f, 0f, 1f/duration_frame*(i+1));
            yield return new WaitForMillisecondFrames(0);
        }

        //m_SpriteRenderer.DOFade(1f, duration);
        //yield return new WaitForSeconds(duration);
        //DOTween.Kill(m_SpriteRenderer);
        m_SpriteRenderer.color = Color.black;
        yield break;
    }

    private IEnumerator FadeOut() {
        int duration = 1500;
        int duration_frame = duration * Application.targetFrameRate / 1000;
        m_SpriteRenderer.color = Color.black;

        for (int i = 0; i < duration_frame; ++i) { // 점점 알파값 0으로
            m_SpriteRenderer.color = new Color(0f, 0f, 0f, 1f - 1f/duration_frame*(i+1));
            yield return new WaitForMillisecondFrames(0);
        }

        //m_SpriteRenderer.DOFade(0f, duration); // 점점 알파값 0으로
        //yield return new WaitForSeconds(duration);
        //DOTween.Kill(m_SpriteRenderer);
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
        yield break;
    }
}

using UnityEngine;
using System.Collections;

public class ScreenEffectAnimation : MonoBehaviour { // 화면 전환 효과

    public SpriteRenderer m_SpriteRenderer;
    private IEnumerator m_TransitionAnimation;

    void OnDisable() {
        StopAllCoroutines();
        m_SpriteRenderer.color = Color.black;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    
    public void PlayTransition() {
        transform.localScale = new Vector3(1f, 1f, 1f);
        m_SpriteRenderer.color = Color.black;
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
        int delay = 1000 - (int) (transform.position.y*1000f/12f) + Random.Range(1000, 300);
        yield return new WaitForMillisecondFrames(delay);

        float init_scale_x = transform.localScale.x;
        int frame = duration * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_scale = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            float localScale_x = Mathf.Lerp(init_scale_x, 0f, t_scale);
            transform.localScale = new Vector3(localScale_x, transform.localScale.y, transform.localScale.z);
            yield return new WaitForMillisecondFrames(0);
        }
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
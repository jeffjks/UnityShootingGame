
using UnityEngine;
using System.Collections;
using DG.Tweening;
 
public class ScreenEffectAnimation : MonoBehaviour {
    
    public SpriteRenderer m_SpriteRenderer;
    
    public void PlayTransition() {
        StartCoroutine(Transition());
    }
    
    public void PlayFadeIn() {
        StartCoroutine(FadeIn());
    }
    

    private IEnumerator Transition() {
        float duration = 0.66f;
        float delay = 1f - transform.position.y/12 + Random.Range(0f, 0.3f);

        yield return new WaitForSeconds(delay);
        transform.DOScaleX(0f, duration);
        yield return new WaitForSeconds(duration);
        DOTween.Kill(transform);
        transform.localScale = new Vector3(1f, 1f, 1f);
        m_SpriteRenderer.color = new Color(0f, 0f, 0f, 0f);
        gameObject.SetActive(false);
        yield break;
    }

    private IEnumerator FadeIn() {
        float duration = 1f;
        m_SpriteRenderer.DOFade(1f, duration); // 점점 알파값 1로
        yield return new WaitForSeconds(duration);
        DOTween.Kill(m_SpriteRenderer);
        m_SpriteRenderer.color = Color.black;
        yield break;
    }
}
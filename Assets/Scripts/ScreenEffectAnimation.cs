
using UnityEngine;
using System.Collections;
 
public class ScreenEffectAnimation : MonoBehaviour {

    public Animator m_Animator;
    public SpriteRenderer m_SpriteRenderer;

    void Start() {
        m_Animator.speed = 0f;
    }

    void OnEnable() {
        m_Animator.speed = 0f;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    
    public void PlayTransition() {
        Invoke("Transition", 1f - transform.position[1]/12 + Random.Range(0f, 0.3f));
    }
    
    public void PlayFadeIn() {
        StartCoroutine(FadeIn());
    }
    

    private void Transition() {
        transform.localScale = new Vector3(1f, 1f, 1f);
        m_Animator.speed = 1.5f;
        m_Animator.Play("Transition", -1, 0f);
        Invoke("Deactivate", 1.5f);
    }

    private IEnumerator FadeIn() {
        for (float i = 0f; i < 1f; i += 0.02f) {
            m_SpriteRenderer.color = new Color(0f, 0f, 0f, i);
            yield return null;
        }
        yield break;
    }

    private void Deactivate() {
        m_Animator.speed = 0f;
        gameObject.SetActive(false);
    }
}
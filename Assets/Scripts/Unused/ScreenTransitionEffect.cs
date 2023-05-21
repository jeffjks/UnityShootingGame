
using UnityEngine;
using System.Collections;

public class ScreenTransitionEffect : MonoBehaviour { // 화면 전환 효과

    public SpriteRenderer m_SpriteRenderer;
    private IEnumerator m_TransitionAnimation;
    
    public void PlayTransition() {
        InitWidth();
        m_SpriteRenderer.color = Color.black;
        StartCoroutine(Transition());
    }

    public void InitWidth()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
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
    }
}
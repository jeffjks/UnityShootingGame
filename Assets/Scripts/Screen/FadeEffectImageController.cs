using UnityEngine;
using UnityEngine.UI;

public class FadeEffectImageController : MonoBehaviour
{
    private Image _fadeImage;

    private void Awake()
    {
        _fadeImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        FadeScreenService.Action_OnChangeScreenAlpha += SetImageAlpha;
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = _fadeImage.color;
        color.a = alpha;
        _fadeImage.color = color;
    }
}

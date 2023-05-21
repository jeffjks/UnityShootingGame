using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWhiteEffecter : MonoBehaviour // 흰색 점멸 이펙트
{
    public SpriteRenderer m_SpriteRenderer;
    private Vector2 m_MaxSize;
    private const float WIDTH_SCALER = 1.5625f; // WIDTH
    private const float WIDTH_DURATION = 0.125f;
    private const float FADE_DURATION = 0.55f;

    private float _width;
    public float Width
    {
        get
        {
            return _width;
        }
        set
        {
            _width = value;
            SetWidth(_width);
        }
    }

    private float _alpha;
    public float Alpha
    {
        get
        {
            return _alpha;
        }
        set
        {
            _alpha = value;
            SetAlpha(_alpha);
        }
    }

    void Awake()
    {
        Alpha = 0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, Depth.WHITE_EFFECT);
        m_MaxSize = new Vector2(Size.CAMERA_WIDTH, Size.CAMERA_HEIGHT) * WIDTH_SCALER;
    }
    
    private void SetAlpha(float alpha) {
        m_SpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
    }
    
    private void SetWidth(float width) {
        transform.localScale = new Vector3(width, m_MaxSize.y, 1f);
    }
    
    public IEnumerator WhiteEffect(bool isLarge)
    {
        Alpha = 1f;

        if (isLarge)
        {
            Width = 0f;
		
            for (float i = Width; (i + Time.deltaTime / WIDTH_DURATION) < 1; i += Time.deltaTime / WIDTH_DURATION)
            {
                Width = i;
                yield return null;
            }
        }
        else
        {
            Width = 1f;
        }
		
        for (float i = Alpha; (i - Time.deltaTime / FADE_DURATION) > 0; i -= Time.deltaTime / FADE_DURATION)
        {
            Alpha = i;
            yield return null;
        }
    }
}
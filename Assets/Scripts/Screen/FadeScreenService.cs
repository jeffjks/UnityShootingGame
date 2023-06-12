using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FadeScreenService : MonoBehaviour
{
	public enum FadeState
	{
		FadeIn,
		FadeOut,
		FadingIn,
		FadingOut,
	}
	
	private FadeState _currentState = FadeState.FadeIn;
    private static FadeScreenService Instance { get; set; }

    private float _alpha;
    private float Alpha
    {
	    get => _alpha;
	    set
	    {
		    _alpha = value;
		    SetTransitionsAlpha(_alpha);
	    }
    }
    
    void Awake()
    {
        if (Instance != null) {
	        Destroy(this.gameObject);
	        return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

	public static void ScreenFadeOut(float duration = 1f, Action callback = null)
	{
		Instance._currentState = FadeState.FadingOut;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.FadeOut(duration, callback));
	}

	public static void ScreenFadeIn(float duration = 1f)
	{
		Instance._currentState = FadeState.FadingIn;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.FadeIn(duration));
	}

	private IEnumerator FadeOut(float duration, Action callback = null)
	{
		if (Mathf.Approximately(duration, 0f))
		{
			Alpha = 0f;
		}

		float timer = 0f;
		while (timer < duration)
		{
			Alpha = 1f - timer / duration;
			timer += Time.deltaTime;
			yield return null;
		}

		Alpha = 0f;
		yield return null;
		_currentState = FadeState.FadeIn;

		callback?.Invoke();
	}

	private IEnumerator FadeIn(float duration)
	{
		if (Mathf.Approximately(duration, 0))
		{
			Alpha = 1f;
		}

		float timer = 0f;
		while (timer < duration)
		{
			Alpha = timer / duration;
			timer += Time.deltaTime;
			yield return null;
		}

		Alpha = 1f;
		yield return null;
		_currentState = FadeState.FadeOut;
	}

	private void SetTransitionsAlpha(float alpha)
	{
		DrawQuad(alpha);
	}

	private void DrawQuad(float alpha)
	{
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.Begin(GL.QUADS);
		GL.Color(new Color(0f, 0f, 0f, alpha));   // moved here, needs to be inside begin/end
		GL.Vertex3(0, 0, -1);
		GL.Vertex3(0, 1, -1);
		GL.Vertex3(1, 1, -1);
		GL.Vertex3(1, 0, -1);
		GL.End();
		GL.PopMatrix();
	}
}

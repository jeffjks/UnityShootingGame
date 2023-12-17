using System;
using System.Collections;
using UnityEngine;

public class FadeScreenService : MonoBehaviour
{
	private enum FadeState
	{
		FadeIn,
		FadeOut,
		FadingIn,
		FadingOut,
	}
	
	public static event Action<float> Action_OnChangeScreenAlpha;
	private static FadeState _currentState = FadeState.FadeIn;
    private static FadeScreenService Instance { get; set; }
    
    private static float Alpha
    {
	    set => Action_OnChangeScreenAlpha?.Invoke(value);
    }
    
    private void Awake()
    {
        if (Instance != null) {
	        Destroy(gameObject);
	        return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

	public static void ScreenFadeOut(float duration = 1f, Action callback = null)
	{
		_currentState = FadeState.FadingOut;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(FadeOut(duration, callback));
	}

	public static void ScreenFadeIn(float duration = 1f)
	{
		_currentState = FadeState.FadingIn;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(FadeIn(duration));
	}

	private static IEnumerator FadeOut(float duration, Action callback = null)
	{
		if (Mathf.Approximately(duration, 0f))
		{
			Alpha = 0f;
		}

		float timer = 0f;
		while (timer < duration)
		{
			Alpha = timer / duration;
			timer += Time.deltaTime;
			yield return new WaitForFrames(1);
		}

		Alpha = 1f;
		yield return new WaitForFrames(1);
		_currentState = FadeState.FadeIn;

		callback?.Invoke();
	}

	private static IEnumerator FadeIn(float duration)
	{
		if (Mathf.Approximately(duration, 0))
		{
			Alpha = 1f;
		}

		float timer = 0f;
		while (timer < duration)
		{
			Alpha = 1f - timer / duration;
			timer += Time.deltaTime;
			yield return new WaitForFrames(1);
		}

		Alpha = 0f;
		yield return new WaitForFrames(1);
		_currentState = FadeState.FadeOut;
	}
}

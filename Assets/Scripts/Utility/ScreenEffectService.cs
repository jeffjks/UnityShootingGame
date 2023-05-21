using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenEffectService : MonoBehaviour
{
	public enum FadeState
	{
		FadeIn,
		FadeOut,
		FadingIn,
		FadingOut,
	}

	public GameObject m_Transition;
	public GameObject m_FadeEffecter;
	public GameObject m_WhiteEffecter;
	private ScreenWhiteEffecter _screenWhiteEffecter;
	private List<ScreenTransitionEffect> _transitionList = new List<ScreenTransitionEffect>();
	private FadeState _currentState = FadeState.FadeIn;
    private static ScreenEffectService Instance { get; set; }

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
        
        Init();
        
        DontDestroyOnLoad(gameObject);
    }

    private void Init() {
	    int r = 1;
	    for(int i = 0; i < 8; i++) { // 가로 6개, 세로 8개. 12*16
		    for(int j = 0; j < 6; j++) {
			    GameObject ins = Instantiate(m_Transition, new Vector3(j*2f-5f, i*2f-15f, Depth.TRANSITION), Quaternion.Euler(0, 0, 45+45*r)); // depth = -4f
			    ins.transform.parent = m_FadeEffecter.transform;
			    ScreenTransitionEffect ScreenTransitionEffect = ins.GetComponent<ScreenTransitionEffect>();
			    _transitionList.Add(ScreenTransitionEffect);
			    r *= -1;
		    }
		    r *= -1;
	    }

	    Alpha = 0f;
	    m_FadeEffecter.SetActive(true);
	    m_WhiteEffecter.SetActive(true);
    }

	public static void ScreenFadeIn(float duration = 1f)
	{
		Instance._currentState = FadeState.FadingIn;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.FadeIn(duration));
	}

	public static void ScreenFadeOut(float duration = 1f, Action callback = null)
	{
		Instance._currentState = FadeState.FadingOut;
		Instance.StopAllCoroutines();
		Instance.StartCoroutine(Instance.FadeOut(duration, callback));
	}

	public static void ScreenTransitionIn()
	{
		if (Instance._currentState != FadeState.FadeOut)
		{
			Debug.LogError("ScreenTransitionIn must be run in FadeOut state.");
			return;
		}
		foreach (var transition in Instance._transitionList)
		{
			transition.PlayTransition();
		}
	}

	private IEnumerator FadeIn(float duration)
	{
		if (Mathf.Approximately(duration, 0f))
		{
			Alpha = 0f;
		}

		for (float i = Alpha; (i - Time.deltaTime / duration) > 0; i -= Time.deltaTime / duration)
		{
			Alpha = i;
			yield return null;
		}

		Alpha = 0f;
		yield return null;
		_currentState = FadeState.FadeIn;
	}

	private IEnumerator FadeOut(float duration, Action callback = null)
	{
		InitTransitionsWidth();
		
		if (Mathf.Approximately(duration, 0))
		{
			Alpha = 1f;
		}
		
		for (float i = Alpha; (i + Time.deltaTime / duration) < 1; i += Time.deltaTime / duration)
		{
			Alpha = i;
			yield return null;
		}

		Alpha = 1f;
		yield return null;
		_currentState = FadeState.FadeOut;

		callback?.Invoke();
	}

	private void SetTransitionsAlpha(float alpha)
	{
		foreach (var transition in Instance._transitionList)
		{
			transition.m_SpriteRenderer.color = new Color(0f, 0f, 0f, alpha);
		}
	}

	private void InitTransitionsWidth()
	{
		foreach (var transition in Instance._transitionList)
		{
			transition.InitWidth();
		}
	}

	public static void ScreenWhiteEffect(bool isLarge)
	{
		Instance.StartCoroutine(Instance._screenWhiteEffecter.WhiteEffect(isLarge));
	}
}

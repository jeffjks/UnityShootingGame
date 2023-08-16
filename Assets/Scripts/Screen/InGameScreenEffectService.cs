using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InGameScreenEffectService : MonoBehaviour
{
	public enum TransitionState
	{
		TransitionIn,
		TransitionOut,
		TransitioningIn,
		TransitioningOut
	}

	public InGameScreenTransitionEffect m_TransitionBlock;
	public RectTransform m_TransitionContent;
	public WhiteEffectController m_WhiteEffectController;
	
	public TransitionState CurrentState { get; set; }
	
	private const int TRANSITION_NUMBER = 48;
	private const int TRANSITION_BLOCK_ROW = 8;
	private const int DELAY_INTERVAL = 160;
	
	private readonly List<InGameScreenTransitionEffect> _transitionList = new ();
    private static InGameScreenEffectService Instance { get; set; }
    
    void Awake()
    {
        if (Instance != null) {
	        Destroy(gameObject);
	        return;
        }
        Instance = this;
        
        Init();
    }

    private void Init()
    {
	    CurrentState = TransitionState.TransitionIn;
	    const int COLUMN = TRANSITION_NUMBER / TRANSITION_BLOCK_ROW;
	    
	    for (var i = 0; i < TRANSITION_NUMBER; ++i) {
		    var screenTransitionEffect = Instantiate(m_TransitionBlock, m_TransitionContent);
		    screenTransitionEffect.Delay = i / TRANSITION_BLOCK_ROW * DELAY_INTERVAL;
		    if ((i + i / COLUMN) % 2 == 1)
		    {
			    screenTransitionEffect.SetRotationState();
		    }
		    _transitionList.Add(screenTransitionEffect);
	    }
    }

	public static void TransitionOut(float duration = 1f)
	{
		Instance.CurrentState = TransitionState.TransitioningOut;
		
		foreach (var transition in Instance._transitionList)
		{
			//transition.gameObject.SetActive(true);
			transition.PlayFadeOut(duration, Instance.SetTransitionOut);
		}
	}

	public static void TransitionIn()
	{
		Instance.CurrentState = TransitionState.TransitioningIn;
		
		foreach (var transition in Instance._transitionList)
		{
			transition.PlayTransitionIn(Instance.SetTransitionIn);
		}
	}

	private void SetTransitionIn()
	{
		Instance.CurrentState = TransitionState.TransitionIn;
	}

	private void SetTransitionOut()
	{
		Instance.CurrentState = TransitionState.TransitionOut;
	}

	public static void WhiteEffect(bool isLarge)
	{
		Instance.m_WhiteEffectController.PlayWhiteEffect(isLarge);
	}
}

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

	public GameObject m_TransitionBlock;
	public GameObject m_TransitionEffectController;
	public GameObject m_WhiteEffectController;
	
	public TransitionState CurrentState { get; set; }

	public const int TRANSITION_BLOCK_WIDTH = 135;
	public const int TRANSITION_BLOCK_HEIGHT = 135;
	private const int TRANSITION_BLOCK_COLUMN = 6;
	private const int TRANSITION_BLOCK_ROW = 8;
	private const int DELAY_INTERVAL = 100;
	
	private WhiteEffectController _whiteEffectController;
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

        _whiteEffectController = m_WhiteEffectController.GetComponent<WhiteEffectController>();
    }

    private void Init()
    {
	    CurrentState = TransitionState.TransitionIn;
	    
	    for (int i = 0; i < TRANSITION_BLOCK_ROW; i++) {
		    for (int j = 0; j < TRANSITION_BLOCK_COLUMN; j++)
		    {
			    Vector3 pos = new Vector3(j * TRANSITION_BLOCK_WIDTH, i * TRANSITION_BLOCK_HEIGHT, Depth.TRANSITION);
			    GameObject obj = Instantiate(m_TransitionBlock, pos, Quaternion.identity);
			    obj.transform.SetParent(m_TransitionEffectController.transform);
			    InGameScreenTransitionEffect screenTransitionEffect = obj.GetComponent<InGameScreenTransitionEffect>();
			    screenTransitionEffect.Delay = (TRANSITION_BLOCK_ROW - i - 1) * DELAY_INTERVAL;
			    screenTransitionEffect.SetRotationState((i + j) % 2 == 1);
			    _transitionList.Add(screenTransitionEffect);
		    }
	    }
	    
	    //m_TransitionEffectController.SetActive(true);
	    //m_WhiteEffectController.SetActive(true);
    }

	public static void TransitionOut(float duration = 1f)
	{
		Instance.CurrentState = TransitionState.TransitioningOut;
		
		foreach (var transition in Instance._transitionList)
		{
			transition.gameObject.SetActive(true);
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
		Instance._whiteEffectController.PlayWhiteEffect(isLarge);
	}
}

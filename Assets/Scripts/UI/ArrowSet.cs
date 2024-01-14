using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowSet : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;

    public void SetInteractable(bool state)
    {
        m_CanvasGroup.interactable = state;
        m_CanvasGroup.alpha = state ? 1f : 0.5f;
    }
}

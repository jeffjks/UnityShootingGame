using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowHighlighter : MonoBehaviour
{
    [SerializeField] private MoveDirection m_MoveDirection;
    [SerializeField] private Animator m_Animator;

    private readonly int _animationHighlight = Animator.StringToHash("Highlight");

    public void SetHighlightSprite(MoveDirection moveDirection)
    {
        if (moveDirection == m_MoveDirection)
        {
            m_Animator.SetTrigger(_animationHighlight);
        }
    }
}

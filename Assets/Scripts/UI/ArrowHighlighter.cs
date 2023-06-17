using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArrowHighlighter : MonoBehaviour
{
    public enum ArrowDirection
    {
        Left,
        Right
    }

    public ArrowDirection m_ArrowDirection;
    public Sprite m_HighlightedSprite;
    public MenuHandler m_TargetMenuHandler;
    
    private Image _imageUI;
    private Sprite _defaultSprite;

    void Awake()
    {
        _imageUI = GetComponent<Image>();
        _defaultSprite = _imageUI.sprite;
    }

    private void OnEnable()
    {
        InputFieldMoveHandler.Action_OnMove += OnMoveInput;
    }

    private void OnDisable()
    {
        InputFieldMoveHandler.Action_OnMove -= OnMoveInput;
    }

    private void OnMoveInput(InputValue inputValue)
    {
        if (!m_TargetMenuHandler.IsActive)
        {
            return;
        }
        
        var moveInputX = inputValue.Get<Vector2>().x;

        if (m_ArrowDirection == ArrowDirection.Left)
        {
            ChangeSprite(moveInputX < 0);
        }
        else if (m_ArrowDirection == ArrowDirection.Right)
        {
            ChangeSprite(moveInputX > 0);
        }
    }

    private void ChangeSprite(bool state)
    {
        if (state)
            _imageUI.sprite = m_HighlightedSprite;
        else
            _imageUI.sprite = _defaultSprite;
    }
}

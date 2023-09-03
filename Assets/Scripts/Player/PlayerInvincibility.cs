using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    public GameObject m_PlayerShield;
    
    public const int REVIVE_TIME = 3000;
    
    private static bool _isInvincible;

    public static Action<bool> Action_OnInvincibilityChanged;

    public static bool IsInvincible
    {
        get => _isInvincible;
        private set {
            _isInvincible = value;
            Action_OnInvincibilityChanged?.Invoke(_isInvincible);
        }
    }
    
    private static int _remainingFrame;

    private void Start()
    {
        Action_OnInvincibilityChanged += SetPlayerShield;
        
        if (DebugOption.InvincibleMod)
        {
            IsInvincible = true;
            _remainingFrame = -1;
        }
    }

    private void OnDestroy()
    {
        Action_OnInvincibilityChanged -= SetPlayerShield;
    }

    private void Update()
    {
        if (_remainingFrame == -1)
        {
            return;
        }
        if (_remainingFrame > 0)
        {
            _remainingFrame--;
        }
        else if (IsInvincible)
        {
            IsInvincible = false;
        }
    }

    public static void SetInvincibility(int millisecond = -1)
    {
        int frame = (millisecond == -1) ? -1 : millisecond * Application.targetFrameRate / 1000;
        
        if (frame < _remainingFrame && frame != -1)
            return;
        if (frame == 0)
            return;
        if (DebugOption.InvincibleMod)
            return;

        IsInvincible = true;
        _remainingFrame = frame;
    }

    private void SetPlayerShield(bool state)
    {
        m_PlayerShield.SetActive(state);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    public GameObject m_PlayerShield;
    
    public const int REVIVE_TIME = 3000;
    
    private static bool _isInvincible;
    public static PlayerInvincibility Instance { get; private set; }

    public Action<bool> Action_OnInvincibilityChanged;

    public static bool IsInvincible
    {
        get => _isInvincible;
        private set {
            _isInvincible = value;
            Instance.Action_OnInvincibilityChanged?.Invoke(_isInvincible);
        }
    }
    
    private static int _remainingFrame;

    private void Start()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        Instance.Action_OnInvincibilityChanged += Instance.SetPlayerShield;

        if (GameManager.InvincibleMod)
        {
            SetInvincibility();
        }
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
        if (GameManager.InvincibleMod)
            return;

        IsInvincible = true;
        _remainingFrame = frame;
    }

    private void SetPlayerShield(bool state)
    {
        m_PlayerShield.SetActive(state);
    }
}

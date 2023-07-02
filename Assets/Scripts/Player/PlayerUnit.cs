using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerObject
{
    public bool m_IsPreviewObject;
    
    public bool SlowMode { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsShooting { get; set; }

    private int _playerAttackLevel;
    public int PlayerAttackLevel
    {
        get => _playerAttackLevel;
        set {
            _playerAttackLevel = Mathf.Clamp(value, 0, MAX_PLAYER_ATTACK_LEVEL);
            Action_OnUpdatePlayerAttackLevel?.Invoke();
        }
    }

    private static bool _isControllable;

    public static bool IsControllable
    {
        get => _isControllable;
        set
        {
            _isControllable = value;
            Instance.Action_OnControllableChanged?.Invoke();
        }
    }
    
    private static PlayerUnit Instance { get; set; }
    
    private const int MAX_PLAYER_ATTACK_LEVEL = 4;

    public event Action Action_OnUpdatePlayerAttackLevel;
    public event Action Action_OnControllableChanged;

    private void Start()
    {
        if (m_IsPreviewObject)
        {
            PlayerAttackLevel = MAX_PLAYER_ATTACK_LEVEL;
        }
        else
        {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            DontDestroyOnLoad(transform.parent);

            Action_OnControllableChanged += () =>
            {
                SlowMode = false;
                IsAttacking = false;
            };
        }
    }
    
    public void DealCollisionDamage(EnemyUnit enemyUnit)
    {
        DealDamage(enemyUnit);
    }

    public bool PowerUp()
    {
        if (PlayerAttackLevel < MAX_PLAYER_ATTACK_LEVEL)
        {
            PlayerAttackLevel++;
            return true;
        }
        return false;
    }
}
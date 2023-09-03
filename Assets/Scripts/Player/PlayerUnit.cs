using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerObject
{
    public bool m_IsPreviewObject;
    [DrawIf("m_IsPreviewObject", true, ComparisonType.Equals)]
    public float m_MaxLaserLength;
    
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
            Instance.Action_OnControllableChanged?.Invoke(_isControllable);
        }
    }
    
    private static PlayerUnit Instance { get; set; }
    
    private const int MAX_PLAYER_ATTACK_LEVEL = 4;

    public event Action Action_OnUpdatePlayerAttackLevel;
    public event Action<bool> Action_OnControllableChanged;

    private void Awake()
    {
        _maxDamageLevel = _playerDamageData.damageByLevel.Count - 1;
        
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

            Action_OnControllableChanged += (controllable) =>
            {
                if (controllable)
                    return;
                SlowMode = false;
                IsAttacking = false;
            };
            SystemManager.Action_OnStageClear += () => IsControllable = false;
            SystemManager.Action_OnNextStage += (hasNextStage) =>
            {
                if (hasNextStage)
                    IsControllable = true;
            };
        }
        
        //CurrentAngle = 180f;
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
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IHasGroundCollider
{
    public void SetColliderPositionOnScreen(Vector2 screenPosition, Quaternion screenRotation);
}

[RequireComponent(typeof(EnemyColorBlender))]
public class EnemyHealth : MonoBehaviour, IHasGroundCollider
{
    private EnemyDeath _enemyDeath;
    [Tooltip("None: 독립적인 Blend, 자체 체력이 있지만 본체에도 데미지 전달\nIndependent: 독립적인 Blend, 독립적인 자체 체력\nShare: 본체와 Blend와 체력 모두 공유")]
    public HealthType m_HealthType;
    [DrawIf("m_HealthType", HealthType.Share, ComparisonType.NotEqual)]
    [SerializeField] private int m_DefaultHealth = -1;
    [SerializeField] private Collider2D[] m_Collider2D; // 지상 적 콜라이더 보정 및 충돌 체크

    public event Action Action_LowHealthState;
    public event Action Action_DamagingBlend;
    public event Action Action_OnHealthChanged; // 여기 등록된 메소드는 Invoke 실행 시 인자로 체력 백분율, 체력 고정값을 받아서 실행됨

    private EnemyHealth _parentEnemyHealth;
    private int _currentHealth;
    private readonly Dictionary<PlayerDamageType, bool> _isTakingDamage = new (); // 중복 데미지 방지
    private bool _isLowHealthState;
    private bool _isInvincible;
    private int _remainingFrame;

    public int CurrentHealth
    {
        get => _currentHealth;
        set {
            _currentHealth = value;
            Action_OnHealthChanged?.Invoke();
            
            if (value == 0)
                OnHpZero();
        }
    }

    private bool IsLowHealth {
        get
        {
            if (m_DefaultHealth < 10000)
            {
                if (HealthRatioScaled < 300) // 30% 미만
                    return true;
            }
            else
            {
                // 최대 체력이 10000 이상이면 체력 3000 미만시 붉은색 점멸
                if (CurrentHealth < 3000)
                    return true;
            }

            return false;
        }
    }

    public int HealthRatioScaled => CurrentHealth * 1000 / m_DefaultHealth;
    
    public static readonly LinkedList<EnemyHealth> EnemyList = new();

    public static void InitEnemyId()
    {
        EnemyList.Clear();
    }

    private void Awake()
    {
        if (transform != transform.root) {
            _parentEnemyHealth = transform.parent.GetComponentInParent<EnemyHealth>();
        }
        _enemyDeath = GetComponent<EnemyDeath>();
        
        CurrentHealth = m_DefaultHealth;
        
        ResetIsTakingDamage();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (_remainingFrame == -1)
            return;
        
        if (_remainingFrame > 0)
        {
            _remainingFrame--;
        }
        else if (_isInvincible)
        {
            _isInvincible = false;
        }
    }

    private void LateUpdate()
    {
        ResetIsTakingDamage();
    }

    private void ResetIsTakingDamage() {
        _isTakingDamage[PlayerDamageType.Normal] = false;
        _isTakingDamage[PlayerDamageType.Laser] = false;
        _isTakingDamage[PlayerDamageType.LaserAura] = false;
        _isTakingDamage[PlayerDamageType.Bomb] = false;
    }
    

    public void TakeDamage(int amount, PlayerDamageType damageType = PlayerDamageType.Normal, bool blend = true)
    {
        // blend - ImageBlend 실행 여부
        if (PauseManager.IsGamePaused)
            return;

        if (m_HealthType == HealthType.None && _parentEnemyHealth != null) { // 본체와 자신에게 데미지 및 자신 색 blend
            _parentEnemyHealth.TakeDamage(amount, damageType, false);
        }
        else if (m_HealthType == HealthType.Share && _parentEnemyHealth != null) { // 본체에게 데미지 및 본체 색 blend
            _parentEnemyHealth.TakeDamage(amount, damageType, true);
            return;
        }

        if (blend) {
            UpdateColorBlend();
        }

        if (_isInvincible) {
            return;
        }
        if (IsDuplicatedDamage(damageType)) {
            return;
        }

        if (m_DefaultHealth >= 0f) {
            CurrentHealth -= amount;

            //if (SystemManager.GameMode == GameMode.Replay) // Only for game play
                //return;

            if (CurrentHealth <= 0)
                OnHpZero();
        }
    }

    private bool IsDuplicatedDamage(PlayerDamageType damageType) {
        if (damageType == PlayerDamageType.Normal) {
            return false;
        }
        
        if (_isTakingDamage.TryGetValue(damageType, out bool result))
        {
            _isTakingDamage[damageType] = true;
            return result;
        }
        _isTakingDamage.Add(damageType, true);
        return false;
    }

    public void SetActiveColliders(bool state) {
        if (m_Collider2D.Length == 0)
            return;
        foreach (var colliderItem in m_Collider2D)
        {
            colliderItem.gameObject.SetActive(state);
            //colliderItem.enabled = state;
        }
    }

    public void SetColliderPositionOnScreen(Vector2 screenPosition, Quaternion screenRotation)
    {
        foreach (var colliderItem in m_Collider2D)
        {
            var colliderTransform = colliderItem.transform;
            colliderTransform.position = new Vector3(screenPosition.x, screenPosition.y, Depth.ENEMY);
            colliderTransform.rotation = screenRotation;
        }
    }
    
    public void SetInvincibility(int millisecond = -1)
    {
        int frame = (millisecond == -1) ? -1 : millisecond * Application.targetFrameRate / 1000;
        
        if (m_Collider2D.Length == 0)
            return;
        if (frame < _remainingFrame && frame != -1)
            return;
        if (frame == 0)
            return;

        _isInvincible = true;
        _remainingFrame = frame;
    }

    public void DisableInvincibility()
    {
        _isInvincible = false;
        _remainingFrame = 0;
    }

    private void UpdateColorBlend()
    {
        if (_enemyDeath.IsDead)
            return;

        Action_DamagingBlend?.Invoke();
        
        if (!_isLowHealthState) {
            if (IsLowHealth) {
                _isLowHealthState = true;
                Action_LowHealthState?.Invoke();
            }
        }
    }

    public void OnHpZero()
    {
        if (_enemyDeath.IsDead)
            return;

        WriteReplayHealthData();
        _enemyDeath.KillEnemy();
        
        HitCountController.Instance.AddHitCount();
    }

    public void WriteReplayHealthData()
    {
        //ReplayManager.WriteReplayEnemyHealth(EnemyId, CurrentHealth);
    }
}

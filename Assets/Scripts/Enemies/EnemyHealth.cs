using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(EnemyColorBlender))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyDeath m_EnemyDeath;
    [Tooltip("None: 독립적인 Blend, 자체 체력이 있지만 본체에도 데미지 전달\nIndependent: 독립적인 Blend, 독립적인 자체 체력\nShare: 본체와 Blend와 체력 모두 공유")]
    public HealthType m_HealthType;
    [DrawIf("m_HealthType", HealthType.Share, ComparisonType.NotEqual)]
    [SerializeField] private int m_DefaultHealth = -1;
    [SerializeField] private Collider2D[] m_Collider2D; // 지상 적 콜라이더 보정 및 충돌 체크
    [HideInInspector] public float m_HealthPercent;

    public event Action Action_LowHealthState;
    public event Action Action_DamagingBlend;
    public event Action Action_OnHealthChanged; // 여기 등록된 메소드는 Invoke 실행 시 인자로 체력 백분율, 체력 고정값을 받아서 실행됨

    private EnemyHealth m_ParentEnemyHealth = null;
    private int m_CurrentHealth;
    private Dictionary<PlayerDamageType, bool> m_IsTakingDamage = new Dictionary<PlayerDamageType, bool>(); // 중복 데미지 방지
    private bool m_IsLowHealthState = false;
    private bool m_Invincibility = false;

    public int CurrentHealth {
        get { return m_CurrentHealth; }
        set {
            m_CurrentHealth = value;
            m_HealthPercent = (float) m_CurrentHealth / (float) m_DefaultHealth;
            Action_OnHealthChanged?.Invoke();
        }
    }

    void Start()
    {
        if (transform != transform.root) {
            m_ParentEnemyHealth = transform.parent.GetComponentInParent<EnemyHealth>();
        }
        m_EnemyDeath = GetComponent<EnemyDeath>();
        
        CurrentHealth = m_DefaultHealth;

        ResetIsTakingDamage();
    }
    
    void LateUpdate()
    {
        ResetIsTakingDamage();
    }

    private void ResetIsTakingDamage() {
        m_IsTakingDamage[PlayerDamageType.Normal] = false;
        m_IsTakingDamage[PlayerDamageType.Laser] = false;
        m_IsTakingDamage[PlayerDamageType.LaserAura] = false;
        m_IsTakingDamage[PlayerDamageType.Bomb] = false;
    }
    

    public void TakeDamage(int amount, PlayerDamageType damage_type = PlayerDamageType.Normal, bool blend = true)
    {
        // blend - ImageBlend 실행 여부

        if (m_HealthType == HealthType.None) { // 본체와 자신에게 데미지 및 자신 색 blend
            m_ParentEnemyHealth?.TakeDamage(amount, damage_type, false);
        }
        else if (m_HealthType == HealthType.Share) { // 본체에게 데미지 및 본체 색 blend
            m_ParentEnemyHealth?.TakeDamage(amount, damage_type, true);
            return;
        }

        if (blend) {
            UpdateColorBlend();
        }

        if (m_Invincibility) {
            return;
        }
        if (IsDuplicatedDamage(damage_type)) {
            return;
        }

        if (m_DefaultHealth >= 0f) {
            CurrentHealth -= amount;

            if (CurrentHealth <= 0) {
                OnHpZero();
            }
        }
    }

    private bool IsDuplicatedDamage(PlayerDamageType damage_type) {
        if (damage_type == PlayerDamageType.Normal) {
            return false;
        }
        
        if (m_IsTakingDamage.TryGetValue(damage_type, out bool result)) {
            return result;
        }
        m_IsTakingDamage.Add(damage_type, true);
        return false;
    }

    public void SetActiveColliders(bool state) {
        if (m_Collider2D.Length == 0)
            return;
        for (int i = 0; i < m_Collider2D.Length; i++)
            m_Collider2D[i].enabled = state;
    }

    public void SetColliderPositionOnScreen(Vector3 screenPosition, Quaternion screenRotation) {
        for (int i = 0; i < m_Collider2D.Length; i++) {
            m_Collider2D[i].transform.position = screenPosition;
            m_Collider2D[i].transform.rotation = screenRotation;
        }
    }


    public void DisableInvincibility(int millisecond = -1) { // millisecond간 무적. 0이면 미적용. -1이면 무기한 무적
        if (millisecond == 0)
            return;
        if (m_Collider2D.Length == 0)
            return;
        m_Invincibility = true;

        if (millisecond != -1)
            StartCoroutine(DisableInvincibilityTimer(millisecond));
    }
    
    private IEnumerator DisableInvincibilityTimer(int millisecond) {
        yield return new WaitForMillisecondFrames(millisecond);
        m_Invincibility = false;
        yield break;
    }

    private void UpdateColorBlend() {
        if (m_EnemyDeath.m_IsDead)
            return;

        Action_DamagingBlend?.Invoke();
        
        if (!m_IsLowHealthState) {
            if (IsLowHealth()) {
                m_IsLowHealthState = true;
                Action_LowHealthState?.Invoke();
            }
        }
    }

    private bool IsLowHealth() {
        if (m_DefaultHealth < 10000) {
            if (m_HealthPercent < 0.30f) { // 30% 미만
                return true;
            }
        }
        else { // 최대 체력이 10000 이상이면 체력 3000 미만시 붉은색 점멸
            if (CurrentHealth < 3000) {
                return true;
            }
        }
        return false;
    }

    private void OnHpZero() {
        m_EnemyDeath.OnDying();
    }
}

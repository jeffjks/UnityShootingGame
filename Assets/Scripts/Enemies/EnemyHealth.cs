using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(EnemyColorBlender))]
public class EnemyHealth : MonoBehaviour
{
    [Tooltip("None: 독립적인 Blend, 자체 체력이 있지만 본체에도 데미지 전달\nIndependent: 독립적인 Blend, 독립적인 자체 체력\nShare: 본체와 Blend와 체력 모두 공유")]
    public HealthType m_HealthType;
    [DrawIf("m_HealthType", HealthType.Share, false)]
    [SerializeField] private int m_DefaultHealth;
    [SerializeField] private Collider2D[] m_Collider2D; // 지상 적 콜라이더 보정 및 충돌 체크

    public event Action Action_StartInteractable;
    public event Action Action_LowHealthState;
    public event Action Action_DamagingBlend;
    public event Action Action_OnDying;
    public event Action Action_OnDeath;
    public event Action Action_OnRemoved;

    private int m_CurrentHealth;
    private bool m_Interactable = true;
    private bool m_Invincibility;
    private bool[] m_IsTakingDamage = { false, false, false }; // 중복 데미지 방지
    private bool m_IsLowHealthState = false;
    private bool m_IsDead;
    
    // [Tooltip("-1일 경우 무적이며 데미지를 parent 오브젝트에게 전달")]

    void Start()
    {
        if (m_HealthType == HealthType.Share) { // TODO: 지우기?
            m_DefaultHealth = -1;
        }
        m_CurrentHealth = m_DefaultHealth;

        Action_LowHealthState += SetLowHealthState;
        Action_OnDying += DisableInteractable;
    }
    
    void Update()
    {
        for (int i = 0; i < m_IsTakingDamage.Length; i++)
            m_IsTakingDamage[i] = false;
    }

    public void DisableInteractable() {
        DisableInteractable(-1);
    }

    public void DisableInteractable(int millisecond) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (m_Collider2D.Length == 0)
            return;
        if (millisecond == 0)
            return;
        m_Interactable = false;
        for (int i = 0; i < m_Collider2D.Length; i++)
            m_Collider2D[i].enabled = false;
        
        if (millisecond != -1)
            StartCoroutine(InteractableTimer(millisecond));
    }

    public void EnableInteractable() {
        if (m_Interactable)
            return;
        if (m_Collider2D.Length == 0)
            return;
        m_Interactable = true;
        for (int i = 0; i < m_Collider2D.Length; i++)
            m_Collider2D[i].enabled = true;
        StartCoroutine(InteractableTimer());
    }

    public bool IsInteractable() {
        return m_Interactable;
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

    private IEnumerator InteractableTimer(int millisecond = -1) {
        if (millisecond != -1) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        EnableInteractable();
        Action_StartInteractable?.Invoke();
        yield break;
    }
    

    public void TakeDamage(int amount, sbyte damage_type = -1, bool blend = true)
    {
        // damage_type - -1:일반공격, 0:레이저, 1:레이저(Aura), 2:폭탄
        // blend - ImageBlend 실행 여부
        
        if (damage_type >= 0) {
            if (m_IsTakingDamage[damage_type])
                return;
            else
                m_IsTakingDamage[damage_type] = true;
        }
        
        UpdateColorBlend();

        if (m_DefaultHealth >= 0f) {
            m_CurrentHealth -= amount;

            if (m_CurrentHealth <= 0) {
                //m_EnemyUnit.m_IsDead = true;
                //KilledByPlayer();
                OnDying();
            }
        }
    }


    private void UpdateColorBlend() {
        if (m_IsDead)
            return;
        if (m_HealthType == HealthType.Share)
            return;

        Action_DamagingBlend?.Invoke();
        
        if (!m_IsLowHealthState) {
            if (m_DefaultHealth < 10000) {
                if (m_CurrentHealth * 10 < m_DefaultHealth * 3) { // 30% 미만
                    Action_LowHealthState?.Invoke();
                }
            }
            else { // 최대 체력이 10000 이상이면 체력 3000 미만시 붉은색 점멸
                if (m_CurrentHealth < 3000) {
                    Action_LowHealthState?.Invoke();
                }
            }
        }
    }

    private void SetLowHealthState() {
        m_IsLowHealthState = true;
    }

    public void OnDying() {
        m_IsDead = true;

        Action_OnDying?.Invoke();
    }

    public void OnDeath() {
        Action_OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnRemoved() {
        Action_OnRemoved?.Invoke();
        Destroy(gameObject);
    }
}

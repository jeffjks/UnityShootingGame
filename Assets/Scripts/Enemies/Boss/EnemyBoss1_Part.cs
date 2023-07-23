using System.Collections;
using System;
using UnityEngine;


public class EnemyBoss1_Part : EnemyUnit
{
    [SerializeField] private Animator _partAnimation;

    private readonly int _openedBoolAnimation = Animator.StringToHash("Opened");

    private void Start()
    {
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    public void SetOpenState(bool state) {
        _partAnimation.SetBool(_openedBoolAnimation, state);
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            BulletManager.BulletsToGems(0);
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.SetBulletFreeState(500);
        yield break;
    }
}

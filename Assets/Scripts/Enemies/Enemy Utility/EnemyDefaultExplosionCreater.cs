using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefaultExplosionCreater : MonoBehaviour, IExplosionCreater
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private ExplType m_ExplType = ExplType.None;
    [SerializeField] private ExplAudioType m_ExplAudioType = ExplAudioType.None;

    private ExplosionJsonManager _explosionJsonManager;

    private void Awake()
    {
        _explosionJsonManager = ExplosionJsonManager.Instance;

        m_EnemyDeath.Action_OnKilled += StartExplosion;
    }

    public void StartExplosion() {
        Effect effect = new Effect(m_ExplType, m_ExplAudioType);
        _explosionJsonManager.CreateExplosionEffect(m_EnemyDeath, effect);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefaultExplosionCreater : MonoBehaviour, IExplosionCreater
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private ExplType m_ExplType = ExplType.None;
    [SerializeField] private ExplAudioType m_ExplAudioType = ExplAudioType.None;

    private ExplosionJsonManager m_ExplosionJsonManager = null;

    private void Awake()
    {
        m_ExplosionJsonManager = ExplosionJsonManager.instance_jm;

        m_EnemyDeath.Action_OnDying += StartExplosion;
    }

    public void StartExplosion() {
        Effect effect = new Effect(m_ExplType, m_ExplAudioType);
        m_ExplosionJsonManager.CreateExplosionEffect(m_EnemyDeath, effect);
    }
}
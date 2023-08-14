using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosionCreater : MonoBehaviour, IExplosionCreater
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private string m_JsonKey;
    [SerializeField] private bool m_RunOnDying = true;

    private ExplosionJsonManager m_ExplosionJsonManager = null;

    private void Awake()
    {
        m_ExplosionJsonManager = ExplosionJsonManager.Instance;

        if (m_RunOnDying) {
            m_EnemyDeath.Action_OnKilled += StartExplosion;
        }
    }

    public void StartExplosion() {
        m_ExplosionJsonManager.StartExplosionEffect(m_JsonKey, m_EnemyDeath);
    }
}

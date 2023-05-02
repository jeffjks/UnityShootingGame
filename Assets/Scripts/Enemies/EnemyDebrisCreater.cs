using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDebrisCreater : MonoBehaviour
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private Debris m_Debris;
    private PoolingManager m_PoolingManager = null;

    void Start()
    {
        m_PoolingManager = PoolingManager.instance_op;
        m_EnemyDeath.Action_OnDeath += CreateDebris;
    }

    private void CreateDebris() {
        GameObject obj = m_PoolingManager.PopFromPool("Debris", PoolingParent.DEBRIS);
        DebrisEffect debris = obj.GetComponent<DebrisEffect>();
        obj.transform.position = transform.position;
        obj.SetActive(true);
        debris.OnStart(m_Debris);
    }
}

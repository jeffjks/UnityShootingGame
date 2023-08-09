using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDebrisCreater : MonoBehaviour
{
    [SerializeField] private EnemyDeath m_EnemyDeath;
    [SerializeField] private DebrisType m_Debris;

    void Start()
    {
        m_EnemyDeath.Action_OnDeath += CreateDebris;
    }

    private void CreateDebris()
    {
        GameObject obj = PoolingManager.PopFromPool("Debris", PoolingParent.Debris);
        DebrisEffect debris = obj.GetComponent<DebrisEffect>();
        obj.transform.position = transform.position;
        obj.SetActive(true);
        debris.OnStart(m_Debris);
    }
}

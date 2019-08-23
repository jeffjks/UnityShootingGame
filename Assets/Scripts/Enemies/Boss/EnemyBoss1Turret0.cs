using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret0 : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    
    private bool m_Shooting = false;
    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
        GetCoordinates();
        m_Pattern1 = Pattern1();
        StartCoroutine(m_Pattern1);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        
        base.Update();
    }

    private IEnumerator Pattern1()
    {
        yield return null;   
    }
}
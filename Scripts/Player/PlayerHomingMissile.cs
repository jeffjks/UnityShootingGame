using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingMissile : PlayerMissile {
    
    [Space(10)]
    [SerializeField] private float m_RotationSpeed = 0.09f;
    private GameObject m_Target;
    
    protected override void OnStart()
    {
        m_Vector2 = transform.up * m_Speed;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_Target == null)
            m_Target = FindClosestEnemy();
        
        if (m_Target != null) {
            Vector2 vec = (m_Target.transform.position - transform.position).normalized;
            transform.up = Vector3.RotateTowards(transform.up, vec, m_RotationSpeed, 0f);
            m_Vector2 = transform.up * m_Speed;
        }

        MoveVector();
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        float distance = Mathf.Infinity;

        foreach (GameObject target_temp in enemies) {
            EnemyUnit enemy = target_temp.GetComponentInParent<EnemyUnit>();
            if (enemy.m_Collider2D != null) {
                Vector2 diff = target_temp.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance) {
                    target = target_temp;
                    distance = curDistance;
                }
            }
        }
        return target;
    }
}




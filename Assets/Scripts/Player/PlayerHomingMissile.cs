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
        
        else {
            Vector2 vec = (m_Target.transform.position - transform.position).normalized;
            transform.up = Vector3.RotateTowards(transform.up, vec, m_RotationSpeed, 0f);
            m_Vector2 = transform.up * m_Speed;
        }

        MoveVector();
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        GameObject target = null;
        Vector2 camera_pos = m_PlayerManager.m_MainCamera.transform.position;
        float distance = Mathf.Infinity;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject target_temp in enemies) {
            EnemyUnit enemy = target_temp.GetComponentInParent<EnemyUnit>();
            if (enemy.m_Position2D.x < camera_pos.x - Size.CAMERA_WIDTH*0.5f) // -6 (default)
                continue;
            else if (enemy.m_Position2D.x > camera_pos.x + Size.CAMERA_WIDTH*0.5f) // 6 (default)
                continue;
            else if (enemy.m_Position2D.y < camera_pos.y - Size.CAMERA_HEIGHT*0.5f) // -16
                continue;
            else if (enemy.m_Position2D.y > camera_pos.y + Size.CAMERA_HEIGHT*0.5f) // 0
                continue;

            if (enemy.m_IsAttackable) {
                Vector2 diff = enemy.m_Position2D - (Vector2) transform.position;
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




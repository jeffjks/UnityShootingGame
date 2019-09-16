using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingMissile : PlayerMissile {
    
    [Space(10)]
    [SerializeField] private float m_RotationSpeed = 0.09f;
    private GameObject m_Target;
    private Vector2 m_MainCameraPosition;
    
    protected override void OnStart()
    {
        m_Vector2 = transform.up * m_Speed;
        m_Target = null;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (m_PlayerManager != null) {
            m_MainCameraPosition = m_PlayerManager.m_MainCamera.transform.position;
            
            if (m_Target == null) {
                m_Target = FindClosestEnemy();
            }
            else {
                Vector2 vec = (m_Target.transform.position - transform.position).normalized;
                transform.up = Vector3.RotateTowards(transform.up, vec, m_RotationSpeed, 0f);
                m_Vector2 = transform.up * m_Speed;
            }
        }
        /*
        Vector2 left = new Vector2(m_MainCameraPosition.x - Size.CAMERA_WIDTH*0.5f, -8f);
        Vector2 right = new Vector2(m_MainCameraPosition.x + Size.CAMERA_WIDTH*0.5f, -8f);
        Vector2 top = new Vector2(0f, m_MainCameraPosition.y + Size.CAMERA_HEIGHT*0.5f);
        Vector2 bottom = new Vector2(0f, m_MainCameraPosition.y - Size.CAMERA_HEIGHT*0.5f);
        */
        MoveVector();
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        GameObject target = null;
        float distance = Mathf.Infinity;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject target_temp in enemies) {
            EnemyUnit enemy = target_temp.GetComponentInParent<EnemyUnit>();
            if (target_temp.transform.position.x < m_MainCameraPosition.x - Size.CAMERA_WIDTH*0.5f) // -6 (default)
                continue;
            else if (target_temp.transform.position.x > m_MainCameraPosition.x + Size.CAMERA_WIDTH*0.5f) // 6 (default)
                continue;
            else if (target_temp.transform.position.y < m_MainCameraPosition.y - Size.CAMERA_HEIGHT*0.5f) // -16
                continue;
            else if (target_temp.transform.position.y > m_MainCameraPosition.y + Size.CAMERA_HEIGHT*0.5f) // 0
                continue;

            if (enemy.m_IsAttackable) {
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




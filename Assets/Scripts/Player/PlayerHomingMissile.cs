using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingMissile : PlayerWeapon {
    
    [Space(10)]
    private float m_RotationSpeed = 324f;
    private GameObject m_Target;
    private Vector2 m_MainCameraPosition;
    private SystemManager m_SystemManager = null;
    
    public override void OnStart() {
        base.OnStart();
        m_SystemManager = SystemManager.instance_sm;
        m_Target = null;
        
        RotateImmediately(m_MoveVector.direction);
        m_MoveVector.speed = m_Speed;
        
        UpdateTransform();
        SetPosition2D();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (m_SystemManager != null) {
            m_MainCameraPosition = MainCamera.Instance.transform.position;
            
            if (m_Target == null) {
                m_Target = FindClosestEnemy();
            }
            else {
                RotateSlightly(m_Target.transform.position, m_RotationSpeed);
                //Vector2 vec = (m_Target.transform.position - transform.position).normalized;
                //transform.up = Vector3.RotateTowards(transform.up, vec, m_RotationSpeed, 0f);
            }
        }
        //transform.up = new Vector2(transform.up.x, transform.up.y);
        //m_MoveVector = new MoveVector(transform.up);

        MoveDirection(m_Speed, m_CurrentAngle);
        
        //RotateImmediately(m_MoveVector.direction);
        UpdateTransform();
        SetPosition2D();
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies;
        GameObject target = null;
        float distance = Mathf.Infinity;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject target_temp in enemies) {
            EnemyUnit enemy = target_temp.GetComponentInParent<EnemyUnit>();
            if (target_temp.transform.position.x < m_MainCameraPosition.x - Size.MAIN_CAMERA_WIDTH/2) // -6 (default)
                continue;
            else if (target_temp.transform.position.x > m_MainCameraPosition.x + Size.MAIN_CAMERA_WIDTH/2) // 6 (default)
                continue;
            else if (target_temp.transform.position.y < - Size.MAIN_CAMERA_HEIGHT) // -16
                continue;
            else if (target_temp.transform.position.y > 0) // 0
                continue;
            
            if (!enemy.IsInteractable()) {
                continue;
            }
            
            Vector2 diff = enemy.m_Position2D - m_Position2D;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance) {
                target = target_temp;
                distance = curDistance;
            }
        }
        return target;
    }
}




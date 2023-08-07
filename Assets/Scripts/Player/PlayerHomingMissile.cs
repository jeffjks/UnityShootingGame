using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHomingMissile : PlayerWeapon {
    
    [Space(10)]
    private const float ROTATION_SPEED = 324f;
    private GameObject _target;
    private Vector2 m_MainCameraPosition;
    
    public override void OnStart() {
        base.OnStart();
        _target = null;
        
        CurrentAngle = m_MoveVector.direction;
        m_MoveVector.speed = m_Speed;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (SystemManager.PlayState != PlayState.OutGame) {
            m_MainCameraPosition = MainCamera.Instance.transform.position;
            
            if (_target == null) {
                _target = FindClosestEnemy();
            }
            else {
                var targetAngle = GetAngleToTarget(m_Position2D, _target.transform.position);
                CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, targetAngle, ROTATION_SPEED / Application.targetFrameRate * Time.timeScale);
                
                //Vector2 vec = (_target.transform.position - transform.position).normalized;
                //transform.up = Vector3.RotateTowards(transform.up, vec, m_RotationSpeed, 0f);
            }
        }
        //transform.up = new Vector2(transform.up.x, transform.up.y);
        //m_MoveVector = new MoveVector(transform.up);

        MoveDirection(m_Speed, CurrentAngle);
        
        //RotateImmediately(m_MoveVector.direction);
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




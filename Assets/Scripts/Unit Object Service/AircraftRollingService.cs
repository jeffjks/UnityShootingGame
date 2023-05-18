using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftRollingService : MonoBehaviour
{
    public UnitObject m_UnitObject;
    public Transform m_Rotator;
    public float m_MaxRoll;
    public float m_RollSpeed;
    [Tooltip("체크 시 현재 각도와 방향 비교, 해제 시 이전 방향과 현재 방향 비교")]
    public bool m_RelativeToCurrentAngle;

    private float m_PreviousDirection;
    private float m_CurrentRollDegree;
    
    private void Update()
    {
        float current_direction = m_UnitObject.m_MoveVector.direction;
        float target_rollDegree;

        if (m_RelativeToCurrentAngle) {
            target_rollDegree = System.Math.Sign(current_direction % 180) * m_MaxRoll; // Mathf 대신 System.Math 사용
        }
        else {
            target_rollDegree = System.Math.Sign(m_PreviousDirection - current_direction) * m_MaxRoll; // Mathf 대신 System.Math 사용
        }
        
        m_CurrentRollDegree = Mathf.MoveTowards(m_CurrentRollDegree, target_rollDegree, m_RollSpeed / Application.targetFrameRate * Time.timeScale);

        Roll();
        
        m_PreviousDirection = m_UnitObject.m_MoveVector.direction;
    }

    private void Roll() {
        m_Rotator.localRotation = Quaternion.AngleAxis(m_CurrentRollDegree, Vector3.up);
    }
}
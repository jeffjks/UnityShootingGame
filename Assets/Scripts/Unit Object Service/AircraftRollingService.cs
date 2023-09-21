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

    private float _currentRollDegree;
    public float CurrentRollDegree
    {
        get => _currentRollDegree;
        set
        {
            _currentRollDegree = value;
            m_Rotator.localRotation = Quaternion.AngleAxis(_currentRollDegree, Vector3.up);
        }
    }
    
    private void Update()
    {
        var current_direction = m_UnitObject.m_MoveVector.direction;
        var target_rollDegree = m_RelativeToCurrentAngle // Mathf 대신 System.Math 사용
            ? System.Math.Sign((int) current_direction - 180) * m_MaxRoll
            : System.Math.Sign(m_PreviousDirection - current_direction) * m_MaxRoll;
        
        CurrentRollDegree = Mathf.MoveTowards(CurrentRollDegree, target_rollDegree, m_RollSpeed / Application.targetFrameRate * Time.timeScale);
        
        m_PreviousDirection = m_UnitObject.m_MoveVector.direction;
    }
}
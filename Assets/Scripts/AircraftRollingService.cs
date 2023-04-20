using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftRollingService : MonoBehaviour
{
    public Entity m_RollingObject; // 수정 필요
    public Transform m_Rotator;
    public float m_MaxRoll;
    public float m_RollSpeed;

    private Vector2 m_PreviousPosition;
    private Vector2 m_PreviousDirVector;
    private float m_CurrentRollDegree;

    private void Start()
    {
        m_PreviousPosition = m_Rotator.position;
        m_PreviousDirVector = m_Rotator.forward;
    }

    private void Update()
    {
        Vector2 current_dirVector = m_RollingObject.m_MoveVector.GetVector();
        
        float target_rollDegree = System.Math.Sign(Vector2.SignedAngle(current_dirVector, m_PreviousDirVector)) * m_MaxRoll; // Mathf 대신 System.Math 사용
        m_CurrentRollDegree = Mathf.MoveTowards(m_CurrentRollDegree, target_rollDegree, 72f / Application.targetFrameRate * Time.timeScale);

        Roll();

        m_PreviousPosition = m_Rotator.position;
        m_PreviousDirVector = m_RollingObject.m_MoveVector.GetVector();
    }

    private void Roll() {
        m_Rotator.localRotation = Quaternion.AngleAxis(m_CurrentRollDegree, Vector3.up);
    }
}
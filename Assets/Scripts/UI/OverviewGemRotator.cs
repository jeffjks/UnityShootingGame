using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewGemRotator : MonoBehaviour
{
    public Vector3 m_RotationAngle;
    public float m_RotationSpeed;

	void Update () {
        transform.Rotate(m_RotationAngle * m_RotationSpeed * Time.deltaTime);
    }
}

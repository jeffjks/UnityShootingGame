using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDroneRotator : MonoBehaviour
{
    public int m_RotateReverse;
    
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * 360f * m_RotateReverse, Space.Self);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShotModule : MonoBehaviour
{
    public Transform m_Player;

    void Update()
    {
        float rot = m_Player.rotation.eulerAngles[1];
        transform.localEulerAngles = new Vector2(transform.localEulerAngles[0], -rot);
        // Quaternion.Euler(0f, 0f, rot);
    }
}

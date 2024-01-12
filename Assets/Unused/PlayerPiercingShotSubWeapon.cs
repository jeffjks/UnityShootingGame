using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED SCRIPT

public class PlayerPiercingShotSubWeapon : MonoBehaviour
{
    public Transform m_Player;

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        float rot = m_Player.rotation.eulerAngles[1];
        transform.localEulerAngles = new Vector2(transform.localEulerAngles[0], -rot);
        // Quaternion.Euler(0f, 0f, rot);
    }
}

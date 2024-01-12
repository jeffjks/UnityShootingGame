using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotSpawner : MonoBehaviour
{
    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        transform.eulerAngles = new Vector3 (-90f, 0f, 0f);
    }
}

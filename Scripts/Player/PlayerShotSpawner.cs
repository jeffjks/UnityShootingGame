using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotSpawner : MonoBehaviour
{
    void Update()
    {
        transform.eulerAngles = new Vector3 (-90f, 0f, 0f);
    }
}

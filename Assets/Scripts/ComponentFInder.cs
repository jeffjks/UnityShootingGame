using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentFInder : MonoBehaviour
{
    void Start()
    {
        var objs = FindObjectsOfType<Light>();

        foreach (var obj in objs)
        {
            Debug.Log(obj.name);
        }
    }
}

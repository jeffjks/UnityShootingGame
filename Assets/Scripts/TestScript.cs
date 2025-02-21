using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;

// yield return new WaitForMillisecondFrames -> WaitFor

public class Test
{
    int a;
}

public class TestScript : MonoBehaviour
{
    public GameObject m_Test;
    
    private void Start()
    {
        Destroy(m_Test);
        m_Test?.SetActive(true);
        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        yield return null;
        if (m_Test == null)
            Debug.Log("m_Test is Null");
        yield return null;
        if (m_Test is null)
            Debug.Log("m_Test is Null");
        yield return null;
        if (ReferenceEquals(m_Test, null))
            Debug.Log("m_Test is Reference Null");
        yield return null;
    }
}

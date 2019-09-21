using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public bool m_Active1, m_Active2;
    public int m_Frame;
    private float a, b;
    private int n;

    void Start()
    {
        //StartCoroutine(TestCoroutine1());
        //StartCoroutine(TestCoroutine2());
    }

    void Update() {
        if (n > 150)
            return;
        Application.targetFrameRate = m_Frame;
        a += Time.deltaTime;
        Debug.Log(a + ", " + b);
        n++;
    }

    void FixedUpdate() {
        if (n > 150)
            return;
        b += Time.fixedDeltaTime;
    }

    private IEnumerator TestCoroutine1() {
        while(true) {
            Debug.Log("Fixed :"+Time.timeSinceLevelLoad);
            if (m_Active1)
                transform.position = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator TestCoroutine2() {
        int n = 0;
        while(true) {
            Debug.Log(Time.frameCount + ", " + n);
            n++;
            if (m_Active2)
                transform.position = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
            for (int i = 0; i < 2; i++)
                yield return new WaitForFixedUpdate();
        }
    }
}

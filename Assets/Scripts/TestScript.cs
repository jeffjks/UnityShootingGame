using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// yield return new WaitForMillisecondFrames -> WaitFor

public class TestScript : MonoBehaviour
{
    private Quaternion m_DefaultRotation;
    
    private void Awake() {
        //Debug.Log(transform.rotation);
        //transform.root.rotation = Quaternion.Euler(0f, 20f, 0f);
        //Debug.Log(transform.rotation);
        
        var temp = transform.root.rotation;
        transform.root.rotation = Quaternion.identity;
        m_DefaultRotation = transform.rotation * Quaternion.Inverse(transform.root.rotation);
        transform.root.rotation = temp;
        
        Debug.Log(m_DefaultRotation.eulerAngles);
    }
}

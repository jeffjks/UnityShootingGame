using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// yield return new WaitForMillisecondFrames -> WaitFor

public class TestScript : MonoBehaviour
{
    public TriggerBodyType m_TriggerBodyType;
    private TriggerBody _triggerBody;
    private void Start()
    {
        _triggerBody = GetComponent<TriggerBody>();
        SimulationManager.TriggerBodies[m_TriggerBodyType].AddLast(_triggerBody);
    }
}

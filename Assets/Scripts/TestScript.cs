using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public InputField m_InputField;

    void Update()
    {
        Debug.Log(m_InputField.isFocused);
        m_InputField.ActivateInputField();
    }
}

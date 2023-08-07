using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textUI;
    
    void Start()
    {
        _textUI.SetText("ver " + Application.version);
    }
}

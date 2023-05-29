using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginInputFieldController : MonoBehaviour, ISubmitHandler
{
    public LoginMenuHandler LoginMenuHandler;
    
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    public void OnSubmit(BaseEventData baseEventData)
    {
        LoginMenuHandler.Login();
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginInputFieldController : MonoBehaviour, ISubmitHandler
{
    public LoginMenuHandler LoginMenuHandler;

    public void OnSubmit(BaseEventData eventData)
    {
        LoginMenuHandler.Login();
    }
}

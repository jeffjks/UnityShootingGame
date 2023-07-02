using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegisterRecordInputFieldController : MonoBehaviour, ISubmitHandler
{
    public RegisterLocalRankingMenuHandler m_RegisterLocalRankingMenuHandler;

    public void OnSubmit(BaseEventData eventData)
    {
        m_RegisterLocalRankingMenuHandler.SelectConfirm();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PopupMenuHandler : MenuHandler
{
    public UnityAction Action_OnPositive;
    public UnityAction Action_OnNegative;
    [SerializeField] private TextStyling m_TextStyling;
    [SerializeField] private TextMeshProUGUI m_Message;
    
    protected override void Init() { }

    public void OnPositive()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        Action_OnPositive?.Invoke();
    }

    public void OnNegative()
    {
        Action_OnNegative?.Invoke();
        Back(false);
        Destroy(gameObject);
    }

    public void SetMessageText(string nativeMessage, string message)
    {
        m_TextStyling.m_NativeText = nativeMessage;
        m_Message.SetText(message);
    }
}
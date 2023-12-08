using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupMenuHandler : MenuHandler
{
    public UnityAction Action_OnPositive;
    public UnityAction Action_OnNegative;
    [SerializeField] private TextStyling m_TextStyling;
    [SerializeField] private TextMeshProUGUI m_Message;
    public Button m_ButtonPositive;
    public Button m_ButtonNegative;

    protected override void Init()
    {
        _isActive = true;
        m_ButtonPositive.onClick.AddListener(OnPositive);
        m_ButtonNegative.onClick.AddListener(OnNegative);
    }

    private void OnPositive()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        Action_OnPositive?.Invoke();
        Close();
    }

    private void OnNegative()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        Action_OnNegative?.Invoke();
        Close();
    }

    private void Close()
    {
        Back(false);
        Destroy(gameObject);
    }

    public void SetMessageText(string nativeMessage, string message)
    {
        m_TextStyling.m_NativeText = nativeMessage;
        m_Message.SetText(message);
    }
}
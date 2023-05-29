using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputFieldSelecter : MonoBehaviour
{
    public TMP_InputField[] m_InputFields;

    public void OnMove(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();

        foreach (var inputField in m_InputFields)
        {
            if (inputField.isFocused)
            {
                EventSystem.current.currentInputModule.enabled = false;
                
                if (moveInput == Vector2.up)
                {
                    (inputField.navigation.selectOnUp)?.Select();
                    EventSystem.current.currentInputModule.enabled = true;
                }
                else if (moveInput == Vector2.down)
                {
                    (inputField.navigation.selectOnDown)?.Select();
                    EventSystem.current.currentInputModule.enabled = true;
                }
                return;
            }
        }
    }
}

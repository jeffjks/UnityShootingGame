using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuHandler : MonoBehaviour
{
    public int m_InitialSelection;
    public bool m_PreserveLastSelection;
    
    private static readonly Stack<GameObject> _previousMenuStack = new();
    private static readonly Dictionary<GameObject, Selectable> _lastSelected = new();

    protected void GoToTargetMenu(GameObject targetMenu)
    {
        targetMenu.SetActive(true);
        _previousMenuStack.Push(gameObject);

        SaveLastSelection();
        FindNextSelection(targetMenu);
        
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public virtual void Back()
    {
        if (_previousMenuStack.Count == 0)
        {
            return;
        }
        var previousMenu = _previousMenuStack.Peek();
        previousMenu.SetActive(true);
        _previousMenuStack.Pop();

        SaveLastSelection();
        FindNextSelection(previousMenu);
        
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }

    private void SaveLastSelection()
    {
        if (m_PreserveLastSelection)
            _lastSelected[gameObject] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    }

    private void FindNextSelection(GameObject menu)
    {if (_lastSelected.TryGetValue(menu, out Selectable selectable))
        {
            selectable.Select();
            return;
        }
        Selectable[] selectables = menu.GetComponentsInChildren<Selectable>();
        selectables[m_InitialSelection].Select();
    }
}

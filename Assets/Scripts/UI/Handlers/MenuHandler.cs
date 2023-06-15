using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuHandler : MonoBehaviour
{
    public int m_InitialSelection;
    public bool m_PreserveLastSelection;
    
    protected static readonly Stack<GameObject> _previousMenuStack = new();
    protected static readonly Dictionary<GameObject, Selectable> _lastSelected = new();

    protected void GoToTargetMenu(MenuHandler targetMenu)
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        targetMenu.gameObject.SetActive(true);
        _previousMenuStack.Push(gameObject);

        SaveLastSelection();
        FindNextSelection(targetMenu.gameObject);
        
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public virtual void Apply()
    {
        
        if (_previousMenuStack.Count == 0)
            return;
        
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        ReturnToPreviousMenu();
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    public virtual void Back()
    {
        if (_previousMenuStack.Count == 0)
            return;
        
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        ReturnToPreviousMenu();
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }

    protected void BackToMainMenu()
    {
        if (_previousMenuStack.Count == 0)
            return;
        
        while (_previousMenuStack.Count > 1)
            _previousMenuStack.Pop();
        
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        var previousMenu = _previousMenuStack.Peek();
        previousMenu.SetActive(true);
        _previousMenuStack.Pop();

        SaveLastSelection();
        FindNextSelection(previousMenu);
        
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }

    private void ReturnToPreviousMenu()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        var previousMenu = _previousMenuStack.Peek();
        previousMenu.SetActive(true);
        _previousMenuStack.Pop();

        SaveLastSelection();
        FindNextSelection(previousMenu);
    }

    private void SaveLastSelection()
    {
        if (m_PreserveLastSelection)
            _lastSelected[gameObject] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
    }

    private void FindNextSelection(GameObject menu)
    {
        if (_lastSelected.TryGetValue(menu, out Selectable selectable))
        {
            selectable.Select();
            return;
        }
        Selectable[] selectables = menu.GetComponentsInChildren<Selectable>();
        selectables[m_InitialSelection].Select();
    }
}

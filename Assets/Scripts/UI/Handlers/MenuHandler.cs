using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuHandler : MonoBehaviour
{
    public int m_InitialSelection;
    public bool m_PreserveLastSelection;

    protected bool _isActive;
    protected static readonly Stack<MenuHandler> _previousMenuStack = new();

    public bool IsActive => _isActive;
    
    private static readonly Dictionary<MenuHandler, Selectable> _lastSelected = new();

    protected abstract void Init();

    protected void GoToTargetMenu(MenuHandler targetMenu, bool activateNewPanel = true)
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        
        CriticalStateSystem.SetCriticalState(10);
        
        if (activateNewPanel)
        {
            targetMenu.gameObject.SetActive(true);
            targetMenu.Init();
        }
        
        _previousMenuStack.Push(this);

        SaveLastSelection(activateNewPanel);
        FindNextSelection(targetMenu);
        
        AudioService.PlaySound("ConfirmUI");
        if (activateNewPanel)
            gameObject.SetActive(false);
        
        _isActive = false;
        targetMenu._isActive = true;
    }

    public virtual void Apply()
    {
        if (_previousMenuStack.Count == 0)
            return;
        
        if (CriticalStateSystem.InCriticalState)
            return;
        
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
        
        ReturnToPreviousMenu();
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }

    public virtual void Back(bool activateNewPanel)
    {
        if (_previousMenuStack.Count == 0)
            return;
        
        if (CriticalStateSystem.InCriticalState)
            return;
        
        ReturnToPreviousMenu();
        AudioService.PlaySound("CancelUI");
        
        if (activateNewPanel)
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
        
        var previousMenu = _previousMenuStack.Peek();
        previousMenu.gameObject.SetActive(true);
        previousMenu.Init();
        _previousMenuStack.Pop();

        SaveLastSelection();
        FindNextSelection(previousMenu);
        
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
        
        CriticalStateSystem.SetCriticalState(10);
        
        _isActive = false;
        previousMenu._isActive = true;
    }

    private void ReturnToPreviousMenu()
    {
        CriticalStateSystem.SetCriticalState(10);
        
        var previousMenu = _previousMenuStack.Peek();
        previousMenu.gameObject.SetActive(true);
        previousMenu.Init();
        _previousMenuStack.Pop();

        SaveLastSelection();
        FindNextSelection(previousMenu);
        
        _isActive = false;
        previousMenu._isActive = true;
    }

    private void SaveLastSelection(bool activateNewPanel = true)
    {
        if (!activateNewPanel)
        {
            _lastSelected[this] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            return;
        }

        if (m_PreserveLastSelection)
            _lastSelected[this] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        else
            _lastSelected.Remove(this);
    }

    private void FindNextSelection(MenuHandler menu)
    {
        if (_lastSelected.TryGetValue(menu, out Selectable selectable))
        {
            selectable.Select();
            return;
        }
        Selectable[] selectables = menu.GetComponentsInChildren<Selectable>();
        selectables[menu.m_InitialSelection].Select();
    }
}

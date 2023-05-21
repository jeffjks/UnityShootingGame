using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuHandler : MonoBehaviour
{
    private static readonly Stack<GameObject> _previousMenuStack = new();
    private static readonly Dictionary<GameObject, Selectable> _lastSelected = new();

    protected void GoToTargetMenu(GameObject targetMenu, int index = 0)
    {
        targetMenu.SetActive(true);
        _previousMenuStack.Push(gameObject);
        _lastSelected[gameObject] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

        if (_lastSelected.TryGetValue(targetMenu, out Selectable selectable))
        {
            selectable.Select();
            return;
        }
        Selectable[] selectables = targetMenu.GetComponentsInChildren<Selectable>();
        selectables[index].Select();
    }

    public virtual void Back()
    {
        if (_previousMenuStack.Count == 0)
        {
            return;
        }
        var _previousMenu = _previousMenuStack.Peek();
        _previousMenu.SetActive(true);
        _lastSelected[_previousMenu].Select();
        _previousMenuStack.Pop();
        
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }
}

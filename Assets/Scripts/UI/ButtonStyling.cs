using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStyling : MonoBehaviour, ISelectHandler, IDeselectHandler, ICancelHandler
{
    public string m_NativeText;

    private Button _buttonUI;
    private TextMeshProUGUI _textUI;
    private MenuHandler _menuHandler;
    private string _englishText;
    private float _alpha;

    private float Alpha
    {
        get => _alpha;
        set
        {
            _alpha = value;
            SetAlpha(_alpha);
        }
    }
    private const float DEFAULT_ALPHA = 1f;
    private const float SELECTED_ALPHA = 0.33f;
    
    private GameManager m_GameManager = null;

    void Awake()
    {
        _buttonUI = GetComponent<Button>();
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        _menuHandler = GetComponentInParent<MenuHandler>();
        _englishText = _textUI.text;
        
        SetAlpha(DEFAULT_ALPHA);
    }

    private void Start()
    {
        if (!m_GameManager)
        {
            m_GameManager = GameManager.instance_gm;
        }
        SetText();
    }

    private void OnEnable()
    {
        if (!m_GameManager)
        {
            return;
        }
        SetText();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void SetText()
    {
        if (m_NativeText == "..." || m_NativeText == String.Empty)
        {
            return;
        }
        if (GameSetting.m_Language == Language.English)
        {
            _textUI.text = _englishText;
        }
        else if (GameSetting.m_Language == Language.Korean)
        {
            _textUI.text = m_NativeText;
        }
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        SetTextColor(true);
        Alpha = SELECTED_ALPHA;
        StartCoroutine(ShowSelectedEffect());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetTextColor(false);
        Alpha = DEFAULT_ALPHA;
        StopAllCoroutines();
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (_menuHandler == null)
            return;
        _menuHandler.Back();
    }

    private IEnumerator ShowSelectedEffect()
    {
        while (true)
        {
            Alpha += 0.024f;
            if (Alpha > 1f)
            {
                Alpha = SELECTED_ALPHA;
            }
            yield return null;
        }
    }

    private void SetTextColor(bool isSelected)
    {
        if (isSelected)
            _textUI.color = new Color32(54, 219, 54, 255);
        else
            _textUI.color = new Color32(83, 221, 233, 255);
    }

    private void SetAlpha(float alpha)
    {
        _alpha = alpha;
        _buttonUI.image.color = new Color(_buttonUI.image.color.r, _buttonUI.image.color.g, _buttonUI.image.color.b, alpha);
        _textUI.color = new Color(_textUI.color.r, _textUI.color.g, _textUI.color.b, alpha);
    }
}

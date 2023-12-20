using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class EndingCredit : MonoBehaviour
{
    public TextMeshProUGUI m_CreditText;
    public RectTransform m_CreditTextRectTransform;
    public TextErrorMessage m_TextErrorMessage;

    private Dictionary<Language, string> _creditJsonData = new();

    private const float DEFAULT_SCROLL_SPEED = 48f;
    private const float FAST_SCROLL_SPEED = 384f;
    private float _currentScrollSpeed;
    private bool _isQuitting;
    private bool _isFirePress;

    private void Start()
    {
        SystemManager.PlayState = PlayState.OnStageResult;
        _creditJsonData = Utility.LoadDataFile<Dictionary<Language, string>>(GameManager.ResourceFilePath, "resources2.dat").jsonData;
        if (_creditJsonData.TryGetValue(GameSetting.CurrentLanguage, out var creditText))
            m_CreditText.SetText(creditText);
        else
            m_TextErrorMessage.DisplayText("FileLoadException");
        
        FadeScreenService.ScreenFadeIn(0f);
        AudioService.LoadMusics("Main");
        AudioService.PlayMusic("Ending");
        
        InGameInputController.Action_OnFireInput += OnFireInvoked;
        InGameInputController.Action_OnBombInput += QuitEndingCredit;
        InGameInputController.Action_OnEscapeInput += QuitEndingCredit;
    }

    private void OnDestroy()
    {
        InGameInputController.Action_OnFireInput -= OnFireInvoked;
        InGameInputController.Action_OnBombInput -= QuitEndingCredit;
        InGameInputController.Action_OnEscapeInput -= QuitEndingCredit;
    }

    private void OnFireInvoked(bool isPressed)
    {
        _isFirePress = isPressed;
    }

    private void Update ()
    {
        if (transform.localPosition.y >= m_CreditTextRectTransform.rect.height)
        {
            _currentScrollSpeed = 0f;
            QuitEndingCredit(3f);
            return;
        }
        
        _currentScrollSpeed = _isFirePress ? FAST_SCROLL_SPEED : DEFAULT_SCROLL_SPEED;

        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.y += _currentScrollSpeed * Time.deltaTime;
        transform.localPosition = newLocalPos;
    }

    private void QuitEndingCredit(bool isPressed)
    {
        if (!isPressed)
            return;
        QuitEndingCredit(0f);
    }

    private void QuitEndingCredit(float delay)
    {
        if (_isQuitting)
            return;
        _isQuitting = true;
        StartCoroutine(QuitEnding(delay));
    }

    private IEnumerator QuitEnding(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        FadeScreenService.ScreenFadeOut(2f);
        AudioService.FadeOutMusic(2f);
        yield return new WaitForSeconds(3f);
        
        AudioService.StopMusic();
        SystemManager.Instance.FinishEndingCredit();
    }
}
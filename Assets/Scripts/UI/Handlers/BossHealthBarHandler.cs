using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum BossHealthBarState {
    ScrollUp,
    ScrollingUp,
    ScrollDown,
    ScrollingDown
}

public class BossHealthBarHandler : MonoBehaviour
{
    [SerializeField] private RectTransform m_BossHealthBar;
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private Color m_IdleColor;
    [SerializeField] private Color m_LowHealthColor;

    private EnemyUnit _enemyUnit;
    private float _interpolateY;
    private float _healthRate = 1f;
    private IEnumerator _currentScrollCoroutine;
    private BossHealthBarState m_BossHealthBarState = BossHealthBarState.ScrollUp;
    private float _deltaHeight;
    
    private const float SCROLLING_SPEED = 1f;
    private const string ProgressBarMaterialVarName = "_ProgressBar";
    private const string MaterialColorVarName = "_ColorMask";

    private void Awake()
    {
        _deltaHeight = GetComponent<RectTransform>().rect.height;
        StageManager.Action_BossHealthBar += StartHealthListener;
    }

    private void OnDestroy()
    {
        StageManager.Action_BossHealthBar -= StartHealthListener;
    }

    // private void Update()
    // {
    //     if (_healthRate <= 0f)
    //         return;
    //     HealthAnimTime += Time.deltaTime;
    //     Health = Mathf.Lerp(FromHealth, ToHealth, HealthAnimTime / DamageDelay);
    //     if (Health < 0)
    //     {
    //         ToHealth = Health = HealthMax;
    //         HealthAnimTime = 0;
    //     }
    // }

    private float InterpolateY
    {
        get => _interpolateY;
        set {
            _interpolateY = value;
            SetHealthBarPosition();
        }
    }

    private void SetHealthBarPosition() {
        var hpBarPos = m_BossHealthBar.anchoredPosition;
        m_BossHealthBar.anchoredPosition = new Vector2(hpBarPos.x, Mathf.Lerp(0f, - _deltaHeight, _interpolateY));
    }

    private void StartHealthListener(EnemyUnit enemyUnit) {
        enemyUnit.m_EnemyHealth.Action_OnHealthChanged += SetHealthRate;
        enemyUnit.m_EnemyHealth.Action_OnHealthChanged += CheckHealthBarLowState;
        _enemyUnit = enemyUnit;
        StartScrollDownBar();
        enemyUnit.m_EnemyDeath.Action_OnKilled += StartScrollUpBar;
        enemyUnit.m_EnemyDeath.Action_OnRemoved += StartScrollUpBar;
        SetHealthRate();
        CheckHealthBarLowState();
    }

    private void StartScrollDownBar() {
        if (_currentScrollCoroutine != null) {
            StopCoroutine(_currentScrollCoroutine);
        }
        _currentScrollCoroutine = ScrollDownBar();
        StartCoroutine(_currentScrollCoroutine);
    }

    private void StartScrollUpBar() {
        if (_currentScrollCoroutine != null) {
            StopCoroutine(_currentScrollCoroutine);
        }
        _currentScrollCoroutine = ScrollUpBar();
        StartCoroutine(_currentScrollCoroutine);
    }

    private IEnumerator ScrollDownBar() {
        m_BossHealthBarState = BossHealthBarState.ScrollingDown;
        while (m_BossHealthBarState == BossHealthBarState.ScrollingDown) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, 1f, SCROLLING_SPEED * Time.deltaTime);
            if (InterpolateY >= 1f) {
                m_BossHealthBarState = BossHealthBarState.ScrollDown;
                InterpolateY = 1f;
                break;
            }
            yield return new WaitForFrames(1);
        }
    }

    private IEnumerator ScrollUpBar() {
        m_BossHealthBarState = BossHealthBarState.ScrollingUp;
        while (m_BossHealthBarState == BossHealthBarState.ScrollingUp) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, 0f, SCROLLING_SPEED * Time.deltaTime);
            if (InterpolateY <= 0f) {
                m_BossHealthBarState = BossHealthBarState.ScrollUp;
                InterpolateY = 0f;
                break;
            }
            yield return new WaitForFrames(1);
        }
    }
    
    private void SetHealthRate() {
        try {
            _healthRate = _enemyUnit.m_EnemyHealth.HealthPercent;
        }
        catch(NullReferenceException) {
            _healthRate = 0f;
        }

        var propId = Shader.PropertyToID(ProgressBarMaterialVarName);
        m_HealthBar.material.SetFloat(propId, _healthRate);
        //m_HealthBar.fillAmount = _healthRate;
    }

    private void CheckHealthBarLowState() {
        var propId = Shader.PropertyToID(MaterialColorVarName);
        var barColor = (_healthRate <= 0.1f) ? m_LowHealthColor : m_IdleColor;
        m_HealthBar.material.SetColor(propId, barColor);
    }
}

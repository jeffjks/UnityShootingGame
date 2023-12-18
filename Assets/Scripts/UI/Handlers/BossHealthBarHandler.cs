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
    private BossHealthBarState _bossHealthBarState = BossHealthBarState.ScrollUp;
    private float _deltaHeight;
    private Material _bossHealthMaterial;
    
    private const float ScrollingSpeed = 1f;
    private readonly int _progressBarPropId = Shader.PropertyToID("_ProgressBar");
    private readonly int _colorMaskPropId = Shader.PropertyToID("_ColorMask");

    private void Awake()
    {
        _deltaHeight = GetComponent<RectTransform>().rect.height;
        StageManager.Action_BossHealthBar += StartHealthListener;
        _bossHealthMaterial = new Material(m_HealthBar.material);
        m_HealthBar.material = _bossHealthMaterial;
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
        _bossHealthBarState = BossHealthBarState.ScrollingDown;
        while (_bossHealthBarState == BossHealthBarState.ScrollingDown) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, 1f, ScrollingSpeed * Time.deltaTime);
            if (InterpolateY >= 1f) {
                _bossHealthBarState = BossHealthBarState.ScrollDown;
                InterpolateY = 1f;
                break;
            }
            yield return new WaitForFrames(1);
        }
    }

    private IEnumerator ScrollUpBar() {
        _bossHealthBarState = BossHealthBarState.ScrollingUp;
        while (_bossHealthBarState == BossHealthBarState.ScrollingUp) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, 0f, ScrollingSpeed * Time.deltaTime);
            if (InterpolateY <= 0f) {
                _bossHealthBarState = BossHealthBarState.ScrollUp;
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
        
        _bossHealthMaterial.SetFloat(_progressBarPropId, _healthRate);
        //m_HealthBar.fillAmount = _healthRate;
    }

    private void CheckHealthBarLowState() {
        var barColor = (_healthRate <= 0.1f) ? m_LowHealthColor : m_IdleColor;
        _bossHealthMaterial.SetColor(_colorMaskPropId, barColor);
    }
}

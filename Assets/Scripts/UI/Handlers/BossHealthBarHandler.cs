using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
struct TopUIPosition
{
    public float m_hpTopY, m_hpBottomY, m_uiTopY, m_uiBottomY;

    public TopUIPosition(float m_hpTopY, float m_hpBottomY, float m_uiTopY, float m_uiBottomY) {
        this.m_hpTopY = m_hpTopY;
        this.m_hpBottomY = m_hpBottomY;
        this.m_uiTopY = m_uiTopY;
        this.m_uiBottomY = m_uiBottomY;
    }
}

enum BossHealthBarState {
    ScrollUp,
    ScrollingUp,
    ScrollDown,
    ScrollingDown
}

public class BossHealthBarHandler : MonoBehaviour
{
    [SerializeField] private RectTransform m_BossHealthBar = null, m_TopUI = null;
    [SerializeField] private Image m_HealthBar = null;
    [SerializeField] private Sprite m_HealthBarGreen = null, m_HealthBarRed = null;
    [SerializeField] private TopUIPosition m_TopUIPosition = new TopUIPosition(0.48f, -0.32f, 0f, -0.48f);

    [HideInInspector] public EnemyUnit m_EnemyUnit;

    private float m_InterpolateY;
    private float m_HealthRate = 1f;
    private const float POSITION_Y_UP = 0f;
    private const float POSITION_Y_DOWN = 1f;
    private const float SCROLLING_SPEED = 1f;
    private IEnumerator m_CurrentScrollCoroutine;
    private BossHealthBarState m_BossHealthBarState = BossHealthBarState.ScrollUp;

    public float InterpolateY {
        get { return m_InterpolateY; }
        set {
            m_InterpolateY = value;
            SetHealthBarPosition();
        }
    }

    private void SetHealthBarPosition() {
        Vector3 HPVector3 = m_BossHealthBar.position;
        Vector3 TopUIVector3 = m_TopUI.localPosition;

        HPVector3.Set(HPVector3.x, Mathf.Lerp(m_TopUIPosition.m_hpTopY, m_TopUIPosition.m_hpBottomY, InterpolateY), HPVector3.z);
        TopUIVector3.Set(TopUIVector3.x, Mathf.Lerp(m_TopUIPosition.m_uiTopY, m_TopUIPosition.m_uiBottomY, InterpolateY), TopUIVector3.z);

        m_BossHealthBar.position = HPVector3;
        m_TopUI.localPosition = TopUIVector3;
    }

    public void StartHealthListener(EnemyUnit enemyUnit) {
        enemyUnit.m_EnemyHealth.Action_OnHealthChanged += SetHealthRate;
        enemyUnit.m_EnemyHealth.Action_OnHealthChanged += CheckHealthBarLowState;
        m_EnemyUnit = enemyUnit;
        StartCoroutine(ScrollDownBar());
        enemyUnit.m_EnemyDeath.Action_OnDying += StartScrollUpBar;
    }

    private void StartScrollDownBar() {
        if (m_CurrentScrollCoroutine != null) {
            StopCoroutine(m_CurrentScrollCoroutine);
        }
        m_CurrentScrollCoroutine = ScrollDownBar();
        StartCoroutine(m_CurrentScrollCoroutine);
    }

    private void StartScrollUpBar() {
        if (m_CurrentScrollCoroutine != null) {
            StopCoroutine(m_CurrentScrollCoroutine);
        }
        m_CurrentScrollCoroutine = ScrollUpBar();
        StartCoroutine(m_CurrentScrollCoroutine);
    }

    private IEnumerator ScrollDownBar() {
        m_BossHealthBarState = BossHealthBarState.ScrollingDown;
        m_BossHealthBar.gameObject.SetActive(true);
        while (m_BossHealthBarState == BossHealthBarState.ScrollingDown) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, POSITION_Y_DOWN, SCROLLING_SPEED * Time.deltaTime);
            if (InterpolateY >= POSITION_Y_DOWN) {
                m_BossHealthBarState = BossHealthBarState.ScrollDown;
                break;
            }
            yield return null;
        }
    }

    private IEnumerator ScrollUpBar() {
        m_BossHealthBarState = BossHealthBarState.ScrollingUp;
        while (m_BossHealthBarState == BossHealthBarState.ScrollingUp) {
            InterpolateY = Mathf.MoveTowards(InterpolateY, POSITION_Y_UP, SCROLLING_SPEED * Time.deltaTime);
            if (InterpolateY <= POSITION_Y_UP) {
                m_BossHealthBarState = BossHealthBarState.ScrollUp;
                m_BossHealthBar.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }
    
    private void SetHealthRate() {
        try {
            m_HealthRate = m_EnemyUnit.m_EnemyHealth.m_HealthPercent;
        }
        catch(System.NullReferenceException) {
            m_HealthRate = 0f;
        }

        m_HealthBar.fillAmount = m_HealthRate;
    }

    private void CheckHealthBarLowState() {
        if (m_HealthRate <= 0.1f) {
            m_HealthBar.sprite = m_HealthBarRed;
            m_EnemyUnit.m_EnemyHealth.Action_OnHealthChanged -= CheckHealthBarLowState;
        }
        else {
            m_HealthBar.sprite = m_HealthBarGreen;
        }
    }

    public void InitPosition() {
        InterpolateY = POSITION_Y_UP;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class TopUIPosition
{
    public float m_hpTopY, m_hpBottomY, m_uiTopY, m_uiBottomY;

    public TopUIPosition(float m_hpTopY, float m_hpBottomY, float m_uiTopY, float m_uiBottomY) {
        this.m_hpTopY = m_hpTopY;
        this.m_hpBottomY = m_hpBottomY;
        this.m_uiTopY = m_uiTopY;
        this.m_uiBottomY = m_uiBottomY;
    }
}

public class BossHealthHandler : MonoBehaviour
{
    [SerializeField] private RectTransform m_BossHealthBar = null, m_TopUI = null;
    [SerializeField] private Image m_HealthBar = null;
    [SerializeField] private Sprite m_HealthBarGreen = null, m_HealthBarRed = null;
    [SerializeField] private TopUIPosition m_TopUIPosition = new TopUIPosition(0.48f, -0.32f, 0f, -0.48f);

    [HideInInspector] public EnemyUnit m_EnemyUnitBoss;

    private float m_PositionY;
    private float m_HealthRate = 1f;
    private SystemManager m_SystemManager = null;

    void Start()
    {
        m_SystemManager = SystemManager.instance_sm;
    }

    void Update()
    {
        float m_hpTopY = m_TopUIPosition.m_hpTopY;
        float m_hpBottomY = m_TopUIPosition.m_hpBottomY;
        float m_uiTopY = m_TopUIPosition.m_uiTopY;
        float m_uiBottomY = m_TopUIPosition.m_uiBottomY;

        Vector3 HPVector3 = m_BossHealthBar.position;
        Vector3 TopUIVector3 = m_TopUI.localPosition;

        if (m_SystemManager.m_PlayState == 1) {
            m_PositionY = Mathf.MoveTowards(m_PositionY, 1f, 1f * Time.deltaTime);
        }
        else {
            m_PositionY = Mathf.MoveTowards(m_PositionY, 0f, 1f * Time.deltaTime);
        }

        HPVector3.Set(HPVector3.x, Mathf.Lerp(m_TopUIPosition.m_hpTopY, m_TopUIPosition.m_hpBottomY, m_PositionY), HPVector3.z);
        TopUIVector3.Set(TopUIVector3.x, Mathf.Lerp(m_TopUIPosition.m_uiTopY, m_TopUIPosition.m_uiBottomY, m_PositionY), TopUIVector3.z);

        m_BossHealthBar.position = HPVector3;
        m_TopUI.localPosition = TopUIVector3;

        try {
            m_HealthRate = m_EnemyUnitBoss.m_Health / m_EnemyUnitBoss.m_MaxHealth;
        }
        catch(System.NullReferenceException) {
            m_HealthRate = 1f;
        }
        m_HealthBar.fillAmount = m_HealthRate;

        if (m_HealthRate > 0.1f) {
            m_HealthBar.sprite = m_HealthBarGreen;
        }
        else {
            m_HealthBar.sprite = m_HealthBarRed;
        }
    }
}

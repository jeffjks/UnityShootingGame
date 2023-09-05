using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewManager : MonoBehaviour
{
    public GameObject m_OverviewPanel;
    public OverviewGemRenderer m_OverviewGemRenderer;

    private void Awake()
    {
        Instantiate(m_OverviewGemRenderer, new Vector3(-100f, 0f, 0f), Quaternion.identity);
    }

    private void OnEnable()
    {
        SystemManager.Action_OnShowOverview += OpenWindow;
        SystemManager.Action_OnNextStage += CloseWindow;
    }

    private void OnDisable()
    {
        SystemManager.Action_OnShowOverview -= OpenWindow;
        SystemManager.Action_OnNextStage -= CloseWindow;
    }

    private void OpenWindow()
    {
        m_OverviewPanel.SetActive(true);
    }

    private void CloseWindow(bool hasNextStage)
    {
        m_OverviewPanel.SetActive(false);
    }
}

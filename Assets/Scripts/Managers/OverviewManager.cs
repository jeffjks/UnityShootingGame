using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewManager : MonoBehaviour
{
    public GameObject m_OverviewPanel;
    public Canvas m_Canvas;
    
    private static OverviewManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        m_Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        SystemManager.Action_OnQuitInGame += DestroySelf;
    }
    
    private void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
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

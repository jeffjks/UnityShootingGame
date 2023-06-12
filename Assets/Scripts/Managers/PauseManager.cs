using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public PauseMenuHandler m_PauseMenuHandler;
    public GameObject m_PauseMenuUI;
    public IngameInputController m_InGameInputController;
    
    private SystemManager m_SystemManager = null;
    private bool _pauseEnabled = true;
    private const float PAUSE_DELAY = 2f;

    public static bool IsGamePaused = false;

    private void OnEnable()
    {
        m_InGameInputController.Action_OnPause += Pause;
    }

    private void OnDisable()
    {
        m_InGameInputController.Action_OnPause -= Pause;
    }

    private void Start()
    {
        m_SystemManager = SystemManager.instance_sm;
    }

    private void Pause() {
        if (!_pauseEnabled)
        {
            return;
        }
        if (SystemManager.PlayState == PlayState.OnStageResult)
        {
            return;
        }
        if (IsGamePaused)
        {
            return;
        }


        IsGamePaused = true;
        Time.timeScale = 0f;
        AudioService.PauseAudio();
        
        //m_EventSystemUI.SetActive(true);
        m_PauseMenuHandler.gameObject.SetActive(true);
        m_PauseMenuUI.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.firstSelectedGameObject.GetComponent<Selectable>().Select();
    }

    public void Resume()
    {
        if (!IsGamePaused)
        {
            return;
        }
        
        IsGamePaused = false;
        Time.timeScale = 1;
        _pauseEnabled = false;
        AudioService.UnpauseAudio();
        
        //m_EventSystemUI.SetActive(false);
        m_PauseMenuHandler.gameObject.SetActive(false);
        m_PauseMenuUI.SetActive(false);
        StartCoroutine(PauseEnabled());
    }

    private IEnumerator PauseEnabled() {
        yield return new WaitForSeconds(PAUSE_DELAY);
        _pauseEnabled = true;
    }

    public void QuitGame() {
        StopAllCoroutines();
        _pauseEnabled = true;

        m_SystemManager.QuitGame();
    }
}
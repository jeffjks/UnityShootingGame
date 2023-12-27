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
    public PausePopupMenuHandler m_PausePopupMenuHandler;
    public GameObject m_PauseMenuUI;
    public Transform m_InGameTransform;
    
    private const int PAUSE_DELAY_FRAME = 120;

    public static bool IsGamePaused = false;
    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        InGameInputController.Action_OnPauseInput += OpenPauseMenu;
        InGameInputController.Action_OnEscapeInput += OpenPauseMenu;
    }

    private void OnDestroy()
    {
        InGameInputController.Action_OnPauseInput -= OpenPauseMenu;
        InGameInputController.Action_OnEscapeInput -= OpenPauseMenu;
    }

    private void OpenPauseMenu(bool isPressed)
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (SystemManager.PlayState is PlayState.OnStageResult or PlayState.OnStageTransition)
            return;
        if (IsGamePaused)
            return;
        if (!isPressed)
            return;

        Pause();
        
        CriticalStateSystem.SetCriticalState(20);
        
        m_PauseMenuHandler.gameObject.SetActive(true);
        m_PauseMenuUI.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.firstSelectedGameObject.GetComponent<Selectable>().Select();
    }

    public void OpenPopupMenu(string messageNative, string message)
    {
        Pause();
        
        m_PausePopupMenuHandler.ShowPopupMessage(messageNative, message);
    }

    private void Pause()
    {
        IsGamePaused = true;
        Time.timeScale = 0f;
        AudioService.PauseAudio();
    }

    public void ClosePauseMenu()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (!IsGamePaused)
            return;

        Unpause();
        
        CriticalStateSystem.SetCriticalState(PAUSE_DELAY_FRAME);
        
        //m_EventSystemUI.SetActive(false);
        m_PauseMenuHandler.gameObject.SetActive(false);
        m_PauseMenuUI.SetActive(false);
    }

    private void Unpause()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;
        AudioService.UnpauseAudio();
    }

    public void QuitGame() {
        StopAllCoroutines();
        
        IsGamePaused = false;
        CriticalStateSystem.SetCriticalState(20);
        AudioService.StopMusic();
        AudioService.StopAllSound();
        SystemManager.QuitGame(null);
    }
}
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
    public Transform m_InGameTransform;
    
    private const int PAUSE_DELAY_FRAME = 120;

    public static bool IsGamePaused = false;
    private static PauseManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        InGameInputController.Action_OnPauseInput += Pause;
        InGameInputController.Action_OnEscapeInput += Pause;
    }

    private void OnDestroy()
    {
        InGameInputController.Action_OnPauseInput -= Pause;
        InGameInputController.Action_OnEscapeInput -= Pause;
    }

    private void Pause(bool isPressed)
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (SystemManager.PlayState is PlayState.OnStageResult or PlayState.OnStageTransition)
            return;
        if (IsGamePaused)
            return;
        if (!isPressed)
            return;

        IsGamePaused = true;
        Time.timeScale = 0f;
        AudioService.PauseAudio();
        //PlayerUnit.IsControllable = false;
        CriticalStateSystem.SetCriticalState(20);
        
        //m_EventSystemUI.SetActive(true);
        m_PauseMenuHandler.gameObject.SetActive(true);
        m_PauseMenuUI.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.firstSelectedGameObject.GetComponent<Selectable>().Select();
    }

    public void Resume()
    {
        if (CriticalStateSystem.InCriticalState)
            return;
        if (!IsGamePaused)
            return;
        
        IsGamePaused = false;
        AudioService.UnpauseAudio();
        Time.timeScale = 1;
        //PlayerUnit.IsControllable = true;
        
        CriticalStateSystem.SetCriticalState(PAUSE_DELAY_FRAME);
        
        //m_EventSystemUI.SetActive(false);
        m_PauseMenuHandler.gameObject.SetActive(false);
        m_PauseMenuUI.SetActive(false);
    }

    public void QuitGame() {
        StopAllCoroutines();
        
        IsGamePaused = false;
        CriticalStateSystem.SetCriticalState(20);
        AudioService.StopMusic();
        AudioService.StopAllSound();
        SystemManager.Instance.QuitGame(null);
    }
}
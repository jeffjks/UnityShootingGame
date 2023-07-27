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
    
    private IngameInputController _inGameInputController;
    private bool _pauseEnabled = true;
    private const float PAUSE_DELAY = 2f;

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
        
        _inGameInputController = EventSystem.current.gameObject.GetComponent<IngameInputController>();
        _inGameInputController.Action_OnPauseInput += Pause;
        _inGameInputController.Action_OnEscapeInput += Pause;
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
        PlayerUnit.IsControllable = false;
        
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
        _pauseEnabled = false;
        AudioService.UnpauseAudio();
        Time.timeScale = 1;
        PlayerUnit.IsControllable = true;
        
        CriticalStateSystem.SetCriticalState(10);
        
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
        
        IsGamePaused = false;
        _pauseEnabled = true;
        SystemManager.Instance.QuitGame(AudioService.UnpauseAudio);
    }
}
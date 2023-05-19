using UnityEngine;
using System.Collections;
using System.IO;

public class ReplayMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;

    private const byte MAX_REPLAY_NUMBER = 5;
    private bool m_State;
    private string[] m_FilePath = new string[MAX_REPLAY_NUMBER];
    private bool m_OnEnable = false;

    void Update()
	{
        if (!m_State)
            return;

        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    StartReplay(m_Selection);
                    break;
                case 1:
                    StartReplay(m_Selection);
                    break;
                case 2:
                    StartReplay(m_Selection);
                    break;
                case 3:
                    StartReplay(m_Selection);
                    break;
                case 4:
                    StartReplay(m_Selection);
                    break;
                default:
                    Back();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();
        
        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    void OnEnable()
    {
        m_State = true;

        if (!m_OnEnable) {
            SetFilePath();
            m_OnEnable = true;
        }

        for (int i = 0; i < MAX_REPLAY_NUMBER; i++) {
            if (!System.IO.File.Exists(m_FilePath[i])) {
                m_IsEnabled[i] = false;
            }
        }
    }

    private void SetFilePath()
    {
        for (int i = 0; i < MAX_REPLAY_NUMBER; i++) {
            m_FilePath[i] = m_GameManager.m_ReplayDirectory + "replay" + i + ".rep";
        }
    }

    private void StartReplay(int num) {
        if (!m_IsEnabled[num]) {
            AudioService.PlaySound("CancelUI");
            return;
        }
        m_State = false;
        m_SystemManager.m_GameMode = GameMode.GAMEMODE_REPLAY;
        m_GameManager.m_ReplayNum = (byte) num;
        ScreenFadeService.ScreenFadeOut();
        AudioService.PlaySound("SallyUI");
    }

    private void Back() {
        m_PreviousMenu.SetActive(true);
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }
}
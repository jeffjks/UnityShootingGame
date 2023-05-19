using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRankingDifficultyHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_RankingMenu;

    void OnEnable()
    {
        if (m_GameManager.m_NetworkAvailable) {
            m_IsEnabled[0] = m_GameManager.m_IsOnline;
            m_IsEnabled[1] = m_GameManager.m_IsOnline;
            m_IsEnabled[2] = m_GameManager.m_IsOnline;
        }
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 0:
                    SelectDifficulty(Difficulty.NORMAL);
                    break;
                case 1:
                    SelectDifficulty(Difficulty.EXPERT);
                    break;
                case 2:
                    SelectDifficulty(Difficulty.HELL);
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

    private void SelectDifficulty(int difficulty) {
        m_SystemManager.SetDifficulty(difficulty);
        
        m_RankingMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        gameObject.SetActive(false);
    }

    private void Back() {
        m_PreviousMenu.SetActive(true);
        AudioService.PlaySound("CancelUI");
        gameObject.SetActive(false);
    }
}

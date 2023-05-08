using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDifficultyHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_SelectAttributesMenu;
    public GameObject m_PlayerPreview;

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            SelectDifficulty((byte) m_Selection);
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
        m_SystemManager.m_GameMode = GameMode.GAMEMODE_NORMAL;
        
        m_PlayerPreview.SetActive(true);
        m_SelectAttributesMenu.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Back() {
        m_PreviousMenu.SetActive(true);
        CancelSound();
        gameObject.SetActive(false);
    }
}

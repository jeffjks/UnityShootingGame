using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDifficultyHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_SelectAttributesPanel;
    public GameObject m_PlayerPreview;

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Fire1")) {
            SelectDifficulty((byte) m_Selection);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();

        MoveCursorVertical(moveRawVertical);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void SelectDifficulty(byte difficulty) {
        m_GameManager.m_Difficulty = difficulty;
        m_GameManager.m_PracticeState = false;
        
        m_PlayerPreview.SetActive(true);
        m_SelectAttributesPanel.SetActive(true);
        ConfirmSound();
        gameObject.SetActive(false);
    }

    private void Back() {
        m_PreviousPanel.SetActive(true);
        CancelSound();
        gameObject.SetActive(false);
    }
}

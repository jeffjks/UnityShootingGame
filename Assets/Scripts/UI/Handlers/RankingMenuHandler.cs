using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_RankingMenu;
    public GameObject m_MainLogo;
    public NetworkDisplayRankingScore m_NetworkDisplayRankingScore;
    public GameObject[] m_DifficultyText = new GameObject[3];

    void OnEnable() {
        m_MainLogo.SetActive(false);
        m_DifficultyText[m_SystemManager.GetDifficulty()].SetActive(true);
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");

        if (moveRawHorizontal != 0) { // 좌우 방향키
            switch(m_Selection) {
                case 0:
                    if (!m_isHorizontalAxisInUse)
                        MovePage(moveRawHorizontal);
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Fire1"))
            Back();
        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void MovePage(int move) {
        if (m_NetworkDisplayRankingScore.m_Active) {
            if (m_NetworkDisplayRankingScore.TurnOverPage(move)) {
                ConfirmSound();
            }
        }
    }

    private void Back() {
        if (m_NetworkDisplayRankingScore.m_Active) {
            m_MainLogo.SetActive(true);
            m_PreviousMenu.SetActive(true);
            m_DifficultyText[m_SystemManager.GetDifficulty()].SetActive(false);
            CancelSound();
            m_RankingMenu.SetActive(false);
        }
    }
}

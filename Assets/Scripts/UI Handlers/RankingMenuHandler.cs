using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_RankingMenu;
    public GameObject m_MainLogo;
    public NetworkDisplayRankingScore m_NetworkDisplayRankingScore;

    void OnEnable() {
        m_MainLogo.SetActive(false);
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
        if (m_NetworkDisplayRankingScore.TurnOverPage(move)) {
            ConfirmSound();
        }
    }

    private void Back() {
        m_MainLogo.SetActive(true);
        m_PreviousPanel.SetActive(true);
        CancelSound();
        m_RankingMenu.SetActive(false);
    }
}

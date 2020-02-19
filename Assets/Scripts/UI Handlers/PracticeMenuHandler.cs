using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_PracticePanel;
    public GameObject m_SelectAttributesPanel;
    public GameObject m_PlayerPreview;
    private PracticeInfo m_PracticeInfo;

    void OnEnable() {
        UpdateValues();
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
        
        if (moveRawHorizontal != 0) { // 좌우 방향키
            switch(m_Selection) {
                case 0:
                    if (!m_isHorizontalAxisInUse)
                        m_PracticeInfo.m_Stage += moveRawHorizontal;
                    break;
                case 1:
                    if (!m_isHorizontalAxisInUse)
                        m_PracticeInfo.m_Difficulty += (byte) moveRawHorizontal;
                    break;
                case 2:
                    if (!m_isHorizontalAxisInUse)
                        m_PracticeInfo.m_BossOnly = !m_PracticeInfo.m_BossOnly;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Fire1")) { // Fire 키
            switch(m_Selection) {
                case 2:
                    m_PracticeInfo.m_BossOnly = !m_PracticeInfo.m_BossOnly;
                    break;
                case 3:
                    PracticeStart();
                    break;
                case 4:
                    Back();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Back();

        if (m_PracticeInfo.m_Stage < 0)
            m_PracticeInfo.m_Stage = 4;
        else if (m_PracticeInfo.m_Stage > 4)
            m_PracticeInfo.m_Stage = 0;

        if (m_PracticeInfo.m_Difficulty < 0)
            m_PracticeInfo.m_Difficulty = 2;
        else if (m_PracticeInfo.m_Difficulty > 2)
            m_PracticeInfo.m_Difficulty = 0;

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {
        m_PracticeInfo = m_GameManager.m_PracticeInfo;
    }

    private void SetText() {
        if (m_GameManager.m_Language == 0) {
            m_Text[0].text = "Stage " + (m_PracticeInfo.m_Stage + 1);
            switch (m_PracticeInfo.m_Difficulty) {
                case 0:
                    m_Text[1].text = Difficulty.DIFFICULTY1;
                    break;
                case 1:
                    m_Text[1].text = Difficulty.DIFFICULTY2;
                    break;
                case 2:
                    m_Text[1].text = Difficulty.DIFFICULTY3;
                    break;
                default:
                    m_Text[1].text = "Unknown";
                    break;
            }
            if (m_PracticeInfo.m_BossOnly) {
                m_Text[2].text = "Boss";
            }
            else {
                m_Text[2].text = "Field";
            }
        }
        else {
            m_Text[0].text = "스테이지 " + (m_PracticeInfo.m_Stage + 1);
            switch (m_PracticeInfo.m_Difficulty) {
                case 0:
                    m_Text[1].text = Difficulty.DIFFICULTY1;
                    break;
                case 1:
                    m_Text[1].text = Difficulty.DIFFICULTY2;
                    break;
                case 2:
                    m_Text[1].text = Difficulty.DIFFICULTY3;
                    break;
                default:
                    m_Text[1].text = "알 수 없음";
                    break;
            }
            if (m_PracticeInfo.m_BossOnly) {
                m_Text[2].text = "보스전";
            }
            else {
                m_Text[2].text = "필드전";
            }
        }
    }

    private void PracticeStart() {
        m_GameManager.m_PracticeState = true;
        m_GameManager.m_PracticeInfo = m_PracticeInfo;
        
        m_PlayerPreview.SetActive(true);
        m_SelectAttributesPanel.SetActive(true);
        ConfirmSound();
        m_PracticePanel.SetActive(false);
    }

    private void Back() {
        m_GameManager.m_PracticeInfo = m_PracticeInfo;
        SetText();
        CancelSound();
        m_PreviousPanel.SetActive(true);
        m_PracticePanel.SetActive(false);
    }
}
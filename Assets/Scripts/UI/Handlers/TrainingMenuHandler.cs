using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_TrainingMenu;
    public GameObject m_SelectAttributesMenu;
    public GameObject m_PlayerPreview;
    public GameObject m_MainLogo;
    private TrainingInfo m_TrainingInfo;

    void OnEnable() {
        UpdateValues();
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
                        m_TrainingInfo.m_Stage += moveRawHorizontal;
                    break;
                case 1:
                    if (!m_isHorizontalAxisInUse)
                        m_TrainingInfo.m_Difficulty += moveRawHorizontal;
                    break;
                case 2:
                    if (!m_isHorizontalAxisInUse)
                        m_TrainingInfo.m_BossOnly = !m_TrainingInfo.m_BossOnly;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Fire1")) { // Fire 키
            switch(m_Selection) {
                case 2:
                    //m_TrainingInfo.m_BossOnly = !m_TrainingInfo.m_BossOnly;
                    break;
                case 3:
                    TrainingStart();
                    break;
                case 4:
                    Back();
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();
        else if (Input.GetButtonDown("Fire2"))
            Back();

        if (m_TrainingInfo.m_Stage < 0)
            m_TrainingInfo.m_Stage = 4;
        else if (m_TrainingInfo.m_Stage > 4)
            m_TrainingInfo.m_Stage = 0;

        if (m_TrainingInfo.m_Difficulty > Difficulty.HELL)
            m_TrainingInfo.m_Difficulty = Difficulty.HELL;
        else if (m_TrainingInfo.m_Difficulty < 0)
            m_TrainingInfo.m_Difficulty = 0;

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {
        m_TrainingInfo = m_SystemManager.m_TrainingInfo;
    }

    private void SetText() {
        if (GameSetting.m_Language == Language.English) {
            m_Text[0].text = "Stage " + (m_TrainingInfo.m_Stage + 1);
            switch (m_TrainingInfo.m_Difficulty) {
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
            if (m_TrainingInfo.m_BossOnly) {
                m_Text[2].text = "Boss";
            }
            else {
                m_Text[2].text = "Field";
            }
        }
        else {
            m_Text[0].text = "" + (m_TrainingInfo.m_Stage + 1);
            switch (m_TrainingInfo.m_Difficulty) {
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
            if (m_TrainingInfo.m_BossOnly) {
                m_Text[2].text = "보스전";
            }
            else {
                m_Text[2].text = "필드전";
            }
        }
    }

    private void TrainingStart() {
        m_SystemManager.m_GameMode = GameMode.GAMEMODE_TRAINING;
        m_SystemManager.m_TrainingInfo = m_TrainingInfo;
        m_SystemManager.SetDifficulty(m_TrainingInfo.m_Difficulty);
        
        m_PlayerPreview.SetActive(true);
        m_SelectAttributesMenu.SetActive(true);
        AudioService.PlaySound("ConfirmUI");
        m_TrainingMenu.SetActive(false);
    }

    private void Back() {
        m_SystemManager.m_TrainingInfo = m_TrainingInfo;
        SetText();
        AudioService.PlaySound("CancelUI");
        m_MainLogo.SetActive(true);
        m_PreviousMenu.SetActive(true);
        m_TrainingMenu.SetActive(false);
    }
}
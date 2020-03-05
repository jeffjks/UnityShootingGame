using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GraphicsMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_GraphicsPanel;

    private int m_ResolutionOptions;
    private bool m_FullScreenOptions;
    private int m_GraphicsQuality;
    private bool m_AntiAliasing;

    private int m_PreviousResolution;
    private bool m_PreviousFullScreen;
    private int m_PreviousGraphicsQuality;
    private bool m_PreviousAntiAliasing;

    private int m_MaxResolutionNumber;
    private int m_MaxGraphicsQuality;

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
                        m_ResolutionOptions += moveRawHorizontal;
                    break;
                case 1:
                    if (!m_isHorizontalAxisInUse)
                        m_FullScreenOptions = !m_FullScreenOptions;
                    break;
                case 2:
                    if (!m_isHorizontalAxisInUse)
                        m_GraphicsQuality -= moveRawHorizontal;
                    break;
                case 3:
                    if (!m_isHorizontalAxisInUse)
                        m_AntiAliasing = !m_AntiAliasing;
                    break;
                default:
                    break;
            }
        }

        if (Input.GetButtonDown("Fire1")) { // Fire 키
            switch(m_Selection) {
                case 1:
                    m_FullScreenOptions = !m_FullScreenOptions;
                    break;
                case 3:
                    m_AntiAliasing = !m_AntiAliasing;
                    break;
                case 4:
                    Apply();
                    break;
                case 5:
                    Cancel();
                    break;
                default:
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
        else if (Input.GetButtonDown("Fire2"))
            Cancel();

        if (m_ResolutionOptions < 0)
            m_ResolutionOptions = m_MaxResolutionNumber - 1;
        else if (m_ResolutionOptions >= m_MaxResolutionNumber)
            m_ResolutionOptions = 0;

        if (m_GraphicsQuality < 0)
            m_GraphicsQuality = m_MaxGraphicsQuality - 1;
        else if (m_GraphicsQuality >= m_MaxGraphicsQuality)
            m_GraphicsQuality = 0;

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {
        m_ResolutionOptions = m_GameManager.GetResolutionNumber();
        m_FullScreenOptions = m_GameManager.m_FullScreen;
        m_GraphicsQuality = m_GameManager.GetGraphicsQualityNumber();
        m_AntiAliasing = m_GameManager.m_AntiAliasing;

        m_MaxResolutionNumber = m_GameManager.m_MaxResolutionNumber;
        m_MaxGraphicsQuality = m_GameManager.m_MaxGraphicsQuality;
    }

    private void SetText() {
        Resolution resolution = m_GameManager.GetResolution(m_ResolutionOptions);
        m_Text[0].text = resolution.width + " x " + resolution.height;
        m_Text[1].text = m_GameManager.GetFullScreen(m_FullScreenOptions);
        m_Text[2].text = m_GameManager.GetGraphicsQuality(m_GraphicsQuality);
        m_Text[3].text = m_GameManager.GetAntiAliasing(m_AntiAliasing);
    }

    private void Apply() {
        m_GameManager.SetScreen(m_ResolutionOptions, m_FullScreenOptions);
        m_GameManager.SetGraphicsQuality(m_GraphicsQuality);
        m_GameManager.SetAntiAliasing(m_AntiAliasing);
        PlayerPrefs.Save();
        ConfirmSound();

        m_PreviousPanel.SetActive(true);
        m_GraphicsPanel.SetActive(false);
    }

    private void Cancel() {
        UpdateValues();
        SetText();
        CancelSound();
        m_PreviousPanel.SetActive(true);
        m_GraphicsPanel.SetActive(false);
    }
}
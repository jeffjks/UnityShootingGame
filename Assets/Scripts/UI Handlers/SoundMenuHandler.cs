using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundMenuHandler : GameUI
{
    public GameObject m_PreviousPanel;
    public GameObject m_SoundPanel;

    private int m_MusicVolumeOptions;
    private int m_SoundVolumeOptions;

    private int m_Delay;

    void OnEnable() {
        UpdateValues();
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
        
        if (moveRawHorizontal != 0) {
            switch(m_Selection) {
                case 0:
                    if (m_Delay == 0 || m_Delay >= 40)
                        m_MusicVolumeOptions += moveRawHorizontal;
                    break;
                case 1:
                    m_SoundVolumeOptions += moveRawHorizontal;
                    break;
                default:
                    break;
            }
        }

        if (moveRawHorizontal != 0) {
            if (m_Delay < 40) {
                m_Delay += 1;
            }
        }
        else {
            m_Delay = 0;
        }

        if (Input.GetButtonDown("Fire1")) {
            switch(m_Selection) {
                case 2:
                    Apply();
                    break;
                case 3:
                    Cancel();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        m_MusicVolumeOptions = (int) Mathf.Clamp (m_MusicVolumeOptions, 0f, 100f);
        m_SoundVolumeOptions = (int) Mathf.Clamp (m_SoundVolumeOptions, 0f, 100f);

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void UpdateValues() {
        m_MusicVolumeOptions = (int) m_GameManager.m_MusicVolume;
        m_SoundVolumeOptions = (int) m_GameManager.m_SoundVolume;
    }

    private void SetText() {
        m_Text[0].text = m_MusicVolumeOptions.ToString();
        m_Text[1].text = m_SoundVolumeOptions.ToString();
    }

    private void Apply() {
        m_GameManager.SetMusicVolume(m_MusicVolumeOptions);
        m_GameManager.SetSoundVolume(m_SoundVolumeOptions);
        PlayerPrefs.Save();

        m_PreviousPanel.SetActive(true);
        ConfirmSound();
        m_SoundPanel.SetActive(false);
    }

    private void Cancel() {
        UpdateValues();
        SetText();
        m_PreviousPanel.SetActive(true);
        CancelSound();
        m_SoundPanel.SetActive(false);
    }
}
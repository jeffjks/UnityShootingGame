using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundMenuHandler : GameUI
{
    public GameObject m_PreviousMenu;
    public GameObject m_SoundMenu;
    public GameObject m_MainLogo;

    private int m_MusicVolume, m_SoundVolume;
    private int m_PreviousMusicVolume, m_PreviousSoundVolume;

    private int m_Delay;

    void OnEnable() {
        m_MusicVolume = (int) m_GameManager.m_MusicVolume;
        m_SoundVolume = (int) m_GameManager.m_SoundVolume;
        m_PreviousMusicVolume = (int) m_GameManager.m_MusicVolume;
        m_PreviousSoundVolume = (int) m_GameManager.m_SoundVolume;
        if (m_MainLogo != null)
            m_MainLogo.SetActive(false);
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");
        
        if (moveRawHorizontal != 0) {
            switch(m_Selection) {
                case 0:
                    if (m_Delay == 0 || m_Delay >= 40)
                        m_MusicVolume += moveRawHorizontal;
                        m_GameManager.SetMusicVolume(m_MusicVolume);
                    break;
                case 1:
                    if (m_Delay == 0 || m_Delay >= 40)
                        m_SoundVolume += moveRawHorizontal;
                        m_GameManager.SetSoundVolume(m_SoundVolume);
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
        else if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
        else if (Input.GetButtonDown("Fire2"))
            Cancel();

        m_MusicVolume = (int) Mathf.Clamp (m_MusicVolume, 0f, 100f);
        m_SoundVolume = (int) Mathf.Clamp (m_SoundVolume, 0f, 100f);

        SetText();

        MoveCursorVertical(moveRawVertical);
        MoveCursorHorizontal(moveRawHorizontal, false);
        m_Selection = EndToStart(m_Selection, m_Total);
        SetColor();
	}

    private void SetText() {
        m_Text[0].text = m_MusicVolume.ToString();
        m_Text[1].text = m_SoundVolume.ToString();
    }

    private void Apply() {
        m_GameManager.SetMusicVolume(m_MusicVolume);
        m_GameManager.SetSoundVolume(m_SoundVolume);
        PlayerPrefs.Save();
        
        AudioService.PlaySound("ConfirmUI");
        PreviousMenu();
    }

    private void Cancel() {
        m_GameManager.SetMusicVolume(m_PreviousMusicVolume);
        m_GameManager.SetSoundVolume(m_PreviousSoundVolume);
        SetText();
        AudioService.PlaySound("CancelUI");
        PreviousMenu();
    }

    private void PreviousMenu() {
        if (m_MainLogo != null)
            m_MainLogo.SetActive(true);
        m_PreviousMenu.SetActive(true);
        m_SoundMenu.SetActive(false);
    }
}
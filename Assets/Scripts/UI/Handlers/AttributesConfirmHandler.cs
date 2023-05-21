using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttributesConfirmHandler : GameUI
{
    public AttributesSelectHandler m_SelectAttributesHandler;

    private Action Action_startStage;
    private bool m_Enable = false;

    void Start()
    {
        Action_startStage += StartStage;
    }

    private void StartStage()
    {
        SceneManager.LoadScene("Stage1");
    }
    
    void Update()
	{
        if (!m_Enable) {
            if (m_SelectAttributesHandler.m_State == 0) {
                if (Input.GetButtonUp("Fire1")) {
                    m_Enable = true;
                }
            }
            SetColorDeselected();
        }
        else {
            if (m_SelectAttributesHandler.m_State == 0)
                CheckInput();
            SetColor();
        }
	}

    private void CheckInput() {
        if (Input.GetButtonDown("Fire1")) {
            Confirm();
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();

        else if (Input.GetButtonDown("Fire2"))
            Back();
    }

    private void Confirm() { // Select Confirm (Sally)
        m_SelectAttributesHandler.m_State = -1;
        ScreenEffectService.ScreenFadeOut(2f, Action_startStage);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    private void Back() {
        m_SelectAttributesHandler.m_State = 1;
        m_Enable = false;
    }

    private void SetColorDeselected() {
        m_Image[0].color = m_ColorDeselectedImage;
        m_Text[0].color = m_ColorDeselectedText;
    }
}

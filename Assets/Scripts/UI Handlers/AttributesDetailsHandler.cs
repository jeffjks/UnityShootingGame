using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesDetailsHandler : AttributeSelectButtonUI
{
    public AttributesSelectHandler m_SelectAttributesHandler;
    public GameObject m_PreviousPanel;
    public byte m_Attributes;
    public int[] m_Cost;
    public PlayerPreview1 m_PlayerPreview;

    // protected Transform[] m_SelectedAttribute;
    protected int m_TotalAttributes;
    private int m_OriginalSelection;
    private int m_PreviousSelction;

    void Awake()
    {
        m_TotalAttributes = transform.childCount;
        m_GameManager = GameManager.instance_gm;
        m_Selection = m_GameManager.m_CurrentAttributes.GetAttributes(m_Attributes);
        m_OriginalSelection = m_Selection;

        FindAudioSource();
    }

    void OnEnable()
    {
        m_PreviousSelction = m_GameManager.m_CurrentAttributes.GetAttributes(m_Attributes);
    }

    void Update()
	{
        int moveRawHorizontal = (int) Input.GetAxisRaw("Horizontal");

        if (!m_Enable) {
            if (m_SelectAttributesHandler.m_State == 2) {
                if (Input.GetButtonUp("Fire1")) {
                    m_Enable = true;
                }
            }
        }
        else {
            if (m_SelectAttributesHandler.m_State == 2) {
                CheckInput();
                bool has_changed = MoveCursorHorizontal(-moveRawHorizontal, true);

                if (has_changed) {
                    m_Selection = EndAndStart(m_Selection, m_TotalAttributes);
                    m_GameManager.m_CurrentAttributes.SetAttributes(m_Attributes, m_Selection);
                    m_PlayerPreview.SetPreviewDesign();
                }
            }
        }

        m_Selection = EndAndStart(m_Selection, m_TotalAttributes);
        ShowOnlySelected();
	}

    protected void ShowOnlySelected() {
        for (int i = 0; i < m_TotalAttributes; i+=1) {
            if (m_Selection != i)
                transform.GetChild(i).gameObject.SetActive(false);
            else
                transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void CheckInput() {
        if (Input.GetButtonDown("Fire1")) {
            int cost_limit = m_SelectAttributesHandler.m_AvailableCost - m_GameManager.m_UsedCost;
            int cost_need = m_Cost[m_Selection] - m_Cost[m_PreviousSelction];

            if (cost_limit >= cost_need)
                SelectDetail(m_Attributes, cost_need);
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
            Back();

        else if (Input.GetButtonDown("Fire2"))
            Back();
    }

    private void SelectDetail(byte attribute, int cost_need) {
        m_GameManager.m_CurrentAttributes.SetAttributes(attribute, m_Selection);
        m_GameManager.m_UsedCost += cost_need;
        m_SelectAttributesHandler.m_State = 1;
        m_Enable = false;
        ConfirmSound();
        m_PreviousPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Back() {
        m_Selection = m_OriginalSelection;
        m_GameManager.m_CurrentAttributes.SetAttributes(m_Attributes, m_OriginalSelection);
        m_SelectAttributesHandler.m_State = 1;
        m_Enable = false;
        m_PlayerPreview.SetPreviewDesign();
        CancelSound();
        m_PreviousPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}

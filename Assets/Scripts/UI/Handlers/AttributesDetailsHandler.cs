using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesDetailsHandler : AttributeSelectButtonUI
{
    public AttributesSelectHandler m_SelectAttributesHandler;
    public GameObject m_PreviousMenu;
    public byte m_Attributes;
    public int[] m_Cost;
    public AttributesHandler m_AttributesHandler;
    
    protected int m_TotalAttributes;
    private int m_OriginalSelection;
    private int m_PreviousSelction;

    void Awake()
    {
        m_TotalAttributes = transform.childCount;
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
        m_GameManager = GameManager.instance_gm;
        m_Selection = m_PlayerManager.m_CurrentAttributes.GetAttributes(m_Attributes);
    }

    void OnEnable()
    {
        m_PreviousSelction = m_PlayerManager.m_CurrentAttributes.GetAttributes(m_Attributes);
        m_OriginalSelection = m_Selection;
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
                    m_PlayerManager.m_CurrentAttributes.SetAttributes(m_Attributes, m_Selection);
                    SetPreviewDesign();
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
            int cost_limit = m_SelectAttributesHandler.m_AvailableCost - m_SystemManager.m_UsedCost;
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
        m_PlayerManager.m_CurrentAttributes.SetAttributes(attribute, m_Selection);
        m_SystemManager.m_UsedCost += cost_need;
        m_SelectAttributesHandler.m_State = 1;
        m_Enable = false;
        AudioService.PlaySound("ConfirmUI");
        m_PreviousMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Back() {
        m_Selection = m_OriginalSelection;
        m_PlayerManager.m_CurrentAttributes.SetAttributes(m_Attributes, m_OriginalSelection);
        //Debug.Log(m_PlayerManager.m_CurrentAttributes.GetAttributesCode());
        m_SelectAttributesHandler.m_State = 1;
        m_Enable = false;
        SetPreviewDesign();
        AudioService.PlaySound("CancelUI");
        m_PreviousMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void SetPreviewDesign() {
        m_AttributesHandler.SetPreviewDesign();
    }
}

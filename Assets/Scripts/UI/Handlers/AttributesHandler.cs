using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttributesHandler : AttributeSelectButtonUI
{
    public AttributesSelectHandler m_SelectAttributesHandler;
    public GameObject m_PlayerPreviewCamera;
    public GameObject m_PreviousMenu;
    public GameObject m_MainLogo;
    public RectTransform m_RectTransform;
    public PlayerPreviewManager[] m_PlayerPreview = new PlayerPreviewManager[2];

    public GameObject[] m_DetailsPanels;
    public float m_DefaultY;
    
    private float m_TargetY;
    private float m_yVelocity;

    void OnEnable() {
        m_Enable = true;
        SetPreviewDesign();
        m_MainLogo.SetActive(false);

        m_TargetY = m_Selection*60+m_DefaultY;
        m_RectTransform.localPosition = new Vector2(m_RectTransform.localPosition[0], m_TargetY);
    }

    void Update()
	{
        int moveRawVertical = (int) Input.GetAxisRaw("Vertical");
        SetPosition();

        if (!m_Enable) {
            if (m_SelectAttributesHandler.m_State == 1) {
                if (Input.GetButtonUp("Fire2")) {
                    m_Enable = true;
                }
            }
        }
        else {
            if (m_SelectAttributesHandler.m_State == 1) {
                CheckInput();
                MoveCursorVertical(moveRawVertical);
            }
        }
        
        m_Selection = EndAndStart(m_Selection, m_Total);
        if (m_SelectAttributesHandler.m_State == 1)
            SetColor();
        else
            SetDeselectedColor();
	}

    private void CheckInput() {

        if (Input.GetButtonDown("Fire1")) {
            if (m_Selection < m_DetailsPanels.Length) {
                SelectDetails(m_Selection);
            }
            else if (m_Selection == m_DetailsPanels.Length) {
                Complete();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Back();

        if (Input.GetButtonDown("Fire2"))
            Back();
    }

    private void SetPosition() { // m_RectTransform.localPosition.y를 m_TargetY 값으로
        m_TargetY = m_Selection*60+m_DefaultY;
        float y_value = Mathf.SmoothDamp(m_RectTransform.localPosition.y, m_TargetY, ref m_yVelocity, 0.1f);
        //m_RectTransform.transform.DOLocalMoveY(m_TargetY, 0.1f).SetEase(Ease.InOutQuad);
        m_RectTransform.localPosition = new Vector3(m_RectTransform.localPosition.x, y_value, m_RectTransform.localPosition.z);
    }

    private void SelectDetails(int num) {
        try {
            AudioService.PlaySound("ConfirmUI");
            m_DetailsPanels[num].SetActive(true);
            m_SelectAttributesHandler.m_State = 2;
            m_Enable = false;
            m_RectTransform.localPosition = new Vector2(m_RectTransform.localPosition[0], m_TargetY);
            //m_yVelocity = 0f;
            gameObject.SetActive(false);
        } catch {
            return;
        }
    }

    public void SetPreviewDesign() {
        for (int i = 0; i < m_PlayerPreview.Length; i++) {
            m_PlayerPreview[i].SetPreviewDesign();
        }
    }

    private void Complete() {
        AudioService.PlaySound("ConfirmUI");
        m_SelectAttributesHandler.m_State = 0;
        m_Enable = false;
    }

    private void Back() {
        AudioService.PlaySound("CancelUI");
        m_Enable = false;
        m_PreviousMenu.SetActive(true);
        m_MainLogo.SetActive(true);
        m_SelectAttributesHandler.gameObject.SetActive(false);
        m_PlayerPreviewCamera.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningSignController : MonoBehaviour
{
    public GameObject m_WarningSignController;

    private void Awake()
    {
        StageManager.Action_BossWarningSign += PlayWarningSign;
    }
    
    private void PlayWarningSign()
    {
        AudioService.PlaySound("BossAlert1");
        m_WarningSignController.SetActive(true);
    }
}

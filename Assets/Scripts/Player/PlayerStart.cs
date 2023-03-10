using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject m_DronePart; // Shot Spawner
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;
    public GameObject m_PlayerShield;

    private int m_Vspeed;
    private PlayerManager m_PlayerManager = null;
    private SystemManager m_SystemManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerController.DisableInvincible();
        SetAttributes();

        if (m_SystemManager.GetCurrentStage() == 0 && !m_SystemManager.m_BossOnlyState) {
            StartCoroutine(SpawnEvent());
        }
        else if (m_SystemManager.GetCurrentStage() == 5) {
            transform.position = new Vector3(transform.position.x, 2f, Depth.PLAYER);
            enabled = false;
            return;
        }
        else {
            EndOpening();
        }
    }

    void Update() {
        m_PlayerController.SetVerticalSpeed(m_Vspeed / Application.targetFrameRate);
        //m_PlayerController.m_Vector2 = 
        //transform.Translate(Vector3.up * m_Vspeed / Application.targetFrameRate, Space.World);
    }

    private IEnumerator SpawnEvent() {
        yield return new WaitForSeconds(2.5f);
        m_Vspeed = 2523;
        while (m_Vspeed > -1024) {
            m_Vspeed -= 77;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        EndOpening();
        yield break;
    }

    private void EndOpening() {
        m_PlayerController.EnableInvincible(m_PlayerController.m_ReviveInvincibleTime);
        m_PlayerManager.m_PlayerControlable = true;
        m_Vspeed = 0;
        enabled = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }


    private void SetAttributes() {
        m_DronePart.SetActive(false);
        
        m_SpeedPart[m_PlayerManager.m_CurrentAttributes.m_Speed].SetActive(true);

        if (m_PlayerManager.m_CurrentAttributes.m_Module != 0)
            m_ModulePart.SetActive(true);
        
        SetPlayerColors();
        
        m_DronePart.SetActive(true);
        m_PlayerShield.SetActive(true);
        //transform.GetChild(0).GetChild(11).gameObject.SetActive(true);
    }

    private void SetPlayerColors() {
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        PlayerColors playerColors = GetComponentInChildren<PlayerColors>();
        int max_meshRenderer = meshRenderer.Length;
        
        // Color
        for (int i = 0; i < 3; i++) {
            if (m_PlayerManager.m_CurrentAttributes.m_Color == i)
                for (int j = 0; j < max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
    }
}

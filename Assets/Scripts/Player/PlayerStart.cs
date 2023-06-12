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

        if (m_SystemManager.SpawnAtSpawnPointCondition()) {
            StartCoroutine(SpawnEvent());
        }
        else if (SystemManager.Stage == -1) {
            transform.position = new Vector3(transform.position.x, 2f, Depth.PLAYER);
            enabled = false;
            return;
        }
        else {
            EndSpawnEvent();
        }
    }

    void Update() {
        //m_PlayerController.m_MoveVector = new MoveVector(m_Vspeed / Application.targetFrameRate * Time.timeScale, 180f);
        Vector2Int posInt2D = m_PlayerController.m_PositionInt2D;
        m_PlayerController.m_PositionInt2D = new Vector2Int(posInt2D.x, posInt2D.y + (int) (m_Vspeed / Application.targetFrameRate * Time.timeScale));
    }

    private IEnumerator SpawnEvent() {
        yield return new WaitForMillisecondFrames(2500);
        m_Vspeed = 2523;
        while (m_Vspeed > -1024) {
            m_Vspeed -= 77;
            yield return new WaitForMillisecondFrames(100);
        }
        yield return new WaitForMillisecondFrames(500);
        EndSpawnEvent();
    }

    private void EndSpawnEvent() {
        m_PlayerController.DisableInvincibility(m_PlayerController.m_ReviveInvincibleTime);
        PlayerController.IsControllable = true;
        m_Vspeed = 0;
        enabled = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }


    private void SetAttributes() {
        m_DronePart.SetActive(false);
        
        m_SpeedPart[PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed)].SetActive(true);

        if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Module) != 0)
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
            if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Color) == i)
                for (int j = 0; j < max_meshRenderer; j++) {
                meshRenderer[j].material = playerColors.m_Materials[i];
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject m_DronePart; // Shot Spawner
    public GameObject[] m_SpeedPart = new GameObject[3];
    public GameObject m_ModulePart;

    private int _verticalSpeed;

    void Start()
    {
        SetAttributes();

        if (SystemManager.Instance.SpawnAtSpawnPointCondition()) {
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
        //m_PlayerController.m_MoveVector = new MoveVector(_verticalSpeed / Application.targetFrameRate * Time.timeScale, 180f);
        Vector2Int posInt2D = m_PlayerController.m_PositionInt2D;
        m_PlayerController.m_PositionInt2D = new Vector2Int(posInt2D.x, posInt2D.y + (int) (_verticalSpeed / Application.targetFrameRate * Time.timeScale));
    }

    private IEnumerator SpawnEvent() {
        yield return new WaitForMillisecondFrames(2500);
        _verticalSpeed = 2523;
        while (_verticalSpeed > -1024) {
            _verticalSpeed -= 77;
            yield return new WaitForMillisecondFrames(100);
        }
        yield return new WaitForMillisecondFrames(500);
        EndSpawnEvent();
    }

    private void EndSpawnEvent() {
        PlayerInvincibility.SetInvincibility(PlayerInvincibility.REVIVE_TIME);
        PlayerController.IsControllable = true;
        _verticalSpeed = 0;
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
        //m_PlayerShield.SetActive(true);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public PlayerMovement m_PlayerMovement;
    public PlayerColorDatas m_PlayerColorData;
    public GameObject m_DronePart; // Shot Spawner
    public GameObject[] m_SpeedParts = new GameObject[3];
    public GameObject m_SubWeaponPart;

    private int _verticalSpeed;

    void Start()
    {
        SetAttributes();
        PlayerUnit.IsControllable = false;

        if (SystemManager.Instance.SpawnAtSpawnPointCondition()) {
            StartCoroutine(SpawnEvent());
        }
        else if (SystemManager.Stage == -1) {
            transform.position = new Vector3(transform.position.x, 2f, Depth.PLAYER);
            enabled = false;
        }
        else {
            EndSpawnEvent();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        //m_PlayerMovement.m_MoveVector = new MoveVector(_verticalSpeed / Application.targetFrameRate * Time.timeScale, 180f);
        var playerPos = m_PlayerMovement.transform.position;
        playerPos.y += _verticalSpeed * Time.timeScale / Application.targetFrameRate / 256f;
        m_PlayerMovement.transform.position = playerPos;
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
        PlayerUnit.IsControllable = true;
        _verticalSpeed = 0;
        enabled = false;
    }

    void OnDestroy() {
        StopAllCoroutines();
    }


    private void SetAttributes() {
        m_DronePart.SetActive(false);
        
        m_SpeedParts[PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Speed)].SetActive(true);

        if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.SubWeaponIndex) != 0)
            m_SubWeaponPart.SetActive(true);
        
        SetPlayerColors();
        
        m_DronePart.SetActive(true);
        //m_PlayerShield.SetActive(true);
        //transform.GetChild(0).GetChild(11).gameObject.SetActive(true);
    }

    private void SetPlayerColors() {
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();
        int max_meshRenderer = meshRenderer.Length;
        
        // Color
        for (int i = 0; i < 3; i++) {
            if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Color) == i) {
                for (int j = 0; j < max_meshRenderer; j++)
                {
                    meshRenderer[j].material = m_PlayerColorData.playerColorMaterial[i];
                }
            }
        }
    }
}
